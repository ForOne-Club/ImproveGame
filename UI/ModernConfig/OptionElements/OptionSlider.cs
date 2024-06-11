﻿using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public sealed class OptionSlider : ModernConfigOption
{
    private class SlideBox : View
    {
        private Vector2 MousePositionRelative => Main.MouseScreen - GetDimensions().Position();
        public event Action ValueChangeCallback;
        public event Action EndDraggingCallback;
        private float _value;
        private bool _dragging;

        public float Value
        {
            get => Math.Clamp(_value, 0, 1);
            set
            {
                float wantedValue = Math.Clamp(value, 0, 1);
                if (Math.Clamp(_value, 0, 1) != wantedValue)
                {
                    _value = Math.Clamp(value, 0, 1);
                    ValueChangeCallback?.Invoke();
                }
            }
        }

        public SlideBox()
        {
            BgColor = Color.Black * 0.3f;
            Rounded = new Vector4(6f);
            SetSizePixels(80f, 28f);
            VAlign = 0.5f;
            Rounded = new Vector4(14f);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            UpdateDragging();
            _dragging = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            if (_dragging)
            {
                _dragging = false;
                EndDraggingCallback?.Invoke();
            }
        }

        private void UpdateDragging()
        {
            var dimensions = GetDimensions();
            var size = dimensions.Size();
            float roundRadius = size.Y / 2f - 2f; // 半径
            float roundDiameter = roundRadius * 2; // 直径
            float moveWidth = size.X - roundDiameter; // 移动宽度
            float mousePositionX = MousePositionRelative.X;
            float adjustedMousePosX = mousePositionX - roundRadius; // 让圆心保持在鼠标位置
            Value = adjustedMousePosX / moveWidth;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (_dragging)
                UpdateDragging();

            // 基本字段
            var dimensions = GetDimensions();
            var position = dimensions.Position();
            var center = dimensions.Center();
            var size = dimensions.Size();

            // 圆的属性
            float roundRadius = size.Y / 2f - 2f; // 半径
            float roundDiameter = roundRadius * 2; // 直径
            float moveWidth = size.X - roundDiameter; // 移动宽度
            var roundCenter = position + new Vector2(Value * moveWidth, 0f);
            roundCenter.Y = center.Y;
            roundCenter.X += roundRadius;
            var roundLeftTop = roundCenter - new Vector2(roundRadius);

            // 颜色选择
            var innerColor = UIStyle.SliderRound;
            var borderColor = UIStyle.SliderRound;
            if (MouseInRound(roundCenter, (int)roundRadius))
                borderColor = UIStyle.SliderRoundHover;

            // 绘制
            SDFGraphics.HasBorderRound(roundLeftTop, roundDiameter, innerColor, 2f, borderColor);
        }
    }

    private SlideBox _slideBox;
    private SUINumericText _numericTextBox;

    public OptionSlider(ModConfig config, string optionName) : base(config, optionName, 140)
    {
        CheckValid();

        var box = new View
        {
            IsAdaptiveWidth = true,
            HAlign = 1f,
            VAlign = 0.5f,
            Height = StyleDimension.Fill
        };
        box.JoinParent(this);

        AddTextBox(box);
        AddSlideBox(box);
    }

    private void CheckValid()
    {
        if (FieldInfo.FieldType != typeof(int) && FieldInfo.FieldType != typeof(float) &&
            FieldInfo.FieldType != typeof(double))
            throw new Exception($"Field \"{OptionName}\" is not a int, float or double");
    }

    private void AddTextBox(View box)
    {
        bool isInt = FieldInfo.FieldType == typeof(int);
        _numericTextBox = new SUINumericText
        {
            RelativeMode = RelativeMode.Horizontal,
            BgColor = Color.Black * 0.4f,
            Rounded = new Vector4(14f),
            MinValue = Min,
            MaxValue = Max,
            InnerText =
            {
                TextAlign = new Vector2(0.5f, 0.5f),
                TextOffset = new Vector2(0f, -2f),
                MaxCharacterCount = 4,
                MaxLines = 1,
            },
            MaxLength = 4,
            DefaultValue = Default,
            Format = isInt ? "0" : "0.00",
            VAlign = 0.5f
        };
        _numericTextBox.ContentsChanged += (ref string text) =>
        {
            if (!double.TryParse(text, out var value))
                return;
            value = Math.Clamp(value, Min, Max);
            SetConfigValue(value, broadcast: false);
        };
        _numericTextBox.EndTakingInput += () =>
        {
            if (!_numericTextBox.IsValueSafe)
                return;
            SetConfigValue(_numericTextBox.Value, broadcast: true);
        };
        _numericTextBox.SetPadding(2, 2, 2, 2); // Padding影响里面的文字绘制
        _numericTextBox.SetSizePixels(48, 28);
        _numericTextBox.JoinParent(box);
    }

    private void AddSlideBox(View box)
    {
        _slideBox = new SlideBox
        {
            RelativeMode = RelativeMode.Horizontal,
            Spacing = new Vector2(4),
        };
        _slideBox.ValueChangeCallback += () =>
        {
            float value = MathHelper.Lerp((float)Min, (float)Max, _slideBox.Value);
            SetConfigValue(value, broadcast: false);
        };
        _slideBox.EndDraggingCallback += () =>
        {
            float value = MathHelper.Lerp((float)Min, (float)Max, _slideBox.Value);
            SetConfigValue(value, broadcast: true);
        };
        _slideBox.JoinParent(box);
    }

    private void SetConfigValue(double value, bool broadcast)
    {
        if (!Interactable) return;
        if (IsInt)
            ConfigHelper.SetConfigValue(Config, FieldInfo, (int)Math.Round(value), broadcast);
        else if (IsFloat)
            ConfigHelper.SetConfigValue(Config, FieldInfo, (float)value, broadcast);
        else
            ConfigHelper.SetConfigValue(Config, FieldInfo, value, broadcast);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        _slideBox.IgnoresMouseInteraction = !Interactable;
        _numericTextBox.IgnoresMouseInteraction = !Interactable;

        // 简直是天才的转换
        var value = float.Parse(FieldInfo.GetValue(Config)!.ToString()!);
        if (!_numericTextBox.IsWritingText)
            _numericTextBox.Value = value;
        _slideBox.Value = InverseLerp((float)Min, (float)Max, value);
    }

    private static float InverseLerp(float a, float b, float value) => (value - a) / (b - a);

    public override void DrawSelf(SpriteBatch sb)
    {
        base.DrawSelf(sb);

        // 提示
        if (IsMouseHovering)
        {
            TooltipPanel.SetText(Tooltip);
        }
    }

    private bool IsFloat => FieldInfo.FieldType == typeof(float);
    private bool IsInt => FieldInfo.FieldType == typeof(int);
    private bool IsDouble => FieldInfo.FieldType == typeof(double);
}