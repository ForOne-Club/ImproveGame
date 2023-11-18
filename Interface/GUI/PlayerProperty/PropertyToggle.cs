using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

/// <summary>
/// 控制器的开关按钮
/// </summary>
public class PropertyToggle : SUIImage
{
    internal bool Dragging;
    internal Vector2 Offset;

    public PropertyToggle() : base(ModAsset.Luck2.Value)
    {
        SetSizePixels(Texture.Size());
        OnUpdate +=
            _ =>
            {
                Texture = IsMouseHovering ? ModAsset.Luck3.Value : ModAsset.Luck2.Value;
            };
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        base.RightMouseDown(evt);
        // 可拖动界面
        View view = evt.Target as View;
        // 当点击的是子元素不进行移动
        if ((evt.Target == this || view is not null && view.DragIgnore ||
             evt.Target.GetType().IsAssignableFrom(typeof(UIElement))))
        {
            Offset = evt.MousePosition - GetPosPixel();
            Dragging = true;
        }
    }

    public override void RightMouseUp(UIMouseEvent evt)
    {
        base.RightMouseUp(evt);
        Dragging = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Dragging)
        {
            SetPosPixels(Main.mouseX - Offset.X, Main.mouseY - Offset.Y).Recalculate();
        }

        base.Draw(spriteBatch);
    }
}