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

            // 当点击的是子元素不进行移动
            if (Draggable &&
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