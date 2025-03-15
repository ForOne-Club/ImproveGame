using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.UIFramework.SUIElements
{
    public class SUIPanel : View
    {
        /// <summary>
        /// 显示窗口阴影
        /// </summary>
        internal bool Shaded;

        internal float ShadowThickness;
        internal Color ShadowColor;

        /// <summary>
        /// 可拖动
        /// </summary>
        internal bool Draggable;
        internal bool Dragging;
        internal Vector2 Offset;

        /// <summary>
        /// 可调节大小
        /// </summary>
        internal bool Resizeable;
        internal bool Resizing;
        internal int MinResizeWidth = 200;
        internal int MinResizeHeight = 200;
        /// <summary>
        /// 鼠标放到可以调节大小的范围时，会显示一个物品图标，设置为-1则不显示
        /// </summary>
        internal int ItemIdForResizeIcon = ItemID.TitanGlove;

        public Rectangle ResizeRectangle
        {
            get
            {
                CalculatedStyle innerDimensions = GetInnerDimensions();
                return new Rectangle((int)(innerDimensions.X + innerDimensions.Width - 12), (int)(innerDimensions.Y + innerDimensions.Height - 12), 12 + (int)PaddingRight, 12 + (int)PaddingBottom);
            }
        }

        public SUIPanel(Color borderColor, Color backgroundColor, float rounded = 12, float border = 2,
            bool draggable = false)
        {
            SetPadding(10f);
            Draggable = draggable;
            DragIgnore = true;

            ShadowThickness = UIStyle.ShadowThickness;
            ShadowColor = borderColor * 0.35f;

            Border = border;
            BorderColor = borderColor;
            BgColor = backgroundColor;
            Rounded = new Vector4(rounded);
        }

        public SUIPanel(Color backgroundColor, Color borderColor, Vector4 rounded, float border, bool draggable = false)
        {
            SetPadding(10f);
            DragIgnore = true;
            ShadowThickness = UIStyle.ShadowThickness;
            ShadowColor = borderColor * 0.35f;
            Draggable = draggable;

            Border = border;
            BorderColor = borderColor;
            BgColor = backgroundColor;
            Rounded = rounded;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (Resizeable && ResizeRectangle.Contains(evt.MousePosition.ToPoint()))
            {
                Offset = new Vector2(evt.MousePosition.X - PositionPixels.X - innerDimensions.Width, evt.MousePosition.Y - PositionPixels.Y - innerDimensions.Height);
                Resizing = true;
            }
            // 当点击的是子元素不进行移动
            else if (Draggable &&
                (evt.Target == this || (evt.Target is View view && view.DragIgnore) ||
                 evt.Target.GetType().IsAssignableFrom(typeof(UIElement))))
            {
                Offset = evt.MousePosition - PositionPixels;
                Dragging = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            Dragging = false;
            Resizing = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Dragging)
            {
                SetPosPixels(Main.mouseX - Offset.X, Main.mouseY - Offset.Y).Recalculate();
            }

            if (Resizing)
            {
                CalculatedStyle dimensions = GetOuterDimensions();
                Width.Pixels = Math.Max(Main.MouseScreen.X - dimensions.X - Offset.X, MinResizeWidth);
                Height.Pixels = Math.Max(Main.MouseScreen.Y - dimensions.Y - Offset.Y, MinResizeHeight);
                Recalculate();
            }

            if (Resizeable && ResizeRectangle.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.instance.MouseText($"[i:{ItemIdForResizeIcon}]", 0, 0, Main.mouseX + 16, Main.mouseY - 10);
            }

            base.Draw(spriteBatch);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Vector2 ShadowThickness = new Vector2(this.ShadowThickness);
            Vector2 shadowPos = pos - ShadowThickness;
            Vector2 shadowSize = size + ShadowThickness * 2;

            if (Shaded)
            {
                SDFRectangle.Shadow(shadowPos, shadowSize, Rounded, ShadowColor, this.ShadowThickness);
            }
            base.DrawSelf(spriteBatch);
        }
    }
}