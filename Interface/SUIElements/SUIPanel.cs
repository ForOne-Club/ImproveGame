using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseViews;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
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

        public float border;
        public Color borderColor;
        public Color backgroundColor;

        public SUIPanel(Color borderColor, Color backgroundColor, float round = 12, float border = 2,
            bool Draggable = false)
        {
            SetPadding(10f);
            DragIgnore = true;
            RoundMode = RoundMode.Round;
            ShadowThickness = 50f;
            ShadowColor = new Color(0, 0, 0, 0.25f);
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.round = round;
            this.border = border;
            this.Draggable = Draggable;
        }

        public SUIPanel(Color backgroundColor, Color borderColor, Vector4 round4, float border, bool Draggable = false)
        {
            SetPadding(10f);
            DragIgnore = true;
            RoundMode = RoundMode.Round4;
            ShadowThickness = 50f;
            ShadowColor = new Color(0, 0, 0, 0.25f);
            this.backgroundColor = backgroundColor;
            this.borderColor = borderColor;
            this.round4 = round4;
            this.border = border;
            this.Draggable = Draggable;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            pos -= new Vector2(Extension.X, Extension.Y);
            size += new Vector2(Extension.X + Extension.Z, Extension.Y + Extension.W);
            switch (RoundMode)
            {
                case RoundMode.Round:
                    if (Shaded)
                    {
                        Vector2 ShadowThickness = new Vector2(this.ShadowThickness);
                        Vector2 shadowPos = pos - ShadowThickness;
                        Vector2 shadowSize = size + ShadowThickness * 2;
                        PixelShader.DrawShadow(shadowPos, shadowSize, round, ShadowColor, this.ShadowThickness);
                    }

                    PixelShader.DrawRoundRect(pos, size, round, backgroundColor, border, borderColor);
                    break;
                case RoundMode.Round4:
                    PixelShader.DrawRoundRect(pos, size, round4, backgroundColor, border, UIColor.PanelBorder);
                    break;
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            // 可拖动界面
            View view = evt.Target as View;
            // 当点击的是子元素不进行移动
            if (!Draggable ||
                (evt.Target != this && (view is null || !view.DragIgnore) &&
                 !evt.Target.GetType().IsAssignableFrom(typeof(UIElement))))
            {
                return;
            }

            Offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            Dragging = true;
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            Dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (!Dragging)
            {
                return;
            }

            SetPosPixels(Main.mouseX - Offset.X, Main.mouseY - Offset.Y).Recalculate();
            OnDrag?.Invoke(this);
        }

        internal event ElementEvent OnDrag;
    }
}