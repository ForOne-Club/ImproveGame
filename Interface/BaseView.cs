namespace ImproveGame.Interface
{
    public enum MouseType : byte { Left, Right, Middle }

    public struct Spacing
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float HorizontalSize => Left + Right;
        public float VerticalSize => Top + Bottom;

        public void Set(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public void Set(float horizontal, float vertical)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        public void Set(float size)
        {
            Left = Top = Right = Bottom = size;
        }
    }

    public class BaseView
    {
        public BaseView Parent;
        public readonly List<BaseView> Children;

        public bool MouseHover;

        private float _width;
        private float _innerWidth;
        private float _outerWidth;
        private float _widthPercent;
        public Spacing Padding;
        public Spacing Margin;

        public float InnerWidth => _innerWidth;
        public float OuterWidth => _outerWidth;

        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                _innerWidth = _width + _widthPercent * Parent?._width ?? Main.screenWidth + Padding.HorizontalSize;
                _outerWidth = _innerWidth + Margin.HorizontalSize;
            }
        }

        public BaseView()
        {
            UIElement uie = new UIElement();
            uie.Recalculate();
            Children = new List<BaseView>();
        }

        public virtual void Update(Vector2 mouse)
        {
            foreach (BaseView child in Children)
            {
                child.Update(mouse);
            }
        }

        public void Append(BaseView child)
        {
            if (Children.Contains(child))
            {
                return;
            }

            Children.Add(child);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            DrawSelf(sb);
            DrawChildren(sb);
        }

        public virtual void DrawSelf(SpriteBatch sb)
        {
        }

        public virtual void DrawChildren(SpriteBatch sb)
        {
            foreach (BaseView child in Children)
            {
                child.Draw(sb);
            }
        }

        // 鼠标进入
        public virtual void MouseEnter(BaseView target, Vector2 mouse)
        {
            MouseHover = true;
        }

        // 鼠标离开
        public virtual void MouseLeave(BaseView target, Vector2 mouse)
        {
            MouseHover = false;
        }

        // 鼠标按下
        public virtual void MouseDown(BaseView target, Vector2 mouse, MouseType mouseType)
        {
            switch (mouseType)
            {
                case MouseType.Left:
                    MouseDown_Left(target, mouse);
                    break;
                case MouseType.Right:
                    MouseDown_Right(target, mouse);
                    break;
                case MouseType.Middle:
                    MouseDown_Middle(target, mouse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mouseType), mouseType, null);
            }
        }

        // 鼠标松开
        public virtual void MouseUp(BaseView target, Vector2 mouse, MouseType mouseType)
        {
            switch (mouseType)
            {
                case MouseType.Left:
                    MouseDown_Left(target, mouse);
                    break;
                case MouseType.Right:
                    MouseDown_Right(target, mouse);
                    break;
                case MouseType.Middle:
                    MouseDown_Middle(target, mouse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mouseType), mouseType, null);
            }
        }

        // 鼠标点击
        public virtual void Click(BaseView target, Vector2 mouse, MouseType mouseType)
        {
            switch (mouseType)
            {
                case MouseType.Left:
                    MouseClick_Left(target, mouse);
                    break;
                case MouseType.Right:
                    MouseClick_Right(target, mouse);
                    break;
                case MouseType.Middle:
                    MouseClick_Middle(target, mouse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mouseType), mouseType, null);
            }
        }

        public virtual void MouseDown_Left(BaseView target, Vector2 mouse)
        {
            Parent.MouseDown_Left(target, mouse);
        }

        public virtual void MouseDown_Right(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseDown_Middle(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseUp_Left(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseUp_Right(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseUp_Middle(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseClick_Left(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseClick_Right(BaseView target, Vector2 mouse)
        {
        }

        public virtual void MouseClick_Middle(BaseView target, Vector2 mouse)
        {
        }
    }
}