using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements
{
    /// <summary>
    /// 图片按钮
    /// </summary>
    public class PictureButton : UIElement
    {
        // private readonly Texture2D Background = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale").Value;
        // private readonly Texture2D BackgroundBorder = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder").Value;

        public int[] data = new int[5];
        public AnimationTimer HoverTimer = new(2);

        public string text;
        public Vector2 textSize;
        public Vector2 TextPosition => new(imagePosition.X + imageSize.X + 10, 6 + this.Height() / 2 - textSize.Y / 2);

        public Texture2D image;
        public Vector2 imageSize;
        public Vector2 imagePosition;

        public PictureButton(Texture2D texture, string text)
        {
            Width.Pixels = MyUtils.GetTextSize(text).X + this.HPadding() + 75;
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

        private readonly Color BorderColor1 = new(18, 18, 38);
        private readonly Color BorderColor2 = new(233, 176, 0);
        private readonly Color BackgroundColor1 = new(54, 56, 130);
        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();
            Color borderColor = Color.Lerp(BorderColor1, BorderColor2, HoverTimer.Schedule);
            PixelShader.DrawBox(Main.UIScaleMatrix, position, this.GetSize(), 10, 3, borderColor,
                BackgroundColor1);

            sb.Draw(image, position + imagePosition, Color.White);
            MyUtils.DrawString(position + TextPosition, text, Color.White, Color.Black);

            // 原绘制
            /*var rectangle = GetDimensions().ToRectangle();
            Utils.DrawSplicedPanel(sb, Background, rectangle.X, rectangle.Y, rectangle.Width,
                rectangle.Height, 10, 10, 10, 10, Colors.InventoryDefaultColor);
            Utils.DrawSplicedPanel(sb, BackgroundBorder, rectangle.X, rectangle.Y, rectangle.Width,
                rectangle.Height, 10, 10, 10, 10, Color.White * HoverTimer.Schedule);*/
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

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
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
