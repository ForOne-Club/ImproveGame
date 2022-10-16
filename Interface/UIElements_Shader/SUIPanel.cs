using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class SUIPanel : UIElement
    {
        internal bool Draggable;
        internal bool Dragging;
        internal Vector2 Offset;

        public float radius;
        public float border;
        public Color borderColor;
        public Color backgroundColor;
        public bool CalculateBorder;

        public SUIPanel(Color borderColor, Color backgroundColor, float radius = 12, float border = 3, bool CalculateBorder = true)
        {
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.radius = radius;
            this.border = border;
            this.CalculateBorder = CalculateBorder;
            SetPadding(16);
            OnMouseDown += DragStart;
            OnMouseUp += DragEnd;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimenstions = GetDimensions();
            Vector2 position = dimenstions.Position();
            Vector2 size = new(dimenstions.Width, dimenstions.Height);

            if (CalculateBorder)
            {
                position -= new Vector2(border);
                size += new Vector2(border * 2);
            }

            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, radius, border,
                borderColor, backgroundColor);
        }
        
        // 可拖动界面
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Draggable)
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

            if (IsMouseHovering) {
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
