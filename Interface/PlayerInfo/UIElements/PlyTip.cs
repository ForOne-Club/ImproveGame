using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyTip : RelativeUIE
    {
        public const float w = 220f;
        public const float h = 40f;
        private Texture2D icon;
        private Color background;
        private string text;
        private Vector2 textSize;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = GetTextSize(value);
            }
        }
        public Func<string> textFunc;

        public PlyTip(string text, Func<string> textFunc, string icon)
        {
            Relative = true;
            AutoLineFeed = true;
            Mode = RelativeMode.Vertical;
            Interval = new Vector2(10, 10);

            background = UIColor.TitleBackground;
            PaddingLeft = 8f;
            Width.Pixels = w;
            Height.Pixels = h;

            Text = text;
            this.textFunc = textFunc;
            this.textFunc ??= () => string.Empty;
            this.icon = GetTexture($"UI/PlayerInfo/{icon}").Value;
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 8, background);

            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();

            Vector2 iconPos = innerPos + new Vector2(36, innerSize.Y) / 2f;
            BigBagItemSlot.DrawItemIcon(sb, icon, Color.White, iconPos, icon.Size() / 2f, 30);

            Vector2 textPos = innerPos + new Vector2(36 + 4, UIConfigs.Instance.TextDrawOffsetY + (innerSize.Y - textSize.Y) / 2);
            DrawString(textPos, Text, Color.White, Color.Black);

            string infoText = textFunc?.Invoke();
            Vector2 infoSize = GetTextSize(infoText);
            Vector2 infoPos = innerPos + new Vector2(innerSize.X - 60f, UIConfigs.Instance.TextDrawOffsetY + (innerSize.Y - infoSize.Y) / 2);
            DrawString(infoPos, infoText, Color.White, Color.Black);
        }
    }
}
