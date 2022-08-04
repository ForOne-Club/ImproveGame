using ImproveGame.Common.Animations;
using Terraria.GameInput;

namespace ImproveGame.Interface.UIElements_Shader
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class ZeroScrollbar : UIElement
    {
        private float viewPosition; // 滚动条当前位置
        private float MaxViewPoisition => maxViewSize - viewSize;

        private float viewSize = 1f; // 显示出来的高度
        private float maxViewSize = 20f; // 控制元素的高度
        private float ViewScale => viewSize / maxViewSize;

        // 用于拖动内滚动条
        private float offsetY;
        public bool dragging;

        public float ViewPosition
        {
            get => viewPosition;
            set => viewPosition = MathHelper.Clamp(value, 0f, MaxViewPoisition);
        }

        private float _bufferViewPosition;
        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="ViewPosition"/>
        /// </summary>
        public float BufferViewPosition
        {
            get => _bufferViewPosition;
            set => _bufferViewPosition = value;
        }

        public ZeroScrollbar()
        {
            Width.Pixels = 20;
            MaxWidth.Pixels = 20;
            SetPadding(5);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (dragging)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();
                //Main.NewText($"ViewPosition: {ViewPosition}  BufferViewPosition: {BufferViewPosition}");
                //Main.NewText($"ViewSize: {viewSize}  MaxViewSize: {maxViewSize}");
                //Main.NewText($"Height: {Height.Pixels}");
                //Main.NewText($"offset.Y: {offsetY}");
                //Main.NewText($"剩余距离: {Main.MouseScreen.Y - InnerDimensions.Y}");
                ViewPosition = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) / (InnerDimensions.Height * (1 - ViewScale)) * MaxViewPoisition;
            }

            if (BufferViewPosition != 0)
            {
                ViewPosition -= BufferViewPosition * 0.2f;
                BufferViewPosition *= 0.8f;
                if (MathF.Abs(BufferViewPosition) < 0.1f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 1);
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
                    dragging = true;
                    offsetY = evt.MousePosition.Y - InnerDimensions.Y - (InnerDimensions.Height * (1 - ViewScale) * (viewPosition / MaxViewPoisition));
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

        public readonly Color background = new(43, 56, 101);
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimension = GetDimensions();
            Vector2 position = dimension.Position();
            Vector2 size = dimension.Size();

            // 滚动条背板
            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, size.X / 2, 3, Color.Black, background);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Size();
            if (MaxViewPoisition != 0)
                innerPosition.Y += innerDimensions.Height * (1 - ViewScale) * (ViewPosition / MaxViewPoisition);
            innerSize.Y *= ViewScale;

            // 滚动条拖动块
            PixelShader.DrawBox(Main.UIScaleMatrix, innerPosition, innerSize, innerSize.X / 2, 0, Color.White, Color.White);
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);

            this.viewSize = viewSize;
            this.maxViewSize = maxViewSize;
        }
    }
}
