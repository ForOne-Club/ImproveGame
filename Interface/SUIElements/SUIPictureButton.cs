using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 图片按钮
    /// </summary>
    public class SUIPictureButton : HoverSUIE
    {
        public int[] data = new int[5];

        public string text;
        public Vector2 textSize;
        public Vector2 TextPosition => new(imagePosition.X + imageSize.X + 10, UIConfigs.Instance.TextDrawOffsetY + this.Height() / 2 - textSize.Y / 2);

        public Texture2D image;
        public Vector2 imageSize;
        public Vector2 imagePosition;

        public SUIPictureButton(Texture2D texture, string text)
        {
            Width.Pixels = GetTextSize(text).X + this.HPadding() + 75;
            Height.Pixels = 40f;

            image = texture;
            imageSize = texture.Size();
            imagePosition = new Vector2(30, this.Height() / 2) - imageSize / 2;

            this.text = text;
            textSize = GetTextSize(text);
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Width.Pixels = GetTextSize(text).X + this.HPadding() + 75;
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();
            Vector2 size = dimensions.Size();

            Color borderColor = Color.Lerp(UIColor.Default.PanelBorder, UIColor.Default.SlotFavoritedBorder, hoverTimer.Schedule);
            PixelShader.DrawRoundRect(position, size, 10, UIColor.Default.ButtonBackground, 3, borderColor);

            dimensions = GetInnerDimensions();
            position = dimensions.Position();
            sb.Draw(image, position + imagePosition, Color.White);
            TrUtils.DrawBorderString(sb, text, position + TextPosition, Color.White);
        }

        public void SetText(string text)
        {
            this.text = text;
            textSize = GetTextSize(text);
        }

        public void SetImage(Texture2D texture)
        {
            image = texture;
            imageSize = texture.Size();
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
}
