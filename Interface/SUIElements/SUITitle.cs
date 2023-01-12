using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUITitle : View
    {
        private string text;
        public Vector2 textSize;
        private float textScale;
        public Color textColor;
        public Color textBorderColor;
        public Color background;
        public Color border;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = GetBigTextSize(text) * textScale;
            }
        }

        public SUITitle(string text, float textScale)
        {
            this.textScale = textScale;
            this.Text = text;
            background = UIColor.TitleBg;
            PaddingTop = 5;
            PaddingBottom = 5;
            PaddingLeft = 30;
            PaddingRight = 30;
            DragIgnore = true;
            Round = 10f;

            SetInnerPixels(textSize);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle rectangle = GetDimensions();
            Vector2 position = rectangle.Position();
            Vector2 size = rectangle.Size();

            switch (RoundMode)
            {
                case RoundMode.Round:
                    PixelShader.DrawRoundRect(position, size, Round, background);
                    break;
                case RoundMode.Round4:
                    PixelShader.DrawRoundRect(position, size, Round4, background, 2, border);
                    break;
            }
            rectangle = GetInnerDimensions();
            position = rectangle.Position();
            size = rectangle.Size();
            Utils.DrawBorderStringBig(sb, text, position + (size - textSize) / 2f + new Vector2(0, UIConfigs.Instance.TextDrawOffsetY * 3 * textScale), Color.White, textScale);
        }
    }
}
