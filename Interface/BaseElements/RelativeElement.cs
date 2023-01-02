namespace ImproveGame.Interface.BaseUIEs
{
    /// <summary>
    /// 排列模式，横向排列或者纵向排列。
    /// </summary>
    public enum RelativeMode { Horizontal, Vertical };
    /// <summary>
    /// 相对定位，用于可变大小的 UI 更方便计算位置。
    /// </summary>
    public class RelativeElement : UIElement
    {
        /// <summary>
        /// 设置 true 启用相对定位，请不要设置同级元素 HAlign, VAlign 的值。
        /// </summary>
        public bool Relative;
        /// <summary>
        /// 相对的模式，横向填充或者纵向填充
        /// </summary>
        public RelativeMode Layout;
        /// <summary>
        /// 间距
        /// </summary>
        public Vector2 Spacing;
        /// <summary>
        /// 越界换行
        /// </summary>
        public bool Wrap;
        /// <summary>
        /// 设置 true 横向时不同步与前一个元素的 Top，纵向时不同步 Left<br/>
        /// 在大背包中用于一排 Button 的时候，第一个 Button 前面有一个 Switch
        /// </summary>
        public bool First;

        public override void Recalculate()
        {
            if (Relative && Parent.Children is List<UIElement>)
            {
                List<UIElement> uies = Parent.Children as List<UIElement>;
                int index = uies.IndexOf(this);
                // 判断前面有没有元素
                if (index > 0 && uies[index - 1] is RelativeElement)
                {
                    RelativeElement Before = uies[index - 1] as RelativeElement;
                    Vector2 BeforeSize = Before.GetDimensions().Size();
                    Vector2 ParentSize = Parent.GetInnerDimensions().Size();
                    switch (Layout)
                    {
                        // 横向
                        case RelativeMode.Horizontal:
                            Left.Pixels = Before.Left.Pixels + BeforeSize.X + Spacing.X;

                            Top.Pixels = First ? 0 : Before.Top.Pixels;

                            if (Wrap && Left.Pixels + Width.Pixels > ParentSize.X)
                            {
                                Left.Pixels = 0;
                                Top.Pixels = Before.Top.Pixels + BeforeSize.Y + Spacing.Y;
                            }
                            break;
                        // 纵向
                        case RelativeMode.Vertical:
                            Top.Pixels = Before.Top.Pixels + BeforeSize.Y + Spacing.Y;

                            Left.Pixels = First ? 0 : Before.Left.Pixels;

                            if (Wrap && Top.Pixels + Height.Pixels > ParentSize.Y)
                            {
                                Top.Pixels = 0;
                                Left.Pixels = Before.Left.Pixels + BeforeSize.X + Spacing.X;
                            }
                            break;
                    }
                }
            }
            base.Recalculate();
        }

        // 下面这些方法只是为了更方便的使用 UIElement 这个类。
        // 原来是写到了 UIElementHelper ，还是直接写一个基类舒服点。
        // 设置
        public void SetPosPixels(float left, float top)
        {
            Left.Pixels = left;
            Top.Pixels = top;
        }

        public void SetPosPixels(Vector2 size)
        {
            Left.Pixels = size.X;
            Top.Pixels = size.Y;
        }

        public void SetInnerPixels(float width, float height)
        {
            Width.Pixels = width + PaddingLeft + PaddingRight;
            Height.Pixels = height + PaddingTop + PaddingBottom;
        }

        public void SetInnerPixels(Vector2 size)
        {
            Width.Pixels = size.X + PaddingLeft + PaddingRight;
            Height.Pixels = size.Y + PaddingTop + PaddingBottom;
        }

        // 获取
        public float RightPixels()
        {
            return Left.Pixels + Width.Pixels + MarginLeft + MarginRight;
        }

        public float BottomPixels()
        {
            return Top.Pixels + Height.Pixels + MarginTop + MarginBottom;
        }

        public Vector2 GetPosPixel()
        {
            return new Vector2(Left.Pixels + Top.Pixels);
        }
    }
}
