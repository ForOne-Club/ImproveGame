using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using Terraria.GameContent.UI;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class UITitle : UIElement
    {
        private string text;
        private Vector2 textSize;
        private float scale;
        public Color textColor;
        public Color textBorderColor;
        public Color background = new(35, 40, 83);

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = GetBigTextSize(text) * scale;
                Width.Pixels = textSize.X + this.HPadding();
                Height.Pixels = textSize.Y + this.VPadding();
            }
        }

        public UITitle(string text, float scale)
        {
            this.scale = scale;

            //PaddingTop = 5;
            //PaddingBottom = 5;
            PaddingLeft = 30;
            PaddingRight = 30;

            Text = text;
            Width.Pixels = textSize.X + this.HPadding();
            Height.Pixels = textSize.Y + this.VPadding();
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle rectangle = GetDimensions();
            Vector2 position = rectangle.Position();
            Vector2 size = rectangle.Size();

            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, 10, 0, UIColor.Default.TitleBackground, UIColor.Default.TitleBackground);
            rectangle = GetInnerDimensions();
            position = rectangle.Position();
            size = rectangle.Size();
            Utils.DrawBorderStringBig(sb, text, position + new Vector2(0, size.Y / 2 - textSize.Y / 2 + 14 * scale), Color.White, scale);
        }
    }
}
