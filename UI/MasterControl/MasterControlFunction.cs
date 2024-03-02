using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;

namespace ImproveGame.UI.MasterControl;

public class MasterControlFunction(string nameKey) : IComparable<MasterControlFunction>
{
    public virtual MasterControlFunction Register()
    {
        // 自动创建hjson词条
        Language.GetOrRegister($"Mods.ImproveGame.MasterControl.{Name}.DisplayName", () => Name);
        Language.GetOrRegister($"Mods.ImproveGame.MasterControl.{Name}.Description", () => "");
        return MasterControlManager.Instance.Register(this);
    }

    /// <summary>
    /// 是否可用
    /// </summary>
    public event Func<bool> Available;

    /// <summary>
    /// 可用
    /// </summary>
    public virtual bool IsAvailable => Available?.Invoke() ?? true;

    /// <summary>
    /// 不可用
    /// </summary>
    public virtual bool IsUnavailable => !IsAvailable;

    /// <summary>
    /// 图标
    /// </summary>
    public virtual Texture2D Icon { get; set; } = ModAsset.DefaultIcon.Value;

    /// <summary>
    /// 没用，先留着
    /// </summary>
    public virtual Rectangle? IconRectangle { get; set; } = null;

    /// <summary>
    /// 内部名
    /// </summary>
    public virtual string Name { get; init; } = nameKey;

    /// <summary>
    /// 喜欢 (❤ ω ❤)
    /// </summary>
    public virtual bool Favorite { get; set; } = false;

    /// <summary>
    /// 优先级，越高越靠前
    /// </summary>
    public virtual int Priority { get; set; } = 0;

    public event Func<TimerView, Color> BgColor;
    public event Func<TimerView, Color> BorderColor;

    public event Action<TimerView> OnMouseDown;
    public event Action<TimerView> OnUpdate;
    public event Action OnUpdateAlways;

    /// <summary>
    /// 展示名称
    /// </summary>
    public string GetDisplayName(params object[] args) =>
        Language.GetTextValue($"Mods.ImproveGame.MasterControl.{Name}.DisplayName", args);

    /// <summary>
    /// 功能描述
    /// </summary>
    public string GetDescription(params object[] args) =>
        Language.GetTextValue($"Mods.ImproveGame.MasterControl.{Name}.Description", args);

    public virtual Color GetBgColor(TimerView timerView) =>
        BgColor?.Invoke(timerView) ?? Color.Black * timerView.HoverTimer.Lerp(0.35f, 0.5f);
    public virtual Color GetBorderColor(TimerView timerView)
    {
        if (BorderColor != null)
        {
            return BorderColor(timerView);
        }

        if (Favorite)
        {
            return UIStyle.ItemSlotBorderFav;
        }

        return Color.Black * 0.5f;
    }

    public virtual void Update(TimerView timerView)
    {
        OnUpdate?.Invoke(timerView);
    }

    /// <summary>
    /// 总是执行 <see cref="MasterControlManager.UpdateUI(GameTime)"/>
    /// </summary>
    public virtual void UpdateAlways()
    {
        OnUpdateAlways?.Invoke();
    }

    public virtual void MouseDown(TimerView timerView)
    {
        OnMouseDown?.Invoke(timerView);
    }

    #region System Interface and Object Override
    public virtual int CompareTo(MasterControlFunction other)
    {
        if (Favorite)
        {
            if (!other.Favorite)
                return -1;
        }
        else
        {
            if (other.Favorite)
                return 1;
        }

        return -Priority.CompareTo(other.Priority);
    }

    public override bool Equals(object obj) =>
        obj is MasterControlFunction cci ? Equals(cci) : base.Equals(obj);
    public bool Equals(MasterControlFunction other) => Name.Equals(other.Name);
    public override int GetHashCode() => Name.GetHashCode();
    #endregion
}
