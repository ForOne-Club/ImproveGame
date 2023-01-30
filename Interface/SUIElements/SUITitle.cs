using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUITitle : View
    {
        private string text;
        public Vector2 textSize;
        private float _textScale;
        public Color textColor;
        public Color textBorderColor;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = GetFontSize(text, true) * _textScale;
            }
        }

        public SUITitle(string text, float textScale)
        {
            _textScale = textScale;
            Text = text;
            DragIgnore = true;
            Rounded = new Vector4(10f);
            SetPadding(20f, 5f);
            SetInnerPixels(textSize);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();
            Utils.DrawBorderStringBig(sb, text, innerPos + (innerSize - textSize) / 2f + new Vector2(0, UIConfigs.Instance.BigFontOffsetY * _textScale), Color.White, _textScale);
        }
    }
}
