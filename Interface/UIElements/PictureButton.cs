using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements
{
    /// <summary>
    /// 图片按钮
    /// </summary>
    public class PictureButton : UIElement
    {
        private readonly Texture2D Background;
        private readonly Texture2D BackgroundBorder;

        public bool _playSound;

        public int[] data;
        public UIImage UIImage;
        public UIText UIText;
        public AnimationTimer HoverTimer;

        public PictureButton(Texture2D texture, string text)
        {
            _playSound = true;
            data = new int[5];
            Background = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale").Value;
            BackgroundBorder = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder").Value;

            Width.Pixels = MyUtils.GetTextSize(text).X + this.HPadding() + 75;
            Height.Pixels = 40f;

            Append(UIImage = new(texture)
            {
                VAlign = 0.5f
            });
            UIImage.Left.Pixels = 30f - UIImage.Width() / 2f;

            Append(UIText = new(text)
            {
                VAlign = 0.5f
            });
            UIText.Left.Pixels = 50f;

            HoverTimer = new(2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update();
            if (IsMouseHovering)
            {
                if (_playSound)
                {
                    _playSound = false;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }
            else
            {
                _playSound = true;
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            HoverTimer.Open();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            HoverTimer.Close();
        }

        private readonly Color borderColor = new(233, 176, 0);
        private readonly Color light1 = new(192, 130, 255);
        private readonly Color drak1 = new(96, 65, 127);
        private readonly Color light2 = new(56, 156, 255);
        private readonly Color drak2 = new(28, 78, 127);
        protected override void DrawSelf(SpriteBatch sb)
        {
            var rectangle = GetDimensions().ToRectangle();

            Color borderColor = Color.Lerp(drak1, light1, HoverTimer.Schedule);
            /*PixelShader.DrawBox(Main.UIScaleMatrix, GetDimensions().Position(), this.GetSize(),
                4, 3, drak1, drak2, light1, light2);*/

            Utils.DrawSplicedPanel(sb, Background, rectangle.X, rectangle.Y, rectangle.Width,
                rectangle.Height, 10, 10, 10, 10, Colors.InventoryDefaultColor);
            Utils.DrawSplicedPanel(sb, BackgroundBorder, rectangle.X, rectangle.Y, rectangle.Width,
                rectangle.Height, 10, 10, 10, 10, Color.White * HoverTimer.Schedule);
        }

        public void TextAlignCenter()
        {
            float left = 30f + (this.WidthInside() - 30f) / 2f;
            UIText.Left.Pixels = left - UIText.Width() / 2f;
            UIText.Recalculate();
        }

        public void SetText(string text)
        {
            Width.Pixels = MyUtils.GetTextSize(text).X + this.HPadding() + 75;
            UIText.Width.Pixels = MyUtils.GetTextSize(text).X;
            UIText.SetText(text);
        }

        public void SetImage(Texture2D texture)
        {
            UIImage.SetImage(texture);
            UIImage.Left.Pixels = 15 + UIImage.Width() / 2f;
        }
    }
}
