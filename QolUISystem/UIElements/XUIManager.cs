using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.QolUISystem.UIElements;

public class XUIManager : XView
{
    public override void Update()
    {
        if (OuterRectangle.Size != Main.ScreenSize.ToVector2())
        {
            ScreenSizeChange();
        }
        base.Update();
    }

    /// <summary>
    /// 屏幕大小改变时调用
    /// </summary>
    protected virtual void ScreenSizeChange()
    {
        Recalculate();
    }

    public override void Recalculate()
    {
        OuterRectangle = new RectangleF(0, 0, Main.screenWidth, Main.screenHeight);

        OwnRectangle = new RectangleF(
            OuterRectangle.X + Margin.Left,
            OuterRectangle.Y + Margin.Top,
            OuterRectangle.Width - Margin.Left - Margin.Right,
            OuterRectangle.Height - Margin.Top - Margin.Bottom);

        ContentRectangle = new RectangleF(
            OwnRectangle.X + Padding.Left,
            OwnRectangle.Y + Padding.Top,
            OwnRectangle.Width - Padding.Left - Padding.Right,
            OwnRectangle.Height - Padding.Top - Padding.Bottom);

        foreach (XUIElement child in Children)
        {
            child.Recalculate();
        }
    }
}
