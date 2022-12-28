using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyTip : RelativeElement
    {
        public const float w = 220f;
        public const float h = 42f;
        private Texture2D icon;
        private Color background;
        private string text;
        private Vector2 textSize;
        private AnimationTimer hoverTimer;
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
            hoverTimer = new();
            Relative = true;
            Wrap = true;
            Layout = RelativeMode.Vertical;
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

        public override void Update(GameTime gameTime)
        {
            hoverTimer.Update();
            base.Update(gameTime);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            hoverTimer.Open();
            base.MouseOver(evt);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            hoverTimer.Close();
            base.MouseOut(evt);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            float hoverSize = hoverTimer.Schedule * 4;
            Color borderColor = Color.Lerp(new Color(0, 0, 0, 0.25f), new Color(0, 0, 0, 0.5f), hoverTimer.Schedule);
            Color backgroundColor = Color.Lerp(new Color(0, 0, 0, 0.25f), new Color(0, 0, 0, 0.5f), hoverTimer.Schedule);
            Vector2 pos = GetDimensions().Position() - new Vector2(hoverSize);
            Vector2 size = GetDimensions().Size() + new Vector2(hoverSize * 2);
            PixelShader.DrawRoundRect(pos + new Vector2(4), size - new Vector2(8), 6 + hoverSize, backgroundColor);
            PixelShader.DrawRoundRect(pos, size, 10 + hoverSize, Color.Transparent, 3, borderColor);

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
