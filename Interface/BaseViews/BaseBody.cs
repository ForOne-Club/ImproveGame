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
    /// 是否占用光标，使得不对其下层元素进行 主动类型的事件触发
    /// </summary>
    public abstract bool CanDisableMouse(UIElement target);
}