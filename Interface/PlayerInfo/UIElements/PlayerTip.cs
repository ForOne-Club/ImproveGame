using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlayerTip : RelativeUIE
    {
        public const float w = 250f;
        public const float h = 40f;
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

        public PlayerTip(Func<string> textFunc)
        {
            Relative = true;
            AutoLineFeed = true;
            Mode = RelativeMode.Horizontal;
            Interval = new Vector2(10, 10);

            background = UIColor.TitleBackground;
            PaddingLeft = 8f;
            Width.Pixels = w;
            Height.Pixels = h;

            this.textFunc = textFunc;
            this.textFunc ??= () => string.Empty;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 8, background);

            string newText = textFunc();
            if (newText != Text)
            {
                Text = newText;
                Main.NewText($"刷新：{Text}");
            }
            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();
            innerPos.Y += UIConfigs.Instance.TextDrawOffsetY + innerSize.Y / 2 - textSize.Y / 2;
            DrawString(innerPos, Text, Color.White, Color.Black);
        }
    }
}
