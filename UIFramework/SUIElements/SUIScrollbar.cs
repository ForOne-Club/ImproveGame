using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using Terraria.GameInput;

namespace ImproveGame.UIFramework.SUIElements
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class SUIScrollBar : TimerView
    {
        /// <summary>
        /// 展示高度
        /// </summary>
        private float displayHeight = 1f;

        /// <summary>
        /// 所有子元素实际高度
        /// </summary>
        private float actualHeight = 20f;

        /// <summary>
        /// 滚动条位置
        /// </summary>
        private float barTop;

        /// <summary>
        /// 滚动条活动范围
        /// </summary>
        private float BarMaxTop => actualHeight - displayHeight;

        /// <summary>
        /// 滚动条高度
        /// </summary>
        public float BarHeightPercent
        {
            get
            {
                float innerWidth = GetInnerDimensions().Width;
                float innerHeight = GetInnerDimensions().Height;
                return Math.Max(Math.Min(innerWidth, innerHeight) * 2f / innerHeight, displayHeight / actualHeight);
            }
        }

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
        public bool InnerFilled => BarMaxTop == 0f;

        public Color InnerBg, InnerBgHover;
        public float RoundMultiplier;

        /// <summary>
        /// 滚动条位置
        /// </summary>
        public float BarTop
        {
            get => barTop;
            set => barTop = MathHelper.Clamp(value, 0f, BarMaxTop);
        }

        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="BarTop"/>
        /// </summary>
        public float BarTopBuffer { get; set; }

        public SUIScrollBar()
        {
            Visible = true;
            Width.Pixels = 20f;
            SetPadding(5);

            BgColor = UIStyle.ScrollBarBg;
            Border = UIStyle.ItemSlotBorderSize;
            BorderColor = UIStyle.ScrollBarBorder;
            InnerBg = UIStyle.ScrollBarInnerBg;
            InnerBgHover = UIStyle.ScrollBarInnerBgHover;
            RoundMultiplier = UIStyle.ScrollBarRoundMultiplier;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            CalculatedStyle InnerDimensions = GetInnerDimensions();
            CalculatedStyle InnerRectangle = InnerDimensions;
            InnerRectangle.Y += (BarTop / BarMaxTop) * (InnerDimensions.Height * (1 - BarHeightPercent));
            InnerRectangle.Height = InnerDimensions.Height * BarHeightPercent;
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
                if (Math.Abs(BarHeightPercent - 1) > 0.000000001f)
                    BarTop = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) /
                        (InnerDimensions.Height * (1 - BarHeightPercent)) * BarMaxTop;
            }

            if (BarTopBuffer == 0)
            {
                return;
            }

            BarTop -= BarTopBuffer * 0.2f;
            BarTopBuffer *= 0.8f;
            if (!(MathF.Abs(BarTopBuffer) < 0.1f))
            {
                return;
            }

            BarTop = MathF.Round(BarTop, 1);
            BarTopBuffer = 0;
        }

        /// <summary>
        /// 鼠标进入内部
        /// </summary>
        public virtual void InnerMouseOver()
        {
            if ((!HideIfFilled && !HideInnerIfFilled) || !InnerFilled)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        /// <summary>
        /// 鼠标出去内部
        /// </summary>
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
                          (innerDimensions.Height * (1 - BarHeightPercent) * (barTop / BarMaxTop));
            }

            BarTopBuffer = 0;
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
            if (BarMaxTop != 0)
                innerPosition.Y += innerDimensions.Height * (1 - BarHeightPercent) * (BarTop / BarMaxTop);
            innerSize.Y *= BarHeightPercent;

            Color hoverColor = Color.Lerp(InnerBg, InnerBgHover, BarDragging ? 1 : HoverTimer.Schedule);
            var round = new Vector4(MathF.Min(innerSize.X, innerSize.Y) * RoundMultiplier);

            // 滚动条拖动块
            SDFRectangle.NoBorder(innerPosition, innerSize, round, hoverColor);
        }

        /// <summary>
        /// 设置要控制的 UI 的父元素显示区域高度和内部元素实际高度
        /// </summary>
        /// <param name="displayHeight">父元素展示高度</param>
        /// <param name="actualHeight">内部元素实际高度</param>
        public void SetView(float displayHeight, float actualHeight)
        {
            displayHeight = MathHelper.Clamp(displayHeight, 0f, actualHeight);
            barTop = MathHelper.Clamp(barTop, 0f, actualHeight - displayHeight);

            this.displayHeight = displayHeight;
            this.actualHeight = actualHeight;
        }
    }
}