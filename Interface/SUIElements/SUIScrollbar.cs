using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using Terraria.GameInput;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class SUIScrollbar : TimerView
    {
        private float viewPosition; // 滚动条当前位置
        private float MaxViewPoisition => exposedSize - internalSize;

        /// <summary>
        /// 内部大小
        /// </summary>
        private float internalSize = 1f;
        /// <summary>
        /// 外露大小
        /// </summary>
        private float exposedSize = 20f;
        private float ViewScale => internalSize / exposedSize;
        private bool innerHovered;

        // 用于拖动内滚动条
        private float offsetY;

        /// <summary>
        /// 操作条拖动中
        /// </summary>
        public bool BarDragging { get; private set; }

        public bool Visible;

        /// <summary> 当拖动块填满了整个拖动条（即不能拖动也不需要拖动）时，是否不绘制拖动块 </summary>
        public bool HideInnerIfFilled;

        /// <summary> 当拖动块填满了整个拖动条（即不能拖动也不需要拖动）时，是否不绘制<b>整个拖动条</b> </summary>
        public bool HideIfFilled;

        /// <summary> 
        /// 拖动块是否填满了整个拖动条
        /// </summary>
        public bool InnerFilled => MaxViewPoisition == 0f;

        public Color InnerBg, InnerBgHover;
        public float RoundMultiplier;

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
            Width.Pixels = 20f;
            SetPadding(5);

            BgColor = UIColor.ScrollBarBg;
            Border = UIColor.ItemSlotBorderSize;
            BorderColor = UIColor.ScrollBarBorder;
            InnerBg = UIColor.ScrollBarInnerBg;
            InnerBgHover = UIColor.ScrollBarInnerBgHover;
            RoundMultiplier = UIColor.ScrollBarRoundMultiplier;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            CalculatedStyle InnerDimensions = GetInnerDimensions();
            CalculatedStyle InnerRectangle = InnerDimensions;
            InnerRectangle.Y += (ViewPosition / MaxViewPoisition) * (InnerDimensions.Height * (1 - ViewScale));
            InnerRectangle.Height = InnerDimensions.Height * ViewScale;
            if (IsMouseHovering)
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

            base.Update(gameTime);

            if (BarDragging)
            {
                if (Math.Abs(ViewScale - 1) > 0.000000001f)
                    ViewPosition = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) /
                        (InnerDimensions.Height * (1 - ViewScale)) * MaxViewPoisition;
            }

            if (BufferViewPosition == 0)
            {
                return;
            }

            ViewPosition -= BufferViewPosition * 0.2f;
            BufferViewPosition *= 0.8f;
            if (!(MathF.Abs(BufferViewPosition) < 0.1f))
            {
                return;
            }

            ViewPosition = MathF.Round(ViewPosition, 1);
            BufferViewPosition = 0;
        }

        public virtual void InnerMouseOver()
        {
            if ((!HideIfFilled && !HideInnerIfFilled) || !InnerFilled)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        public virtual void InnerMouseOut()
        {
            if ((HideIfFilled || HideInnerIfFilled) && InnerFilled)
                return;

        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            if (!Visible)
                return;

            if (evt.Target != this)
            {
                return;
            }

            CalculatedStyle innerDimensions = GetInnerDimensions();

            if (IsMouseHovering)
            {
                BarDragging = true;
                offsetY = evt.MousePosition.Y - innerDimensions.Y -
                          (innerDimensions.Height * (1 - ViewScale) * (viewPosition / MaxViewPoisition));
            }

            BufferViewPosition = 0;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);

            if (Visible)
            {
                BarDragging = false;
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            if (Visible)
            {
                PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || (HideIfFilled && InnerFilled))
                return;
            Vector2 size = GetDimensions().Size();
            Rounded = new Vector4(MathF.Min(size.X, size.Y) * RoundMultiplier);
            base.DrawSelf(spriteBatch);

            if (HideInnerIfFilled && InnerFilled)
                return;

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Size();
            if (MaxViewPoisition != 0)
                innerPosition.Y += innerDimensions.Height * (1 - ViewScale) * (ViewPosition / MaxViewPoisition);
            innerSize.Y *= ViewScale;

            Color hoverColor = Color.Lerp(InnerBg, InnerBgHover, BarDragging ? 1 : HoverTimer.Schedule);
            var round = new Vector4(MathF.Min(innerSize.X, innerSize.Y) * RoundMultiplier);

            // 滚动条拖动块
            SDFRectangle.NoBorder(innerPosition, innerSize, round, hoverColor);
        }

        /// <summary>
        /// 设置要控制的 UI 的外露大小和内部实际大小
        /// </summary>
        /// <param name="internalSize">内部大小</param>
        /// <param name="exposedSize">外露大小</param>
        public void SetView(float internalSize, float exposedSize)
        {
            internalSize = MathHelper.Clamp(internalSize, 0f, exposedSize);
            viewPosition = MathHelper.Clamp(viewPosition, 0f, exposedSize - internalSize);

            this.internalSize = internalSize;
            this.exposedSize = exposedSize;
        }
    }
}