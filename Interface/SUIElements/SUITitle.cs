using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUITitle : RelativeElement
    {
        private string text;
        private Vector2 textSize;
        private float textScale;
        public Color textColor;
        public Color textBorderColor;
        public Color background = new Color(35, 40, 83);
        public Color border = new Color();
        public int mode;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = GetBigTextSize(text) * textScale;
            }
        }

        public SUITitle(string text, float textScale, int mode = 0)
        {
            background = UIColor.TitleBg;
            this.textScale = textScale;
            this.mode = mode;
            PaddingTop = 5;
            PaddingBottom = 5;

            PaddingLeft = 30;
            PaddingRight = 30;

            Text = text;
            RefreshSize();
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle rectangle = GetDimensions();
            Vector2 position = rectangle.Position();
            Vector2 size = rectangle.Size();

            if (mode == 0)
            {
                PixelShader.DrawRoundRect(position, size, 10f, background);
            }
            else
            {
                PixelShader.DrawRoundRect(position + new Vector2(0, size.Y - 20f), new Vector2(size.X, 20f), 100f, background);
            }
            rectangle = GetInnerDimensions();
            position = rectangle.Position();
            size = rectangle.Size();
            Utils.DrawBorderStringBig(sb, text, position + (size - textSize) / 2f + new Vector2(0, UIConfigs.Instance.TextDrawOffsetY * 3 * textScale), Color.White, textScale);
        }

        public void RefreshSize()
        {
            Width.Pixels = textSize.X + this.HPadding();
            Height.Pixels = textSize.Y + this.VPadding();
        }
    }
}
