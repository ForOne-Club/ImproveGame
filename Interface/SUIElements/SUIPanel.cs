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

        public SUIPanel(Color borderColor, Color backgroundColor, float round = 12, float border = 3, bool Draggable = false)
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
            OnMouseDown += DragStart;
            OnMouseUp += DragEnd;
        }

        public SUIPanel(Color backgroundColor, Vector4 round4)
        {
            SetPadding(10f);
            DragIgnore = true;
            RoundMode = RoundMode.Round4;
            ShadowThickness = 50f;
            ShadowColor = new Color(0, 0, 0, 0.25f);
            this.backgroundColor = backgroundColor;
            this.round4 = round4;
            OnMouseDown += DragStart;
            OnMouseUp += DragEnd;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimenstions = GetDimensions();
            Vector2 pos = dimenstions.Position();
            Vector2 size = dimenstions.Size();
            pos -= new Vector2(Extension.X, Extension.Y);
            size += new Vector2(Extension.X + Extension.Z, Extension.Y + Extension.W);
            switch (RoundMode)
            {
                case RoundMode.Round:
                    if (Shaded)
                    {
                        Vector2 ShadowThickness = new Vector2(this.ShadowThickness);
                        Vector2 ShadowPos = pos - ShadowThickness;
                        Vector2 ShadowSize = size + ShadowThickness * 2;
                        PixelShader.DrawShadow(ShadowPos, ShadowSize, round, ShadowColor, this.ShadowThickness);
                    }
                    PixelShader.DrawRoundRect(pos, size, round, backgroundColor, border, borderColor);
                    break;
                case RoundMode.Round4:
                    PixelShader.DrawRoundRect(pos, size, round4, backgroundColor, 3, UIColor.PanelBorder);
                    break;
            }
        }

        // 可拖动界面
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            View view = evt.Target is View ? evt.Target as View : null;
            // 当点击的是子元素不进行移动
            if (Draggable && (evt.Target == this || (view is not null && view.DragIgnore) || evt.Target.GetType().IsAssignableFrom(typeof(View)) || evt.Target.GetType().IsAssignableFrom(typeof(UIElement))))
            {
                Offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
                Dragging = true;
            }
        }

        // 可拖动/调整大小界面
        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            Dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (Dragging)
            {
                Left.Set(Main.mouseX - Offset.X, 0f);
                Top.Set(Main.mouseY - Offset.Y, 0f);
                Recalculate();
                OnDrag?.Invoke(this);
            }
        }

        internal event ElementEvent OnDrag;
    }
}
