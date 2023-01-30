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
                textSize = GetFontSize(text, true) * textScale;
            }
        }

        public SUITitle(string text, float textScale)
        {
            this.textScale = textScale;
            this.Text = text;
            background = UIColor.TitleBg;
            SetPadding(20f, 5f);
            DragIgnore = true;
            Round = 10f;

            SetInnerPixels(textSize);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            PixelShader.RoundedRectangle(pos, size, Round4, background, 2, border);

            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();
            Utils.DrawBorderStringBig(sb, text, innerPos + (innerSize - textSize) / 2f + new Vector2(0, UIConfigs.Instance.BigFontOffsetY * textScale), Color.White, textScale);
        }
    }
}
