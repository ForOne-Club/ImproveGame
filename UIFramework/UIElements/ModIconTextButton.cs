namespace ImproveGame.UIFramework.UIElements
{
    /// <summary>
    /// 基本上都是从<see cref="UIIconTextButton"/>抄来的，好处在于自己可以乱改
    /// </summary>
    public class ModIconTextButton : UIElement
    {
        private Asset<Texture2D> _BasePanelTexture;
        private Asset<Texture2D> _hoveredTexture;
        private Asset<Texture2D> _iconTexture;
        private Color _color;
        private Color _hoverColor;
        public float FadeFromBlack = 1f;
        private float _whiteLerp = 0.7f;
        private float _opacity = 0.7f;
        private bool _hovered;
        private bool _soundedHover;
        private UIText _title;

        public ModIconTextButton(LocalizedText title, Color textColor, string iconTexturePath, float textSize = 1f, float titleAlignmentX = 0.5f, float titleWidthReduction = 10f) {
            Width = StyleDimension.FromPixels(44f);
			Height = StyleDimension.FromPixels(34f);
			_hoverColor = Color.White;
			_BasePanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale");
			_hoveredTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight");
			if (iconTexturePath != null)
				_iconTexture = Main.Assets.Request<Texture2D>(iconTexturePath);

			SetColor(Color.Lerp(Color.Black, Colors.InventoryDefaultColor, FadeFromBlack), 1f);
			if (title != null)
				SetText(title, textSize, textColor);
        }

        public void SetText(LocalizedText text, float textSize, Color color, bool changeSize = false) {
            if (_title != null)
                _title.Remove();

            UIText uIText = new UIText(text, textSize) {
                HAlign = 0f,
                VAlign = 0.5f,
                Top = StyleDimension.FromPixels(0f),
                Left = StyleDimension.FromPixelsAndPercent(10f, 0f),
                IgnoresMouseInteraction = true
            };

            uIText.TextColor = color;
            Append(uIText);
            _title = uIText;
            if (_iconTexture != null && changeSize) {
                Width.Set(_title.GetDimensions().Width + (float)_iconTexture.Width() + 26f, 0f);
                Height.Set(Math.Max(_title.GetDimensions().Height, _iconTexture.Height()) + 16f, 0f);
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch) {
            if (_hovered) {
                if (!_soundedHover)
                    SoundEngine.PlaySound(SoundID.MenuTick);

                _soundedHover = true;
            }
            else {
                _soundedHover = false;
            }

            CalculatedStyle dimensions = GetDimensions();
            Color color = _color;
            float opacity = _opacity;
            Utils.DrawSplicedPanel(spriteBatch, _BasePanelTexture.Value, (int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height, 10, 10, 10, 10, Color.Lerp(Color.Black, color, FadeFromBlack) * opacity);
            if (_iconTexture != null) {
                Color color2 = Color.Lerp(color, Color.White, _whiteLerp) * opacity;
                spriteBatch.Draw(_iconTexture.Value, new Vector2(dimensions.X + dimensions.Width - (float)_iconTexture.Width() - 5f, dimensions.Center().Y - (float)(_iconTexture.Height() / 2)), color2);
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt) {
            SoundEngine.PlaySound(SoundID.MenuTick);
            base.LeftMouseDown(evt);
        }

        public override void MouseOver(UIMouseEvent evt) {
            base.MouseOver(evt);
            SetColor(Color.Lerp(Colors.InventoryDefaultColor, Color.White, _whiteLerp), 0.7f);
            _hovered = true;
        }

        public override void MouseOut(UIMouseEvent evt) {
            base.MouseOut(evt);
            SetColor(Color.Lerp(Color.Black, Colors.InventoryDefaultColor, FadeFromBlack), 1f);
            _hovered = false;
        }

        public void SetColor(Color color, float opacity) {
            _color = color;
            _opacity = opacity;
        }

        public void SetHoverColor(Color color) {
            _hoverColor = color;
        }

        public void SetIconTexture(string iconTexturePath, bool vanilla) {
            _iconTexture = vanilla ? Main.Assets.Request<Texture2D>(iconTexturePath) : ModContent.Request<Texture2D>(iconTexturePath);
        }

        public void SetHoverTexture(string iconTexturePath, bool vanilla) {
            _hoveredTexture = vanilla ? Main.Assets.Request<Texture2D>(iconTexturePath) : ModContent.Request<Texture2D>(iconTexturePath);
        }

        public void SetPanelTexture(string iconTexturePath, bool vanilla) {
            _BasePanelTexture = vanilla ? Main.Assets.Request<Texture2D>(iconTexturePath) : ModContent.Request<Texture2D>(iconTexturePath);
        }
    }
}
