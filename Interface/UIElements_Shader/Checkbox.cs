using ImproveGame.Common.Animations;
using Microsoft.Xna.Framework;

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
            Text = text;
            this.textScale = textScale;
            this.GetState = GetState;
            this.SetState = SetState;

            PaddingLeft = 14 * textScale;
            PaddingRight = 14 * textScale;
            PaddingTop = 5 * textScale;
            PaddingBottom = 5 * textScale;

            Width.Pixels = (textSize.X + 26) * textScale + this.HPadding();
            Height.Pixels = MathF.Max(textSize.Y * textScale, 20) + this.VPadding();
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            SetState(!GetState());
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Color color = Color.Lerp(Color.Transparent, new(62, 240, 71), GetState() ? 1 : 0);

            Vector2 position = GetInnerDimensions().Position();
            Vector2 size = GetInnerDimensions().Size();
            Vector2 boxSize = new Vector2(21) * textScale;

            PixelShader.DrawBox(Main.UIScaleMatrix, position + new Vector2(0, size.Y / 2 - boxSize.Y / 2),
                boxSize, 4, 3, Color.White, color);

            DrawString(position + new Vector2(boxSize.X + 5 * textScale, size.Y / 2 - textSize.Y * textScale / 2 + 5 * textScale), text, Color.White,
                textBorderColor, textScale);
        }
    }
}
