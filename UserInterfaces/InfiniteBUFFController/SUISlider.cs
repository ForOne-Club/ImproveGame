#if DEBUG && false

using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Components;
using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

/// <summary>
/// 滑块
/// </summary>
[XmlElementMapping("Slider")]
internal class SUISlider : UIView
{
    public SUISlider()
    {
        Width = new Dimension(0f, 1f);
        Height = new Dimension(0f, 1f);
    }

    public float MinValue { get; set; }
    public float MaxValue { get; set; }

    public float CurrentValue { get; set; }

    protected RectangleRender Slider { get; set; } = new();

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        RectangleRender.BackgroundColor = Color.White * 0.25f;

        base.Draw(gameTime, spriteBatch);
        DrawSlider(gameTime, spriteBatch);
    }

    protected virtual void DrawSlider(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var bounds = Bounds;
        var size = Math.Min(bounds.Width, bounds.Height);

        Slider.BackgroundColor = Color.White * 0.5f;
        Slider.BorderRadius = new Vector4(size / 2f);
        Slider.Draw(InnerBounds.Position, new Vector2(size), false, SilkyUI.TransformMatrix);
    }
}

#endif