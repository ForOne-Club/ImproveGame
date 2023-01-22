using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoCard : View
    {
        internal static float width = 210, height = 40f;
        private static readonly Vector2 spacing = new Vector2(6);

        /// <summary> 计算总大小 </summary>
        public static Vector2 TotalSize(int row, int column)
        {
            return new Vector2((width + spacing.X) * row - spacing.X, (height + spacing.Y) * column - spacing.Y);
        }

        private readonly Texture2D _icon;
        private string _text;
        private Vector2 _textSize;

        private string Text
        {
            get => _text;
            set
            {
                _text = value;
                _textSize = FontAssets.MouseText.Value.MeasureString(value);
            }
        }

        private readonly Func<string> _textFunc;

        public PlyInfoCard(string text, Func<string> textFunc, string icon)
        {
            Wrap = true;
            Relative = RelativeMode.Vertical;
            Spacing = spacing;

            PaddingLeft = 10f;
            PaddingRight = 14f;
            Width.Pixels = width;
            Height.Pixels = height;
            DragIgnore = true;

            Text = text;
            this._textFunc = textFunc;
            this._textFunc ??= () => string.Empty;
            this._icon = GetTexture($"UI/PlayerInfo/{icon}").Value;
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensions().Size();

            PixelShader.RoundedRectangle(pos, size, 10, new Color(43, 56, 101), 2, UIColor.PanelBorder);

            Vector2 iconPos = innerPos + new Vector2(30, innerSize.Y) / 2f;
            BigBagItemSlot.DrawItemIcon(sb, _icon, Color.White, iconPos, _icon.Size() / 2f, 30);

            Vector2 textPos = innerPos + new Vector2(30 + 4,
                UIConfigs.Instance.TextDrawOffsetY + (innerSize.Y - _textSize.Y) / 2);
            DrawString(textPos, Text, Color.White, Color.Black);

            string infoText = _textFunc();
            Vector2 infoSize = MouseTextSize(infoText);
            Vector2 infoPos = innerPos + new Vector2(innerSize.X - infoSize.X,
                UIConfigs.Instance.TextDrawOffsetY + (innerSize.Y - infoSize.Y) / 2);
            DrawString(infoPos, infoText, Color.White, Color.Black);
        }
    }
}