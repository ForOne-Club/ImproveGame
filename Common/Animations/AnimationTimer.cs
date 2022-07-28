namespace ImproveGame.Common.Animations
{
    public enum AnimationState // 动画当前的状态
    {
        Initial,
        Open,
        Close,
        OpenComplete,
        CloseComplete
    }
    public enum AnimationMode
    {
        Linear, // 线性变化 (就一种类型, 还是一个大分类)
        Nonlinear, // 缓动动画 (最简单的非线性动画). 如果能学到其他的非线性动画, 我就都加进来.
    }

    // 动画计时器
    public class AnimationTimer
    {
        public float Speed; // √
        public float Timer;
        public float TimerMax; // √
        public float Schedule => Timer / TimerMax;
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

        public void Close()
        {
            Timer = TimerMax;
            State = AnimationState.Close;
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
            if (Mode == AnimationMode.Linear)
            {
                Timer += Speed;
            }
            else if (Mode == AnimationMode.Nonlinear)
            {
                Timer += (TimerMax - Timer) / Speed;
            }
            if (TimerMax - Timer < 1f)
            {
                Timer = 100;
                State = AnimationState.OpenComplete;
                OnOpenComplete?.Invoke();
            }
        }

        private void Update_Close()
        {
            if (Mode == AnimationMode.Linear)
            {
                Timer -= Speed;
            }
            else if (Mode == AnimationMode.Nonlinear)
            {
                Timer -= Timer / Speed;
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
