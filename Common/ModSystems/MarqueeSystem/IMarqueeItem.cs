using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.Common.ModSystems.MarqueeSystem;

public interface IMarqueeItem
{
    public event Action<Vector2, Vector2> OnPreDraw;
    public event Action<Vector2, Vector2> OnPostDraw;

    public RectangleF GetMarquee();
    public bool CanDrawMarquee();
}
