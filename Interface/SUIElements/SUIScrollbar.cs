using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using Terraria.GameInput;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class SUIScrollbar : UIElement
    {
        private float viewPosition; // 滚动条当前位置
        private float MaxViewPoisition => maxViewSize - viewSize;

        private float viewSize = 1f; // 显示出来的高度
        private float maxViewSize = 20f; // 控制元素的高度
        private float ViewScale => viewSize / maxViewSize;
        private bool innerHovered;

        // 用于拖动内滚动条
        private float offsetY;
        public bool dragging;

        public bool Visible;

        public AnimationTimer HoverTimer = new(3);

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

        public SUIScrollbar()
        {
            Visible = true;
            Width.Pixels = 20;
            MaxWidth.Pixels = 20;
            SetPadding(5);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            CalculatedStyle InnerDimensions = GetInnerDimensions();
            CalculatedStyle InnerRectangle = InnerDimensions;
            InnerRectangle.Y += (ViewPosition / MaxViewPoisition) * (InnerDimensions.Height * (1 - ViewScale));
            InnerRectangle.Height = InnerDimensions.Height * ViewScale;
            if (InnerRectangle.Contains(Main.MouseScreen))
            {
                if (!innerHovered)
                {
                    innerHovered = true;
                    InnerMouseOver();
                }
            }
            else
            {
                if (innerHovered)
                {
                    InnerMouseOut();
                }
                innerHovered = false;
            }
            HoverTimer.Update();
            base.Update(gameTime);

            if (dragging)
            {
                if (ViewScale != 1)
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

        public virtual void InnerMouseOver()
        {
            HoverTimer.Open();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public virtual void InnerMouseOut()
        {
            HoverTimer.Close();
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);

            if (!Visible)
                return;

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
            if (!Visible)
                return;
            dragging = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (!Visible)
                return;
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
        }

        public readonly Color hoveredColor = new(220, 220, 220);
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            CalculatedStyle dimension = GetDimensions();
            Vector2 position = dimension.Position();
            Vector2 size = dimension.Size();

            // 滚动条背板
            PixelShader.DrawRoundRect(position, size, size.X / 2, UIColor.ScrollBarBackground, 3, UIColor.PanelBorder);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Size();
            if (MaxViewPoisition != 0)
                innerPosition.Y += innerDimensions.Height * (1 - ViewScale) * (ViewPosition / MaxViewPoisition);
            innerSize.Y *= ViewScale;

            Color hoverColor = Color.Lerp(hoveredColor, Color.White, dragging ? 1 : HoverTimer.Schedule);

            // 滚动条拖动块
            PixelShader.DrawRoundRect(innerPosition, innerSize, innerSize.X / 2, hoverColor);
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
