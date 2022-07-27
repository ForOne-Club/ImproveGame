namespace ImproveGame.Common.Animations
{
    public enum AnimationState
    {
        Initial,
        Open,
        Close,
        OpenComplete,
        CloseComplete
    }
    public enum AnimationMode
    {
        Linear,
        Nonlinear,
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
        public bool IsInitial => State == AnimationState.Initial;
        public bool IsOpen => State == AnimationState.Open || State == AnimationState.OpenComplete;
        public bool IsClose => State == AnimationState.Close || State == AnimationState.CloseComplete;

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
                    break;
                case AnimationState.Close:
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
                    break;
            }
        }
    }
}
