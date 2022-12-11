using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 图片按钮
    /// </summary>
    public class SUIPictureButton : UIElement
    {
        public int[] data = new int[5];
        public AnimationTimer HoverTimer = new(3);

        public string text;
        public Vector2 textSize;
        public Vector2 TextPosition => new(imagePosition.X + imageSize.X + 10, UIConfigs.Instance.UIYAxisOffset + this.Height() / 2 - textSize.Y / 2);

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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update();
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();
            Vector2 size = dimensions.Size();

            Vector2 shadowSize = Vector2.Lerp(new(10), new(20), HoverTimer.Schedule);
            Color shadowColor = Color.Lerp(new(0, 0, 0, 0.2f), new(0, 0, 0, 0.4f), HoverTimer.Schedule);
            PixelShader.DrawShadow(position - shadowSize / 2 + new Vector2(0, 2), size + shadowSize, 10, shadowColor, shadowSize.X);

            Color borderColor = Color.Lerp(UIColor.Default.PanelBorder, UIColor.Default.SlotFavoritedBorder, HoverTimer.Schedule);
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
            HoverTimer.Open();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            HoverTimer.Close();
        }
    }
}
