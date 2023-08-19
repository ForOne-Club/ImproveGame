namespace ImproveGame.Common.ModSystems.MarqueeSystem;

public interface IMarqueeItem
{
    public bool ShouldDrawing { get; set; }
    public Rectangle Marquee { get; }
    public Color BorderColor { get; }
    public Color BackgroundColor { get; }

    public void PreDraw(ref bool shouldDrawing, Rectangle marquee, Color backgroundColor, Color borderColor) { }

    public void PostDraw(Rectangle marquee, Color backgroundColor, Color borderColor) { }
}
