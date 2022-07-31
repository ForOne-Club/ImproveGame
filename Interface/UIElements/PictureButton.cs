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

        protected override void DrawSelf(SpriteBatch sb)
        {
            var rectangle = GetDimensions().ToRectangle();
            Utils.DrawSplicedPanel(sb, Background, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 10, 10, 10, 10, Colors.InventoryDefaultColor);
            Utils.DrawSplicedPanel(sb, BackgroundBorder, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 10, 10, 10, 10, Color.White * HoverTimer.Schedule);
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
