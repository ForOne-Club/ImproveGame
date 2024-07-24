﻿using ImproveGame.Common.Configs;
using ImproveGame.Core;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class SlideText : View
{
    public SlideText(string text, int reservedWidth = 60, float textScale = 1f)
    {
        _text = text;
        _textScale = textScale;
        ReservedWidth = reservedWidth;

        RelativeMode = RelativeMode.Vertical;
        Spacing = new Vector2(4);

        _textWidth = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, new Vector2(_textScale)).X + 10;
        Width.Set(_textWidth, 0f);
        Height.Set(40f, 0f);

        OverflowHidden = true;
        _currentTextSlideSpeed = TextSlideSpeed;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var width = Math.Min(_textWidth, Parent.GetInnerDimensions().Width - ReservedWidth);
        if (width != Width.Pixels)
        {
            Width.Set(width, 0f);
            Recalculate();
        }
    }

    private void DrawText(SpriteBatch sb)
    {
        var dimensions = GetDimensions();
        var position = dimensions.Position();
        var center = dimensions.Center();

        // 文字
        string text = DisplayText;
        var textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
        var textOrigin = textSize / 2f;
        textOrigin.X = 0;
        var textCenter = new Vector2(position.X + 2, center.Y + UIConfigs.Instance.GeneralFontOffsetY - 2);

        if (_textWidth > Width.Pixels)
        {
            float deltaWidth = _textWidth - Width.Pixels; // 文字超出的宽度
            float textOffset = RealTextSlideFactor * deltaWidth;
            textCenter.X -= textOffset; // 文字滑动
        }

        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, DisplayText, textCenter,
            Color.White, Color.Black, 0f, textOrigin, new Vector2(TextScale), -1f, 1.5f);
    }

    private void UpdateTextSlide()
    {
        var rateFactor = CountRefreshRate.CurrentRefreshRateFactor;
        _textSlideFactor += _currentTextSlideSpeed * rateFactor;
        if (_textSlideFactor > 1.5)
            _currentTextSlideSpeed = -TextSlideSpeed;
        if (_textSlideFactor < -0.5)
            _currentTextSlideSpeed = TextSlideSpeed;
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        UpdateTextSlide();

        DrawInClippingRectangle(sb, GetClippingRectangle(sb), DrawText);
    }

    private const float TextSlideSpeed = 0.01f;
    private float _textSlideFactor; // 值域 [-0.5, 1.5]
    private float _currentTextSlideSpeed;
    private float RealTextSlideFactor => Math.Clamp(_textSlideFactor, 0, 1); // 值域 [0, 1]

    /// <summary>
    /// 给右侧留出的适当空间
    /// </summary>
    private int ReservedWidth { get; }

    private float _textWidth;
    private float _textScale;

    public float TextScale
    {
        get => _textScale;
        set
        {
            if (_textScale == value)
                return;

            _textScale = value;
            _textWidth = ChatManager.GetStringSize(FontAssets.MouseText.Value, _text, new Vector2(_textScale)).X + 10;
        }
    }

    public string DisplayText
    {
        get => _text;
        set
        {
            if (_text == value)
                return;

            _text = value;
            _textWidth = ChatManager.GetStringSize(FontAssets.MouseText.Value, _text, new Vector2(TextScale)).X + 10;
        }
    }

    public string _text;
}