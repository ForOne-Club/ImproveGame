using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class Checkbox : UIElement
    {
        public Func<bool> GetState;
        public Action<bool> SetState;
        private string text;
        public float textScale;
        private Vector2 textSize;
        public Color textColor = Color.White;
        public Color textBorderColor = Color.Black;

        public Color color1 = Color.Transparent;
        public Color color2 = Color.DarkGreen;
        public AnimationTimer AT;

        public float Spacing = 8;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = GetTextSize(value);
            }
        }

        public Checkbox(Func<bool> GetState, Action<bool> SetState, string text, float textScale = 1f)
        {
            AT = new(3);
            Text = text;
            this.textScale = textScale;
            this.GetState = GetState;
            this.SetState = SetState;

            PaddingLeft = 14 * textScale;
            PaddingRight = 14 * textScale;
            PaddingTop = 5 * textScale;
            PaddingBottom = 5 * textScale;

            Width.Pixels = (textSize.X + Spacing + 48) * textScale + this.HPadding();
            Height.Pixels = MathF.Max(textSize.Y * textScale, 26) + this.VPadding();
        }

        public override void Update(GameTime gameTime)
        {
            AT.Update();
            base.Update(gameTime);
            if (GetState() && !AT.AnyOpen)
            {
                AT.Open();
            }
            else if (!GetState() && !AT.AnyClose)
            {
                AT.Close();
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            SetState(!GetState());
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Color color = Color.Lerp(new Color(0, 0, 0, 50), new(0, 0, 0, 25), AT.Schedule);
            Color color2 = Color.Lerp(new(27, 30, 61), new(242, 188, 23), AT.Schedule);

            Vector2 position = GetInnerDimensions().Position();
            Vector2 size = GetInnerDimensions().Size();
            Vector2 boxSize = new Vector2(48, 26) * textScale;

            PixelShader.DrawBox(Main.UIScaleMatrix, position + new Vector2(0, size.Y / 2 - boxSize.Y / 2),
                boxSize, boxSize.Y / 2, 3, color2, color);

            Vector2 boxSize2 = new(boxSize.Y - 10 * textScale);
            Vector2 position2 = position + Vector2.Lerp(new Vector2(3 + 2, size.Y / 2 - boxSize2.Y / 2), new Vector2(boxSize.X - 3 - 2 - boxSize2.X, size.Y / 2 - boxSize2.Y / 2), AT.Schedule);
            PixelShader.DrawBox(Main.UIScaleMatrix, position2,
                boxSize2, boxSize2.Y / 2, 0, color2, color2);

            DrawString(position + new Vector2(boxSize.X + Spacing * textScale, size.Y / 2 - textSize.Y * textScale / 2 + 5 * textScale), text, Color.White,
                textBorderColor, textScale);
        }
    }
}
