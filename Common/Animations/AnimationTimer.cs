namespace ImproveGame.Common.Animations
{
    public enum AnimationState // 动画当前的状态
    {
        Initial, Open, Close, OpenComplete, CloseComplete
    }

    public enum AnimationMode
    {
        Linear, // 线性变化 (就一种类型, 还是一个大分类)
        Nonlinear, // 缓动动画 (最简单的非线性动画). 如果能学到其他的非线性动画, 我就都加进来.
        BezierOne, // 贝塞尔回弹：一
    }

    // 动画计时器
    public class AnimationTimer
    {
        // 缓动系数/增量
        public float Speed; // √
        public float Timer;
        public float TimerMax; // √
        public float Schedule
        {
            get
            {
                float s = Timer / TimerMax;
                return Mode switch
                {
                    AnimationMode.BezierOne => Bezier(s).Y,
                    _ => s,
                };
            }
        }
        public AnimationMode Mode; // √
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

        public static readonly Vector2 a = new Vector2(0);
        public static readonly Vector2 b = new Vector2(0f, 0.5f);
        public static readonly Vector2 c = new Vector2(0.75f, 2f);
        public static readonly Vector2 d = new Vector2(1);

        public static Vector2 Bezier(float t)
        {
            Vector2 aa = a + (b - a) * t;
            Vector2 bb = b + (c - b) * t;
            Vector2 cc = c + (d - c) * t;

            Vector2 aaa = aa + (bb - aa) * t;
            Vector2 bbb = bb + (cc - bb) * t;
            return aaa + (bbb - aaa) * t;
        }

        public AnimationTimer(float Speed = 5f, AnimationMode mode = AnimationMode.Nonlinear, float TimerMax = 100f)
        {
            this.Speed = Speed;
            this.TimerMax = TimerMax;
            Mode = mode;
        }

        public void Open()
        {
            Timer = 0;
            State = AnimationState.Open;
        }

        public void TryOpen()
        {
            if (!AnyOpen)
                Open();
        }

        public void Close()
        {
            Timer = TimerMax;
            State = AnimationState.Close;
        }

        public void TryClose()
        {
            if (!AnyClose)
                Close();
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
            if (Mode == AnimationMode.Nonlinear)
            {
                Timer += (TimerMax - Timer) / Speed;
            }
            else
            {
                Timer += Speed;
            }
            if (TimerMax - Timer < 1f)
            {
                Timer = TimerMax;
                State = AnimationState.OpenComplete;
                OnOpenComplete?.Invoke();
            }
        }

        private void Update_Close()
        {
            if (Mode == AnimationMode.Nonlinear)
            {
                Timer -= Timer / Speed;
            }
            else
            {
                Timer -= Speed;
            }
            if (Timer < 1f)
            {
                Timer = 0;
                State = AnimationState.CloseComplete;
                OnCloseComplete?.Invoke();
            }
        }
    }
}
