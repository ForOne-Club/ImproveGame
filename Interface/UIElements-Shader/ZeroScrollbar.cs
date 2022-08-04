using ImproveGame.Common.Animations;
using Terraria.GameInput;

namespace ImproveGame.Interface.UIElements_Shader
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class ZeroScrollbar : UIElement
    {
        private float _viewPosition; // 滚动条当前位置

        private float _viewSize = 1f; // 显示出来的高度

        private float _maxViewSize = 20f; // 控制元素的高度

        // 用于拖动内滚动条
        private float offsetY;
        public bool dragging;

        public float ViewPosition
        {
            get => _viewPosition;
            set => _viewPosition = MathHelper.Clamp(value, 0f, _maxViewSize - _viewSize);
        }

        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="ViewPosition"/>
        /// </summary>
        public float BufferViewPosition;

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

        public override void Update(GameTime gameTime)
        {
            if (dragging)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();
                Vector2 size = InnerDimensions.Size();

                ViewPosition = (Main.MouseScreen.Y - offsetY - InnerDimensions.Y) / (InnerDimensions.Height - size.Y) * (_maxViewSize - _viewSize);
            }

            if (BufferViewPosition != 0)
            {
                ViewPosition += BufferViewPosition * 0.2f;
                BufferViewPosition *= 0.8f;
                if (MathF.Abs(BufferViewPosition) < 0.001f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 3);
                    BufferViewPosition = 0;
                }
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            if (evt.Target == this)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();

                if (InnerDimensions.Contains(Main.MouseScreen))
                {
                    offsetY = evt.MousePosition.Y - InnerDimensions.Y;
                    dragging = true;
                }
                BufferViewPosition = 0;
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
            Vector2 size = dimension.Size();

            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, size.X / 2, 3, Color.Black, background1);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Position();

            PixelShader.DrawBox(Main.UIScaleMatrix, innerPosition, innerSize, innerSize.X / 2, 0, Color.White, Color.White);
        }
    }
}
