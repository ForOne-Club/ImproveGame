using ImproveGame.Core;

namespace ImproveGame.UIFramework;

#region Enums
/// <summary>
/// 动画当前的状态
/// </summary>
public enum AnimationState : byte
{
    Opening, Closing, Opened, Closed
}

/// <summary>
/// 动画类型
/// </summary>
public enum AnimationType : byte
{
    /// <summary> 线性动画 </summary>
    Linear,
    /// <summary> 缓动动画 </summary>
    Easing,
}
#endregion

/// <summary>
/// 动画计时器 <br/>
/// </summary>
public class AnimationTimer(float speed = 5f, float timerMax = 100f, AnimationType animationType = AnimationType.Easing)
{
    /// <summary>
    /// 缓动系数 / 增量 / 速度
    /// </summary>
    public float Speed = speed;

    /// <summary>
    /// 当前位置
    /// </summary>
    public float Timer;

    /// <summary>
    /// 最大位置
    /// </summary>
    public float TimerMax = timerMax;

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

    public AnimationType Type = animationType;
    public AnimationState State = AnimationState.Closed;
    public Action OnOpened;
    public Action OnClosed;

    public bool AnyOpen => State is AnimationState.Opening or AnimationState.Opened;
    public bool Opening => State is AnimationState.Opening;
    public bool Opened => State is AnimationState.Opened;

    public bool AnyClose => State is AnimationState.Closing or AnimationState.Closed;
    public bool Closing => State is AnimationState.Closing;
    public bool Closed => State is AnimationState.Closed;

    #region Open() And Close()
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
    /// 直接跳到完全关闭状态
    /// </summary>
    public void ImmediateClose()
    {
        State = AnimationState.Closed;
        Timer = 0;
    }

    /// <summary>
    /// 直接跳到完全开启状态
    /// </summary>
    public void ImmediateOpen()
    {
        State = AnimationState.Opened;
        Timer = TimerMax;
    }
    #endregion

    /// <summary>
    /// 以高帧率更新计时器，这个方法必须在绘制中调用（而不是UI的更新方法），以实现高帧缓动。
    /// </summary>
    public void UpdateHighFps() => Update(CountRefreshRate.CurrentRefreshRateFactor);

    /// <summary>
    /// 更新计时器
    /// </summary>
    public virtual void Update(float speedFactor = 1f)
    {
        switch (State)
        {
            case AnimationState.Opening:
                if (Type == AnimationType.Easing)
                    Timer += (TimerMax - Timer) / Speed * speedFactor;
                else
                    Timer += Speed * speedFactor;

                if (TimerMax - Timer < TimerMax * 0.0001f)
                {
                    Timer = TimerMax;
                    State = AnimationState.Opened;
                    OnOpened?.Invoke();
                }
                break;
            case AnimationState.Closing:
                if (Type == AnimationType.Easing)
                    Timer -= Timer / Speed * speedFactor;
                else
                    Timer -= Speed * speedFactor;

                if (Timer < TimerMax * 0.0001f)
                {
                    Timer = 0;
                    State = AnimationState.Closed;
                    OnClosed?.Invoke();
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

    public static Vector2 operator *(AnimationTimer timer, Vector2 vector2)
    {
        return vector2 * timer.Schedule;
    }

    public static Vector2 operator *(Vector2 vector2, AnimationTimer timer)
    {
        return vector2 * timer.Schedule;
    }

    public static float operator *(float number, AnimationTimer timer)
    {
        return number * timer.Schedule;
    }

    public static float operator *(AnimationTimer timer, float number)
    {
        return number * timer.Schedule;
    }
}
