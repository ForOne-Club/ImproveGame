using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.BaseViews
{
    /// <summary>
    /// 大背包按钮背景上的效果，需要直接继承此类即可。(继承自 RelativeUIE)
    /// </summary>
    public class TimerView : View
    {
        public AnimationTimer HoverTimer;

        public TimerView()
        {
            HoverTimer = new AnimationTimer(3);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            HoverTimer.OpenAndResetTimer();
            base.MouseOver(evt);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            HoverTimer.CloseAndResetTimer();
            base.MouseOut(evt);
        }

        public override void Update(GameTime gameTime)
        {
            HoverTimer.Update();
            base.Update(gameTime);
        }
    }
}
