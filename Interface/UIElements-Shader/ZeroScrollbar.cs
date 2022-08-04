using ImproveGame.Common.Animations;
using Terraria.GameInput;

namespace ImproveGame.Interface.UIElements_Shader
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class ZeroScrollbar : UIElement
    {
        private float _viewPosition;

        private float _viewSize = 1f;

        private float _maxViewSize = 20f;

        private float offsetY;
        public bool dragging;

        public float ViewPosition
        {
            get
            {
                return _viewPosition;
            }
            set
            {
                _viewPosition = MathHelper.Clamp(value, 0f, _maxViewSize - _viewSize);
            }
        }

        public bool CanScroll => _maxViewSize != _viewSize;

        // 后
        private float ScrollWheelValue;

        public ZeroScrollbar()
        {
            Width.Pixels = 20;
            MaxWidth.Pixels = 20;
            SetPadding(5);
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            _viewPosition = MathHelper.Clamp(_viewPosition, 0f, maxViewSize - viewSize);
            _viewSize = viewSize;
            _maxViewSize = maxViewSize;
        }

        public float GetValue()
        {
            return _viewPosition;
        }

        public void SetViewPosition(int ScrollWheelValue)
        {
            this.ScrollWheelValue -= ScrollWheelValue;
        }

        public override void Update(GameTime gameTime)
        {
            if (dragging)
            {
                InnerBox(out _, out Vector2 size);
                ViewPosition = (Main.MouseScreen.Y - offsetY - GetInnerDimensions().Y) / (this.HeightInside() - size.Y) * (_maxViewSize - _viewSize);
            }

            if (ScrollWheelValue != 0)
            {
                ViewPosition += ScrollWheelValue * 0.2f;
                ScrollWheelValue *= 0.8f;
                if (MathF.Abs(ScrollWheelValue) < 0.001f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 3);
                    ScrollWheelValue = 0;
                }
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            if (evt.Target == this)
            {
                InnerBox(out Vector2 position, out _);
                InnerBox(out Rectangle rectangle);
                if (rectangle.Contains(Main.MouseScreen.ToPoint()))
                {
                    offsetY = evt.MousePosition.Y - position.Y;
                    dragging = true;
                }
                ScrollWheelValue = 0;
            }
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            dragging = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
        }

        public readonly Color background1 = new(43, 56, 101);
        public readonly Color borderColor2 = new(93, 88, 93);
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimension = GetDimensions();
            Vector2 position = dimension.Position();
            Vector2 size = this.GetSize();

            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, size.X / 2, 3, Color.Black, background1);

            InnerBox(out Vector2 innerPosition, out Vector2 innerSize);
            PixelShader.DrawBox(Main.UIScaleMatrix, innerPosition, innerSize, innerSize.X / 2, 0, Color.White, Color.White);
        }

        public void InnerBox(out Vector2 innerPosition, out Vector2 innerSize)
        {
            CalculatedStyle innerDimension = GetInnerDimensions();
            innerPosition = innerDimension.Position();
            innerSize = this.GetSizeInside();
            innerSize.Y *= _viewSize / _maxViewSize;
            innerPosition.Y += (_viewPosition / (_maxViewSize - _viewSize)) * (this.HeightInside() - innerSize.Y);
        }

        public void InnerBox(out Rectangle rectangle)
        {
            InnerBox(out Vector2 innerPosition, out Vector2 innerSize);
            rectangle = new((int)innerPosition.X, (int)innerPosition.Y, (int)innerSize.X, (int)innerSize.Y);
        }
    }
}
