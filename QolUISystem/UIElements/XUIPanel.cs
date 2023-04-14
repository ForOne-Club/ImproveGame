using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.QolUISystem.UIElements;

internal class XUIPanel : XView
{
    public bool Draggable;
    public bool Dragging;
    public Vector2 MouseOffset;

    public override void MouseLeftDown(List<XUIElement> targets)
    {
        MouseOffset = Position.Pixel - Main.MouseScreen;

        if (Draggable && targets[0] == this || !targets[0].PreventDragging)
        {
            Dragging = true;
        }
    }

    public override void MouseLeftUp(List<XUIElement> targets)
    {
        Dragging = false;
    }

    public override void Draw(DrawArgs drawArgs)
    {
        if (Dragging)
        {
            Position.Pixel = Main.MouseScreen + MouseOffset;
            Recalculate();
        }

        base.Draw(drawArgs);
    }
}
