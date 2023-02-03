using ImproveGame.Common.Configs;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.SUIElements
{
    public class SUITitle : View
    {
        private static DynamicSpriteFont LargeText => FontAssets.DeathText.Value;

        private readonly float _textScale;
        private string _text;

        public Vector2 TextOffset;
        public Vector2 TextAlign = new Vector2(0.5f);
        public Vector2 TextSize;
        public Color textColor = Color.White;
        public Color textBorderColor = Color.Black;

        public SUITitle(string text, float textScale)
        {
            _textScale = textScale;
            DragIgnore = true;
            Rounded = new Vector4(10f);

            SetText(text);
            SetPadding(20f, 5f);
            SetInnerPixels(TextSize);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();
            Vector2 textOffset = (innerSize - TextSize) * TextAlign + TextOffset;
            textOffset.Y += UIConfigs.Instance.BigFontOffsetY * _textScale;
            Vector2 textPos = innerPos + textOffset;
            ChatManager.DrawColorCodedStringWithShadow(sb, LargeText, _text, textPos, textColor, 0f, new Vector2(0f), new Vector2(_textScale));
        }

        public void SetText(string text)
        {
            _text = text;
            TextSize = GetFontSize(_text, true) * _textScale;
        }
    }
}
