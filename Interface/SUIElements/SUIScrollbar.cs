using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using Terraria.GameInput;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class SUIScrollBar : TimerView
    {
        /// <summary>
        /// 滚动条位置
        /// </summary>
        private float barPosition;

        /// <summary>
        /// 滚动条活动范围
        /// </summary>
        private float barMaxPoisition => actualHeight - displayHeight;

        /// <summary>
        /// 展示高度
        /// </summary>
        private float displayHeight = 1f;

        /// <summary>
        /// 所有子元素实际高度
        /// </summary>
        private float actualHeight = 20f;

        /// <summary>
        /// 滚动条高度
        /// </summary>
        public float BarHeight => displayHeight / actualHeight;

        private bool innerHovered;

        /// <summary>
        /// 用于拖动内滚动条
        /// </summary>
        private float offsetY;

        /// <summary>
        /// 操作条拖动中
        /// </summary>
        public bool BarDragging { get; protected set; }

        public bool Visible;

        /// <summary>
        /// 当拖动块填满了整个拖动条（即不能拖动也不需要拖动）时，是否不绘制拖动块
        /// </summary>
        public bool HideInnerIfFilled;

        /// <summary>
        /// 当拖动块填满了整个拖动条（即不能拖动也不需要拖动）时，是否不绘制<b>整个拖动条</b>
        /// </summary>
        public bool HideIfFilled;

        /// <summary> 
        /// 拖动块是否填满了整个拖动条
        /// </summary>
        public bool InnerFilled => barMaxPoisition == 0f;

        public Color InnerBg, InnerBgHover;
        public float RoundMultiplier;

        public float BarPosition
        {
            get => barPosition;
            set => barPosition = MathHelper.Clamp(value, 0f, barMaxPoisition);
        }

        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="BarPosition"/>
        /// </summary>
        public float BarPositionBuffer { get; set; }

        public SUIScrollBar()
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
            InnerRectangle.Y += (BarPosition / barMaxPoisition) * (InnerDimensions.Height * (1 - BarHeight));
            InnerRectangle.Height = InnerDimensions.Height * BarHeight;
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
                if (Math.Abs(BarHeight - 1) > 0.000000001f)
                    BarPosition = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) /
                        (InnerDimensions.Height * (1 - BarHeight)) * barMaxPoisition;
            }

            if (BarPositionBuffer == 0)
            {
                return;
            }

            BarPosition -= BarPositionBuffer * 0.2f;
            BarPositionBuffer *= 0.8f;
            if (!(MathF.Abs(BarPositionBuffer) < 0.1f))
            {
                return;
            }

            BarPosition = MathF.Round(BarPosition, 1);
            BarPositionBuffer = 0;
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
                          (innerDimensions.Height * (1 - BarHeight) * (barPosition / barMaxPoisition));
            }

            BarPositionBuffer = 0;
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
            if (barMaxPoisition != 0)
                innerPosition.Y += innerDimensions.Height * (1 - BarHeight) * (BarPosition / barMaxPoisition);
            innerSize.Y *= BarHeight;

            Color hoverColor = Color.Lerp(InnerBg, InnerBgHover, BarDragging ? 1 : HoverTimer.Schedule);
            var round = new Vector4(MathF.Min(innerSize.X, innerSize.Y) * RoundMultiplier);

            // 滚动条拖动块
            SDFRectangle.NoBorder(innerPosition, innerSize, round, hoverColor);
        }

        /// <summary>
        /// 设置要控制的 UI 的外露大小和内部实际大小
        /// </summary>
        /// <param name="displayHeight">父元素显示高度</param>
        /// <param name="actualHeight">内部元素实际高度</param>
        public void SetView(float displayHeight, float actualHeight)
        {
            displayHeight = MathHelper.Clamp(displayHeight, 0f, actualHeight);
            barPosition = MathHelper.Clamp(barPosition, 0f, actualHeight - displayHeight);

            this.displayHeight = displayHeight;
            this.actualHeight = actualHeight;
        }
    }
}