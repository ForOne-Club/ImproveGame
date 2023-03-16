global using ImproveGame.Interface.BaseViews;
using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.BaseViews
{
    /// <summary>
    /// 排列模式，横向排列或者纵向排列。
    /// </summary>
    public enum RelativeMode { Disabled, Horizontal, Vertical };
    /// <summary>
    /// 相对定位，用于可变大小的 UI 更方便计算位置。
    /// </summary>
    public class View : UIElement
    {
        /// <summary>
        /// 相对的模式，横向填充或者纵向填充
        /// </summary>
        public RelativeMode Relative;
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

        /// <summary>
        /// 拖动忽略
        /// </summary>
        public bool DragIgnore;

        /// <summary>
        /// 不透明度 [0,1]
        /// </summary>
        public readonly Opacity Opacity;

        public bool KeepPressed;
        public Vector4 Rounded;
        public float Border;
        public Color BgColor, BorderColor;

        public View()
        {
            Opacity = new Opacity(this);
        }

        public override void Recalculate()
        {
            Opacity.Recalculate();

            if (Relative != RelativeMode.Disabled && Parent is View)
            {
                View Parent = this.Parent as View;
                List<UIElement> uies = Parent.Children as List<UIElement>;
                int index = uies.IndexOf(this);
                // 判断前面有没有元素
                if (index > 0 && uies[index - 1] is View)
                {
                    View Before = uies[index - 1] as View;
                    Vector2 BeforeSize = Before.GetDimensionsSize();
                    Vector2 ParentSize = Parent.GetInnerDimensionsSize();
                    switch (Relative)
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

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            KeepPressed = true;
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            KeepPressed = false;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            if (Border > 0 && (BgColor != Color.Transparent || BorderColor != Color.Transparent))
            {
                SDFRectangle.HasBorder(pos, size, Rounded, BgColor * Opacity.Value, Border, BorderColor * Opacity.Value);
            }
            else if (BgColor != Color.Transparent)
            {
                SDFRectangle.NoBorder(pos, size, Rounded, BgColor * Opacity.Value);
            }

            base.DrawSelf(spriteBatch);
        }

        // 加入
        public void Join(UIElement parent)
        {
            parent.Append(this);
        }

        // 下面这些方法只是为了更方便的使用 UIElement 这个类。
        // 原来是写到了 UIElementHelper ，但是直接写这里比较舒服。
        public View SetPosPixels(float left, float top)
        {
            Left.Pixels = left;
            Top.Pixels = top;
            return this;
        }

        public View SetPosPixels(Vector2 size)
        {
            Left.Pixels = size.X;
            Top.Pixels = size.Y;
            return this;
        }

        public View SetInnerPixels(float width, float height)
        {
            Width.Pixels = width + PaddingLeft + PaddingRight;
            Height.Pixels = height + PaddingTop + PaddingBottom;
            return this;
        }

        public View SetInnerPixels(Vector2 size)
        {
            Width.Pixels = size.X + PaddingLeft + PaddingRight;
            Height.Pixels = size.Y + PaddingTop + PaddingBottom;
            return this;
        }

        public View SetInnerPixels(float size)
        {
            Width.Pixels = size + PaddingLeft + PaddingRight;
            Height.Pixels = size + PaddingTop + PaddingBottom;
            return this;
        }

        public View SetSizePixels(float width, float height)
        {
            Width.Pixels = width;
            Height.Pixels = height;
            return this;
        }

        public View SetSizePixels(Vector2 size)
        {
            Width.Pixels = size.X;
            Height.Pixels = size.Y;
            return this;
        }

        public View SetPadding(float left, float top, float right, float bottom)
        {
            PaddingLeft = left;
            PaddingTop = top;
            PaddingRight = right;
            PaddingBottom = bottom;
            return this;
        }

        public View SetPadding(float h, float v)
        {
            PaddingLeft = PaddingRight = h;
            PaddingTop = PaddingBottom = v;
            return this;
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
            return new Vector2(Left.Pixels, Top.Pixels);
        }

        public Vector2 GetDimensionsSize()
        {
            CalculatedStyle dimensions = GetDimensions();
            return new Vector2(dimensions.Width, dimensions.Height);
        }

        public Vector2 GetInnerDimensionsSize()
        {
            CalculatedStyle dimensions = GetInnerDimensions();
            return new Vector2(dimensions.Width, dimensions.Height);
        }
    }
}
