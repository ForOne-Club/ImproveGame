using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoCard : RelativeElement
    {
        public static float width = 210;
        public static float height = 40f;
        public static readonly Vector2 spacing = new Vector2(6);
        /// <summary> 计算总大小 </summary>
        public static Vector2 TotalSize(int row, int column)
        {
            return new Vector2((width + spacing.X) * row - spacing.X, (height + spacing.Y) * column - spacing.Y);
        }
        private readonly Texture2D icon;
        private string text;
        private Vector2 textSize;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = FontAssets.MouseText.Value.MeasureString(value);
            }
        }
        private readonly Func<string> textFunc;

        public PlyInfoCard(string text, Func<string> textFunc, string icon)
        {
            Relative = true;
            Wrap = true;
            Layout = RelativeMode.Vertical;
            Spacing = spacing;

            PaddingLeft = 10f;
            PaddingRight = 14f;
            Width.Pixels = width;
            Height.Pixels = height;

            Text = text;
            this.textFunc = textFunc;
            this.textFunc ??= () => string.Empty;
            this.icon = GetTexture($"UI/PlayerInfo/{icon}").Value;
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();

            PixelShader.DrawRoundRect(pos, size, 10, new Color(43, 56, 101), 3, UIColor.PanelBorder);

            Vector2 iconPos = innerPos + new Vector2(30, innerSize.Y) / 2f;
            BigBagItemSlot.DrawItemIcon(sb, icon, Color.White, iconPos, icon.Size() / 2f, 30);

            Vector2 textPos = innerPos + new Vector2(30 + 4, UIConfigs.Instance.TextDrawOffsetY + (innerSize.Y - textSize.Y) / 2);
            DrawString(textPos, Text, Color.White, Color.Black);

            string infoText = textFunc();
            Vector2 infoSize = MouseTextSize(infoText);
            Vector2 infoPos = innerPos + new Vector2(innerSize.X - infoSize.X, UIConfigs.Instance.TextDrawOffsetY + (innerSize.Y - infoSize.Y) / 2);
            DrawString(infoPos, infoText, Color.White, Color.Black);
        }
    }
}
