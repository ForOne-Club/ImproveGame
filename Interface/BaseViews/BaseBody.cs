
namespace ImproveGame.Interface.BaseViews;

public abstract class BaseBody : View
{
    protected BaseBody()
    {
        Width.Percent = 1f;
        Height.Percent = 1f;
        base.Recalculate();
    }

    /// <summary>
    /// 启用 body
    /// </summary>
    public abstract bool Enabled { get; set; }

    /// <summary>
    /// 点击此 body 任意元素时候调用，返回 true 则将 body 放入顶层顶层
    /// </summary>
    public abstract bool CanPriority(UIElement target);

    /// <summary>
    /// 返回 <see langword="true"/> 会占用光标，防止下层 UI 再触发鼠标事件
    /// </summary>
    public abstract bool CanSetFocusUIElement(UIElement target);

    private float lastUIScale = Main.UIScale;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (lastUIScale != Main.UIScale)
        {
            lastUIScale = Main.UIScale;
            Recalculate();
        }
    }
}