
using Terraria.GameInput;

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
    /// 是否不可选中
    /// </summary>
    public virtual bool IsNotSelectable => false;

    /// <summary>
    /// 鼠标在当前 UI 某一个元素上时调用此方法，返回 <see langword="true"/> 此元素会占用光标，防止下层 UI 触发鼠标事件
    /// </summary>
    public abstract bool CanSetFocusTarget(UIElement target);

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        CheckWhetherRecalculate(out bool recalculate);

        if (recalculate)
        {
            Recalculate();
        }
    }

    private Vector2 _lastScreenSize = PlayerInput.OriginalScreenSize / Main.UIScale;
    /// <summary>
    /// 总是执行
    /// </summary>
    public virtual void CheckWhetherRecalculate(out bool recalculate)
    {
        recalculate = false;

        Vector2 currentScreenSize = PlayerInput.OriginalScreenSize / Main.UIScale;
        if (currentScreenSize != _lastScreenSize)
        {
            recalculate = true;
            _lastScreenSize = currentScreenSize;
        }
    }
}