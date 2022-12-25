using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.PlayerInfo.UIElements;

namespace ImproveGame.Interface.SUIElements
{
    public class SUIPanel : UIElement
    {
        internal bool Shaded;
        internal float ShadowThickness;
        internal Color ShadowColor;
        internal bool Draggable;
        internal bool Dragging;
        internal Vector2 Offset;

        public float round;
        public float border;
        public Color borderColor;
        public Color backgroundColor;
        public bool CalculateBorder;

        public SUIPanel(Color borderColor, Color backgroundColor, float round = 12, float border = 3, bool CalculateBorder = true)
        {
            SetPadding(10f);
            ShadowThickness = 50f;
            ShadowColor = new Color(0, 0, 0, 0.25f);
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.round = round;
            this.border = border;
            this.CalculateBorder = CalculateBorder;
            OnMouseDown += DragStart;
            OnMouseUp += DragEnd;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimenstions = GetDimensions();
            Vector2 pos = dimenstions.Position();
            Vector2 size = dimenstions.Size();

            if (Shaded)
            {
                Vector2 ShadowThickness = new Vector2(this.ShadowThickness);
                Vector2 ShadowPos = pos - ShadowThickness;
                Vector2 ShadowSize = size + ShadowThickness * 2;
                PixelShader.DrawShadow(ShadowPos, ShadowSize, round, ShadowColor, this.ShadowThickness);
            }

            if (CalculateBorder)
            {
                pos -= new Vector2(border);
                size += new Vector2(border * 2);
            }

            PixelShader.DrawRoundRect(pos, size, round, backgroundColor, border, borderColor);
        }

        // 可拖动界面
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            // 当点击的是子元素不进行移动
            if (Draggable && (evt.Target == this || evt.Target is SUITitle or PlyTip || evt.Target.GetType().IsAssignableFrom(typeof(RelativeUIE)) || evt.Target.GetType().IsAssignableFrom(typeof(UIElement))))
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
