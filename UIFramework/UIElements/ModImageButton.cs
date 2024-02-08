namespace ImproveGame.UIFramework.UIElements
{
    public class ModImageButton : UIElement
    {
        public string HoverText = "";
        public Func<Color> DrawColor;
        public Asset<Texture2D> Texture { get; private set; }
        public Color ColorActive { get; private set; }
        public Color ColorInactive { get; private set; }
        public SoundStyle? PlaySound { get; private set; }
        public Asset<Texture2D> BorderTexture { get; private set; }
        public Asset<Texture2D> BackgroundTexture { get; private set; }

        public ModImageButton(Asset<Texture2D> texture, Color activeColor = default, Color inactiveColor = default) {
            if (texture is not null) {
                Texture = texture;
                Width.Set(Texture.Width(), 0f);
                Height.Set(Texture.Height(), 0f);
            }
            ColorActive = activeColor;
            ColorInactive = inactiveColor;
            PlaySound = null;
        }

        #region 各种设置方法
        public void SetCenter(int x, int y) {
            CalculatedStyle dimensions = GetDimensions();
            Left.Set(x - dimensions.Width / 2, 0f);
            Top.Set(y - dimensions.Height / 2, 0f);
        }

        public void SetSound(SoundStyle sound) {
            PlaySound = sound;
        }

        public void SetColor(Color? activeColor = null, Color? inactiveColor = null) {
            ColorActive = activeColor ?? ColorActive;
            ColorInactive = inactiveColor ?? ColorActive;
        }

        public void SetBackgroundImage(Asset<Texture2D> texture) {
            BackgroundTexture = texture;
        }

        public void SetHoverImage(Asset<Texture2D> texture) {
            BorderTexture = texture;
        }

        public void SetImage(Asset<Texture2D> texture, bool changeSize = false) {
            Texture = texture;
            if (changeSize) {
                Width.Set(Texture.Width(), 0f);
                Height.Set(Texture.Height(), 0f);
            }
        }
        #endregion

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (IsMouseHovering) {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch) {
            CalculatedStyle dimensions = GetDimensions();

            var mainColor = IsMouseHovering ? ColorActive : ColorInactive;
            if (DrawColor is not null) {
                mainColor = DrawColor.Invoke();
            }

            if (BackgroundTexture is not null) {
                spriteBatch.Draw(BackgroundTexture.Value, dimensions.Center(), null, mainColor, 0f, BackgroundTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(Texture.Value, dimensions.Center(), null, mainColor, 0f, Texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            if (BorderTexture is not null && IsMouseHovering) {
                spriteBatch.Draw(BorderTexture.Value, dimensions.Center(), null, mainColor * 1.4f, 0f, BorderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            if (IsMouseHovering && !string.IsNullOrEmpty(HoverText)) {
                string text = HoverText;
                if (HoverText.StartsWith("{$") && HoverText.EndsWith("}"))
                {
                    text = Language.GetTextValue(HoverText.Substring("{$".Length, HoverText.Length - "{$}".Length));
                }
                Main.instance.MouseText(text);
            }
        }

        public override void MouseOver(UIMouseEvent evt) {
            base.MouseOver(evt);
            if (PlaySound.HasValue) {
                SoundEngine.PlaySound(PlaySound.Value);
            }
        }

        public override void MouseOut(UIMouseEvent evt) {
            base.MouseOut(evt);
        }
    }
}
