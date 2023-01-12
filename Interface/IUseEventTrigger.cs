namespace ImproveGame.Interface
{
    public interface IUseEventTrigger
    {
        /// <summary>
        /// 将元素绘制与事件触发放入顶层
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool ToPrimary(UIElement target);
        
        /// <summary>
        /// 是否占用光标，使得不对其下层元素进行 主动类型的事件触发
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CanOccupyCursor(UIElement target);
    }
}