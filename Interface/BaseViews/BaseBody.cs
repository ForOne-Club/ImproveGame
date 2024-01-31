
namespace ImproveGame.Interface.BaseViews;

public abstract class BaseBody : View
{
    protected BaseBody()
    {
        Width.Percent = Height.Percent = 1f;
        Recalculate();
    }

    /// <summary>
    /// 启用 body
    /// </summary>
    public abstract bool Enabled { get; set; }

    /// <summary>
    /// 鼠标在当前 UI 某一个元素上时调用此方法，返回 <see langword="true"/> 此元素会占用光标，防止下层 UI 触发鼠标事件
    /// </summary>
    public abstract bool CanSetFocusTarget(UIElement target);

    private float _lastUIScale = Main.UIScale;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        bool recalculate = false;

        if (_lastUIScale != Main.UIScale)
        {
            _lastUIScale = Main.UIScale;
            recalculate = true;
        }

        CheckWhetherRecalculate(out bool recalculate2);

        if (recalculate || recalculate2)
        {
            Recalculate();
        }
    }

    /// <summary>
    /// 总是执行
    /// </summary>
    public virtual void CheckWhetherRecalculate(out bool recalculate)
    {
        recalculate = false;
    }
}