using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.Common.ModSystems.MarqueeSystem;

public interface IMarqueeItem
{
    public bool CanDraw();

    public RectangleF GetMarquee();

    public Color GetBorderColor();

    public Color GetBackgroundColor();

    public void PreDraw(ref bool drawVanilla, RectangleF rectangle, Color backgroundColor, Color borderColor) { }

    public void PostDraw(RectangleF rectangle, Color backgroundColor, Color borderColor) { }
}
