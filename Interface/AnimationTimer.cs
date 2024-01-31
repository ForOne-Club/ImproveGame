namespace ImproveGame.Interface;

public enum AnimationState : byte // 动画当前的状态
{
    Default, Opening, Closing, CompleteOpen, CompleteClose
}

public enum AnimationType : byte
{
    Linear, // 线性动画 (就一种类型, 还是一个大分类)
    Easing, // 缓动动画 (最简单的非线性动画). 如果能学到其他的非线性动画, 我就都加进来.
}

// 动画计时器
public class AnimationTimer
{
    /// <summary>
    /// 缓动系数 / 增量 / 速度
    /// </summary>
    public float Speed;

    /// <summary>
    /// 当前位置
    /// </summary>
    public float Timer;

    /// <summary>
    /// 最大位置
    /// </summary>
    public float TimerMax;

    /// <summary>
    /// 进度
    /// </summary>
    public float Schedule
    {
        get
        {
            float schedule = Timer / TimerMax;
            return Type switch
            {
                AnimationType.Linear or AnimationType.Easing or _ => schedule
            };
        }
    }

    public AnimationType Type;
    public AnimationState State;
    public Action OnOpened;
    public Action OnClosed;

    public bool Opening => State == AnimationState.Opening;
    public bool Closing => State == AnimationState.Closing;
    public bool CompleteOpen => State == AnimationState.CompleteOpen;
    public bool CompleteClose => State == AnimationState.CompleteClose;
    public bool Default => State == AnimationState.Default;
    public bool AnyOpen => State is AnimationState.Opening or AnimationState.CompleteOpen;
    public bool AnyClose => State is AnimationState.Closing or AnimationState.CompleteClose;

    /// <summary>
    /// 它的 <see cref="TimerMax"/> = 100f <br/>
    /// 它的 <see cref="Type"/> = AnimationType.Nonlinear <br/>
    /// </summary>
    public AnimationTimer(float speed = 5f, float timerMax = 100f)
    {
        Speed = speed;
        TimerMax = timerMax;
        Type = AnimationType.Easing;
    }

    /// <summary>
    /// 开启
    /// </summary>
    public virtual void Open()
    {
        State = AnimationState.Opening;
    }

    /// <summary>
    /// 开启并重置
    /// </summary>
    public virtual void OpenAndResetTimer()
    {
        State = AnimationState.Opening;
        Timer = 0f;
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void Close()
    {
        State = AnimationState.Closing;
    }

    /// <summary>
    /// 关闭并重置
    /// </summary>
    public virtual void CloseAndResetTimer()
    {
        State = AnimationState.Closing;
        Timer = TimerMax;
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    public virtual void Update()
    {
        switch (State)
        {
            case AnimationState.Opening:
                {
                    if (Type == AnimationType.Easing)
                    {
                        Timer += (TimerMax + 1 - Timer) / Speed;
                    }
                    else
                    {
                        Timer += Speed;
                    }

                    if (TimerMax - Timer < 0f)
                    {
                        Timer = TimerMax;
                        State = AnimationState.CompleteOpen;
                        OnOpened?.Invoke();
                    }
                }
                break;
            case AnimationState.Closing:
                {
                    if (Type == AnimationType.Easing)
                    {
                        Timer -= (Timer + 1) / Speed;
                    }
                    else
                    {
                        Timer -= Speed;
                    }

                    if (Timer < 0f)
                    {
                        Timer = 0;
                        State = AnimationState.CompleteClose;
                        OnClosed?.Invoke();
                    }
                }
                break;
        }
    }

    public Color Lerp(Color color1, Color color2)
    {
        return Color.Lerp(color1, color2, Schedule);
    }

    public Vector2 Lerp(Vector2 vector21, Vector2 vector22)
    {
        return new Vector2(Lerp(vector21.X, vector22.X), Lerp(vector21.Y, vector22.Y));
    }

    public float Lerp(float value1, float value2)
    {
        return value1 + (value2 - value1) * Schedule;
    }
}
