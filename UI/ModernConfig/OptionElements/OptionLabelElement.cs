using ImproveGame.Common.Configs;
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

public class OptionLabelElement : View
{
    public OptionLabelElement(ModConfig config, string optionName, int reservedWidth = 60)
    {
        Config = config;
        OptionName = optionName;
        ReservedWidth = reservedWidth;

        RelativeMode = RelativeMode.Vertical;
        Spacing = new Vector2(4);

        _textWidth = ChatManager.GetStringSize(FontAssets.MouseText.Value, DisplayText, Vector2.One).X + 10;
        Width.Set(_textWidth, 0f);
        Height.Set(40f, 0f);

        OverflowHidden = true;
        currentTextSlideSpeed = 0.01f;
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
        var textCenter = new Vector2(position.X + 10, center.Y + UIConfigs.Instance.GeneralFontOffsetY);
        
        if (_textWidth > Width.Pixels)
        {
            float deltaWidth = _textWidth - Width.Pixels; // 文字超出的宽度
            float textOffset = RealTextSlideFactor * deltaWidth;
            textCenter.X -= textOffset; // 文字滑动
        }
        
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, DisplayText, textCenter,
            Color.White, Color.Black, 0f, textOrigin, Vector2.One, -1f, 1.5f);
    }

    private void UpdateTextSlide()
    {
        var rateFactor = CountRefreshRate.CurrentRefreshRateFactor;
        textSlideFactor += currentTextSlideSpeed * rateFactor;
        if (textSlideFactor > 1.5)
            currentTextSlideSpeed = -0.01f;
        if (textSlideFactor < -0.5)
            currentTextSlideSpeed = 0.01f;
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        UpdateTextSlide();
        
        Rectangle scissorRectangle = sb.GraphicsDevice.ScissorRectangle;
        SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

        sb.End();
        Rectangle clippingRectangle = GetClippingRectangle(sb);
        Rectangle adjustedClippingRectangle =
            Rectangle.Intersect(clippingRectangle, sb.GraphicsDevice.ScissorRectangle);
        sb.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
        sb.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
            OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);

        DrawText(sb);

        var rasterizerState = sb.GraphicsDevice.RasterizerState;
        sb.End();
        sb.GraphicsDevice.ScissorRectangle = scissorRectangle;
        sb.GraphicsDevice.RasterizerState = rasterizerState;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
            rasterizerState, null, Main.UIScaleMatrix);
    }

    private float textSlideFactor; // 值域 [-0.5, 1.5]
    private float currentTextSlideSpeed;
    private float RealTextSlideFactor => Math.Clamp(textSlideFactor, 0, 1); // 值域 [0, 1]

    /// <summary>
    /// 给右侧留出的适当空间
    /// </summary>
    private int ReservedWidth { get; }

    private float _textWidth;
    private string DisplayText => GetText($"{LocalizationKey}.Label");
    private string LocalizationKey => $"Configs.{Config.GetType().Name}.{OptionName}";

    private ModConfig Config { get; }
    private string OptionName { get; }
}