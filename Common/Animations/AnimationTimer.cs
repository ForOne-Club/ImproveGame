namespace ImproveGame.Common.Animations;

public enum AnimationState // 动画当前的状态
{
    Initial, Open, Close, OpenComplete, CloseComplete
}

public enum AnimationType
{
    Linear, // 线性变化 (就一种类型, 还是一个大分类)
    Nonlinear, // 缓动动画 (最简单的非线性动画). 如果能学到其他的非线性动画, 我就都加进来.
    Bezier, // 贝塞尔回弹：一
}

// 动画计时器
public class AnimationTimer
{
    // 缓动系数 / 增量
    public float Speed; // √
    public float Timer;
    public float TimerMax; // √
    public float Schedule
    {
        get
        {
            float schedule = Timer / TimerMax;
            return AnimationType switch
            {
                AnimationType.Bezier => Bezier(schedule),
                _ => schedule,
            };
        }
    }

    public AnimationType AnimationType;
    public AnimationState State;
    public Func<Vector2> Center;
    public Action OnOpenComplete;
    public Action OnCloseComplete;

    public bool InOpen => State == AnimationState.Open;
    public bool InClose => State == AnimationState.Close;
    public bool InOpenComplete => State == AnimationState.OpenComplete;
    public bool InCloseComplete => State == AnimationState.CloseComplete;
    public bool InInitial => State == AnimationState.Initial;
    public bool AnyOpen => State == AnimationState.Open || State == AnimationState.OpenComplete;
    public bool AnyClose => State == AnimationState.Close || State == AnimationState.CloseComplete;

    public static float Bezier(float t)
    {
        float a = 0f;
        float b = -0.5f;
        float c = 1.5f;
        float d = 1f;
        float aa = a + (b - a) * t;
        float bb = b + (c - b) * t;
        float cc = c + (d - c) * t;

        float aaa = aa + (bb - aa) * t;
        float bbb = bb + (cc - bb) * t;
        return aaa + (bbb - aaa) * t;
    }

    public AnimationTimer(float speed = 5f, AnimationType animationType = AnimationType.Nonlinear, float timerMax = 100f)
    {
        Speed = speed;
        TimerMax = timerMax;
        AnimationType = animationType;
    }

    public Color Lerp(Color color1, Color color2)
    {
        return Color.Lerp(color1, color2, Schedule);
    }

    public Vector2 Lerp(Vector2 vector21, Vector2 vector22)
    {
        return new Vector2(Lerp(vector21.X, vector22.X),
            Lerp(vector21.Y, vector22.Y));
    }

    public float Lerp(float value1, float value2)
    {
        return value1 + (value2 - value1) * Schedule;
    }

    public void Open()
    {
        State = AnimationState.Open;
        Timer = 0;
    }

    public void TryOpen()
    {
        if (!AnyOpen)
        {
            State = AnimationState.Open;
        }
    }

    public void Close()
    {
        State = AnimationState.Close;
        Timer = TimerMax;
    }

    public void TryClose()
    {
        if (!AnyClose)
        {
            State = AnimationState.Close;
        }
    }

    public void Update()
    {
        switch (State)
        {
            case AnimationState.Open:
                Update_Open();
                break;
            case AnimationState.Close:
                Update_Close();
                break;
        }
    }

    private void Update_Open()
    {
        if (AnimationType == AnimationType.Nonlinear)
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
            State = AnimationState.OpenComplete;
            OnOpenComplete?.Invoke();
        }
    }

    private void Update_Close()
    {
        if (AnimationType == AnimationType.Nonlinear)
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
            State = AnimationState.CloseComplete;
            OnCloseComplete?.Invoke();
        }
    }
}
