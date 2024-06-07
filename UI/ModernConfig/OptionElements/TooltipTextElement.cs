using ImproveGame.Core;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class TooltipTextElement : SUIText
{
    public TooltipTextElement()
    {
        TextOrKey = "";
        UseKey = false;
        TextAlign = new Vector2(0f);
        TextScale = 1.1f;
        IsWrapped = true;
        _currentTextSlideSpeed = TextSlideSpeed;
        SetSizePercent(1f);
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

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        UpdateTextSlide();
        TextOffset.Y = 0;
        float rangeHeight = GetDimensions().Height;
        if (TextSize.Y * TextScale > rangeHeight)
        {
            float deltaWidth = TextSize.Y * TextScale - rangeHeight; // 文字超出的高度
            float textOffset = RealTextSlideFactor * deltaWidth;
            TextOffset.Y = -textOffset; // 文字滑动
        }

        Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
        SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

        spriteBatch.End();
        Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
        Rectangle adjustedClippingRectangle =
            Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
        spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
        spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
            OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);

        base.DrawSelf(spriteBatch);

        var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
        spriteBatch.End();
        spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
        spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
            rasterizerState, null, Main.UIScaleMatrix);
    }

    private const float TextSlideSpeed = 0.006f;
    private float _textSlideFactor; // 值域 [-0.5, 1.5]
    private float _currentTextSlideSpeed;
    private float RealTextSlideFactor => Math.Clamp(_textSlideFactor, 0, 1); // 值域 [0, 1]
}