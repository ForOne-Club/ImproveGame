namespace ImproveGame.Common.ModSystems.MarqueeSystem;

public interface IMarqueeItem
{
    public bool ShouldDraw { get; set; }
    public Rectangle Marquee { get; }
    public Color BorderColor { get; }
    public Color BackgroundColor { get; }

    public void PreDrawMarquee(ref bool shouldDraw, Rectangle marquee, Color backgroundColor, Color borderColor) { }

    public void PostDrawMarquee(Rectangle marquee, Color backgroundColor, Color borderColor) { }
}
