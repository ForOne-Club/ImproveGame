using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class Checkbox : UIElement
    {
        private string text;
        public float textScale;
        private Vector2 textSize;
        public Color textColor = Color.White;
        public Color textBorderColor = Color.Black;

        public Color color1 = Color.Transparent;
        public Color color2 = Color.DarkGreen;

        public AnimationTimer HoverTimer = new(3);
        public AnimationTimer SelectTimer = new(3);
        public bool check;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = MyUtils.GetTextSize(value);
            }
        }

        public Checkbox(string text, float textScale = 1f)
        {
            Text = text;
            this.textScale = textScale;

            PaddingLeft = 14 * textScale;
            PaddingRight = 14 * textScale;
            PaddingTop = 5 * textScale;
            PaddingBottom = 5 * textScale;

            Width.Pixels = (textSize.X + 26) * textScale + this.HPadding();
            Height.Pixels = MathF.Max(textSize.Y * textScale, 20) + this.VPadding();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update();
            SelectTimer.Update();
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            HoverTimer.Open();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            HoverTimer.Close();
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            if (!SelectTimer.AnyClose)
            {
                SelectTimer.Close();
            }
            else if (!SelectTimer.AnyOpen)
            {
                SelectTimer.Open();
            }
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle rectangle = GetDimensions();
            Vector2 position = rectangle.Position();
            Vector2 size = rectangle.Size();

            Color color = Color.Lerp(new(240, 62, 62), new(62, 240, 71), SelectTimer.Schedule);
            Color background1 = Color.Lerp(Color.Transparent, color, HoverTimer.Schedule);
            Color background2 = Color.Lerp(Color.Transparent, Color.White, HoverTimer.Schedule);
            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, 6, 3, background2 * 0.8f, background1 * 0.8f);


            rectangle = GetInnerDimensions();
            position = rectangle.Position();
            size = rectangle.Size();


            Vector2 boxSize = new Vector2(21) * textScale;

            PixelShader.DrawBox(Main.UIScaleMatrix, position + new Vector2(0, size.Y / 2 - boxSize.Y / 2),
                boxSize, 4, 3, Color.White, color);

            MyUtils.DrawString(position + new Vector2(boxSize.X + 5 * textScale, size.Y / 2 - textSize.Y * textScale / 2 + 5 * textScale), text, Color.White,
                textBorderColor, textScale);
        }
    }
}
