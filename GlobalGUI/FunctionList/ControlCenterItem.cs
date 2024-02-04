namespace ImproveGame.GlobalGUI.FunctionList;

public class ControlCenterItem(string nameKey) : IComparable<ControlCenterItem>
{
    public virtual ControlCenterItem Register() => CCIManager.Instance.Register(this);

    public virtual string NameKey { get; init; } = nameKey;
    public virtual bool Favorite { get; set; } = false;
    public virtual int Priority { get; set; }

    public event Func<TimerView, Color> BgColor;
    public event Func<TimerView, Color> BorderColor;

    public event Action<TimerView> OnMouseDown;
    public event Action<TimerView> OnUpdate;

    public virtual Color GetBgColor(TimerView timerView) => BgColor?.Invoke(timerView) ?? Color.Black * timerView.HoverTimer.Lerp(0.35f, 0.5f);
    public virtual Color GetBorderColor(TimerView timerView) => BorderColor?.Invoke(timerView) ?? Color.Black * 0.5f;

    public virtual void Update(TimerView timerView)
    {
        OnUpdate?.Invoke(timerView);
    }

    public virtual void MouseDown(TimerView timerView)
    {
        OnMouseDown?.Invoke(timerView);
    }

    public virtual int CompareTo(ControlCenterItem other)
    {
        if (Favorite && !other.Favorite)
        {
            return -1;
        }

        if (Priority == other.Priority)
        {
            return 0;
        }
        else if (Priority > other.Priority)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}
