using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements;

/// <summary>
/// 只有一个数字输入框的选项，其实就是OptionSlider的简化版
/// </summary>
public sealed class OptionNumber : ModernConfigOption
{
    private readonly SUINumericText _numericTextBox;
    internal double Min = 0;
    internal double Max = 1;
    internal double Default = 1;

    public OptionNumber(ModConfig config, string optionName) : base(config, optionName, 70)
    {
        CheckValid();
        CheckAttributes();

        var box = new View
        {
            IsAdaptiveWidth = true,
            HAlign = 1f,
            VAlign = 0.5f,
            Height = StyleDimension.Fill
        };
        box.JoinParent(this);

        bool isInt = FieldInfo.FieldType == typeof(int);
        _numericTextBox = new SUINumericText
        {
            RelativeMode = RelativeMode.Horizontal,
            BgColor = Color.Black * 0.4f,
            Rounded = new Vector4(12f),
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
        _numericTextBox.SetSizePixels(50, 28);
        _numericTextBox.JoinParent(box);
    }

    private void CheckValid()
    {
        if (FieldInfo.FieldType != typeof(int) && FieldInfo.FieldType != typeof(float) &&
            FieldInfo.FieldType != typeof(double))
            throw new Exception($"Field \"{OptionName}\" is not a int, float or double");
    }

    private void CheckAttributes()
    {
        var rangeAttribute = FieldInfo.GetCustomAttribute<RangeAttribute>();
        if (rangeAttribute is {Min: int, Max: int })
        {
            Max = (int)rangeAttribute.Max;
            Min = (int)rangeAttribute.Min;
        }

        if (rangeAttribute is {Min: float, Max: float })
        {
            Max = (float)rangeAttribute.Max;
            Min = (float)rangeAttribute.Min;
        }

        if (rangeAttribute is {Min: double, Max: double })
        {
            Max = (double)rangeAttribute.Max;
            Min = (double)rangeAttribute.Min;
        }

        var defaultValueAttributeAttribute = FieldInfo.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValueAttributeAttribute is {Value: int })
        {
            Default = (int)defaultValueAttributeAttribute.Value;
        }

        if (defaultValueAttributeAttribute is {Value: float })
        {
            Default = (float)defaultValueAttributeAttribute.Value;
        }

        if (defaultValueAttributeAttribute is {Value: double })
        {
            Default = (double)defaultValueAttributeAttribute.Value;
        }
    }

    private void SetConfigValue(double value, bool broadcast)
    {
        if (FieldInfo.FieldType == typeof(int))
            ConfigHelper.SetConfigValue(Config, FieldInfo, (int)value, broadcast);
        else if (FieldInfo.FieldType == typeof(float))
            ConfigHelper.SetConfigValue(Config, FieldInfo, (float)value, broadcast);
        else
            ConfigHelper.SetConfigValue(Config, FieldInfo, value, broadcast);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // 简直是天才的转换
        var value = float.Parse(FieldInfo.GetValue(Config)!.ToString()!);
        if (!_numericTextBox.IsWritingText)
            _numericTextBox.Value = value;
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        var dimensions = GetDimensions();
        var position = dimensions.Position();
        var size = dimensions.Size();

        // 背景板
        var panelColor = IsMouseHovering ? UIStyle.PanelBgLightHover : UIStyle.PanelBgLight;
        SDFRectangle.NoBorder(position, size, new Vector4(8f), panelColor);

        // 提示
        if (IsMouseHovering)
        {
            TooltipPanel.SetText(Tooltip);
        }
    }
}