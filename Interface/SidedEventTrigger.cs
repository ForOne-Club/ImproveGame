using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface
{
    /// <summary>
    /// 一个特殊的 EventTrigger，用于所有不可共存的侧栏 UI，一般来说，UI 会在打开时，将其他 UI 关闭，且会有动画。
    /// 使用 RegisterViewBody 来注册侧栏 UI。使用 ToggleViewBody 以开关已注册的 UI。
    /// IsClosed 与 IsOpened 用于判断侧栏 UI 是否已关闭或已打开。
    /// </summary>
    public class SidedEventTrigger : EventTrigger
    {
        private record SideUIData
        {
            internal ViewBody ViewBody;
            internal AnimationTimer AnimationTimer;
            internal ISidedView AsSidedView => ViewBody as ISidedView;
        }

        public static bool ViewBodyIs(ViewBody body) => UISystem.Instance.SidedEventTrigger.ViewBody == body;

        private static readonly List<SideUIData> UIPool = new();

        public SidedEventTrigger() : base("Radial Hotbars", "Left Side GUI")
        {
        }

        public static void Clear() => UIPool.Clear();

        public static void RegisterViewBody(ViewBody viewBody)
        {
            if (viewBody is not ISidedView)
                throw new Exception($"ViewBody {viewBody.GetType().Name} is not ILeftSideView!");

            viewBody.Activate();

            var data = new SideUIData
            {
                ViewBody = viewBody,
                AnimationTimer = new AnimationTimer
                {
                    Timer = 0,
                    State = AnimationState.CompleteClose
                }
            };
            
            // 设置初始（关闭时）位置
            data.AsSidedView.OnSwapSlide(0f);
            data.ViewBody.Recalculate();
            
            UIPool.Add(data);
        }

        /// <summary> 指定 ViewBody 是否处于完全关闭状态或正在关闭 </summary>
        public static bool IsClosed(ViewBody viewBody)
        {
            if (viewBody is null)
                return true;
            var existingBody = UIPool.Find(d => d.ViewBody == viewBody);
            if (existingBody is null)
                throw new Exception($"ViewBody {viewBody.GetType().Name} is not registered!");
            return existingBody.AnimationTimer.AnyClose;
        }

        /// <summary> 指定 ViewBody 是否处于完全开启状态或正在开启 </summary>
        public static bool IsOpened(ViewBody viewBody)
        {
            if (viewBody is null)
                return false;
            var existingBody = UIPool.Find(d => d.ViewBody == viewBody);
            if (existingBody is null)
                throw new Exception($"ViewBody {viewBody.GetType().Name} is not registered!");
            return existingBody.AnimationTimer.AnyOpen;
        }

        /// <summary>
        /// 开/关某个侧栏 ViewBody
        /// </summary>
        /// <param name="viewBody">执行操作的 ViewBody</param>
        /// <returns>是否进行开启操作</returns>
        /// <exception cref="Exception">此 ViewBody 没有被注册</exception>
        public static void ToggleViewBody(ViewBody viewBody)
        {
            UISystem.Instance.SidedEventTrigger.SetCarrier(null);

            var existingBody = UIPool.Find(d => d.ViewBody == viewBody);
            if (existingBody is null)
                throw new Exception($"ViewBody {viewBody.GetType().Name} is not registered!");

            // 切换状态
            if (existingBody.AnimationTimer.AnyClose)
                existingBody.AnimationTimer.State = AnimationState.Opening;
            else if (existingBody.AnimationTimer.AnyOpen)
                existingBody.AnimationTimer.State = AnimationState.Closing;

            // 是否要进行开启操作
            bool isOperationOpen = existingBody.AnimationTimer.State is AnimationState.Opening;

            // 对进行操作的 ViewBody 运行开关代码
            if (isOperationOpen)
                existingBody.AsSidedView.Open();
            else
                existingBody.AsSidedView.Close();

            // 如果是开启操作，则将所有其他正在开启或已经开启的 ViewBody 转为关闭状态
            if (!isOperationOpen)
            {
                return;
            }

            foreach (var uiData in UIPool.Where(uiData => uiData != existingBody && uiData.AnimationTimer.AnyOpen))
            {
                uiData.AnimationTimer.State = AnimationState.Closing;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            ViewBody = null;

            foreach (var uiData in UIPool.Where(uiData => uiData.AnimationTimer.AnyOpen))
            {
                if (!uiData.AsSidedView.ForceCloseCondition())
                    continue;
                uiData.AnimationTimer.State = AnimationState.Closing;
                uiData.AsSidedView.Close();
            }

            foreach (var uiData in UIPool) {
                uiData.AnimationTimer.Update();
                if (uiData.AnimationTimer.State is AnimationState.Opening or AnimationState.Closing)
                {
                    uiData.AsSidedView.OnSwapSlide(uiData.AnimationTimer.Schedule);
                    uiData.ViewBody.Recalculate();
                }
                if (uiData.AnimationTimer.CompleteOpen)
                {
                    ViewBody = uiData.ViewBody;
                }
            }

            base.Update(gameTime);
        }

        protected override bool Draw(bool drawToGame = true)
        {
            foreach (var uiData in UIPool.Where(uiData => !uiData.AnimationTimer.CompleteClose && !ViewBodyIs(uiData.ViewBody))) {
                uiData.ViewBody?.Draw(Main.spriteBatch);
            }

            return base.Draw();
        }
    }
}