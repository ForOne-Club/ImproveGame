using ImproveGame.Common.Animations;
using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.QolUISystem.UIElements;

public class XView : XUIElement
{
    public Vector4 Rounded;
    public float Border;
    public Color BgColor, BorderColor;

    public override void DrawSelf(DrawArgs drawArgs)
    {
        Vector2 pos = OwnRectangle.Position + drawArgs.DrawOffset;
        Vector2 size = OwnRectangle.Size;

        if (Border > 0 && (BgColor != Color.Transparent || BorderColor != Color.Transparent))
        {
            SDFRectangle.HasBorder(pos, size, Rounded, BgColor, Border, BorderColor);
        }
        else if (BgColor != Color.Transparent)
        {
            SDFRectangle.NoBorder(pos, size, Rounded, BgColor);
        }
    }
}
