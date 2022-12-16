namespace ImproveGame.Interface.BaseUIEs
{
    /// <summary>
    /// 可用于相对定位的 UIE，当用于可变大小的 UI 更方便计算位置
    /// </summary>
    public class RelativeUIE : UIElement
    {
        public enum RelativeMode { Default, Horizontal, Vertical };
        /// <summary>
        /// 设置 true 横向时不同步与前一个元素的 Top，纵向时不同步 Left<br/>
        /// 在大背包中用于一排 Button 的时候，第一个 Button 前面有一个 Switch
        /// </summary>
        public bool First;
        /// <summary>
        /// 设置 true 启用相对定位，请不要设置同级元素 HAlign, VAlign 的值。<br/>
        /// </summary>
        public bool Relative;
        /// <summary>
        /// 是否自动换行，如果横向超越边界会向下换行，Left 重新开始计算，纵向同理。
        /// </summary>
        public bool AutoLineFeed;
        /// <summary>
        /// 间距
        /// </summary>
        public Vector2 Interval;
        /// <summary>
        /// 相对的模式，横向填充或者纵向填充
        /// </summary>
        public RelativeMode Mode;

        public override void Recalculate()
        {
            // 应该不会通过不了吧。
            if (Relative && Mode != default && Parent.Children is List<UIElement>)
            {
                List<UIElement> uies = Parent.Children as List<UIElement>;
                int index = uies.IndexOf(this);
                // 判断前面有没有元素
                if (uies.IndexInRange(index - 1))
                {
                    UIElement BeforeUIE = uies[index - 1];
                    // 属性 HAlign, VAlign 与 Width, Height 的值有关系。
                    // 要是设置了值就不计算了，否则会出现偏差。
                    // 所以使用这个功能的时候一定要避免设置这两个属性。
                    // if (BeforeUIE.HAlign == 0 && BeforeUIE.VAlign == 0 && HAlign == 0 && VAlign == 0)
                    // {
                    // Left.Precent = BeforeUIE.Left.Precent;
                    // Top.Precent = BeforeUIE.Top.Precent;
                    switch (Mode)
                    {
                        // 横向
                        case RelativeMode.Horizontal:
                            Left.Pixels = BeforeUIE.Left.Pixels + BeforeUIE.Width.Pixels + Interval.X;

                            if (First)
                                Top.Pixels = 0;
                            else
                                Top.Pixels = BeforeUIE.Top.Pixels;

                            if (AutoLineFeed && Left.Pixels + Width.Pixels > Parent.Width.Pixels)
                            {
                                Left.Pixels = 0;
                                Top.Pixels = BeforeUIE.Top.Pixels + BeforeUIE.Height.Pixels + Interval.Y;
                            }
                            break;
                        // 纵向
                        case RelativeMode.Vertical:
                            Top.Pixels = BeforeUIE.Top.Pixels + BeforeUIE.Height.Pixels + Interval.Y;

                            if (First)
                                Left.Pixels = 0;
                            else
                                Left.Pixels = BeforeUIE.Left.Pixels;

                            if (AutoLineFeed && Top.Pixels + Height.Pixels > Parent.Height.Pixels)
                            {
                                Top.Pixels = 0;
                                Left.Pixels = BeforeUIE.Left.Pixels + BeforeUIE.Width.Pixels + Interval.X;
                            }
                            break;
                    }
                    // }
                }
            }
            base.Recalculate();
        }
    }
}
