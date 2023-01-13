namespace ImproveGame.Interface.BaseViews
{
    public abstract class ViewBody : View
    {
        protected ViewBody()
        {
            Width.Percent = 1f;
            Height.Percent = 1f;
            base.Recalculate();
        }

        public abstract bool Display { get; set; }

        /// <summary>
        /// 将元素绘制与事件触发放入顶层
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract bool CanPriority(UIElement target);

        /// <summary>
        /// 是否占用光标，使得不对其下层元素进行 主动类型的事件触发
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract bool CanDisableMouse(UIElement target);
    }
}