using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.UI;

namespace ImproveGame.Interface.UIElements
{
    public class ModImageButton : UIElement
    {
        private Asset<Texture2D> _texture;
        private Color _colorActive;
        private Color _colorInactive;
        private SoundStyle? _playSound = null;
        private Asset<Texture2D> _borderTexture;
        private Asset<Texture2D> _backgroundTexture;

        public ModImageButton(Asset<Texture2D> texture, Color activeColor = default, Color inactiveColor = default) {
            _texture = texture;
            Width.Set(_texture.Width(), 0f);
            Height.Set(_texture.Height(), 0f);
            _colorActive = activeColor;
            _colorInactive = inactiveColor;
        }

        #region 各种设置方法
        public void SetCenter(int x, int y) {
            Left.Set(x - _texture.Width() / 2, 0f);
            Top.Set(y - _texture.Height() / 2, 0f);
        }

        public void SetSound(SoundStyle sound) {
            _playSound = sound;
        }

        public void SetColor(Color? activeColor = null, Color? inactiveColor = null) {
            _colorActive = activeColor ?? _colorActive;
            _colorInactive = inactiveColor ?? _colorActive;
        }

        public void SetBackgroundImage(Asset<Texture2D> texture) {
            _backgroundTexture = texture;
        }

        public void SetHoverImage(Asset<Texture2D> texture) {
            _borderTexture = texture;
        }

        public void SetImage(Asset<Texture2D> texture) {
            _texture = texture;
            Width.Set(_texture.Width(), 0f);
            Height.Set(_texture.Height(), 0f);
        }
        #endregion

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (IsMouseHovering) {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public Func<Color> DrawColor;

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            CalculatedStyle dimensions = GetDimensions();

            var mainColor = IsMouseHovering ? _colorActive : _colorInactive;
            if (DrawColor is not null) {
                mainColor = DrawColor.Invoke();
            }

            if (_backgroundTexture != null) {
                spriteBatch.Draw(_backgroundTexture.Value, dimensions.Center(), null, mainColor, 0f, _backgroundTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(_texture.Value, dimensions.Center(), null, mainColor, 0f, _texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            if (_borderTexture != null && IsMouseHovering) {
                spriteBatch.Draw(_borderTexture.Value, dimensions.Center(), null, mainColor * 1.4f, 0f, _borderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void MouseOver(UIMouseEvent evt) {
            base.MouseOver(evt);
            if (_playSound.HasValue) {
                SoundEngine.PlaySound(_playSound.Value);
            }
        }

        public override void MouseOut(UIMouseEvent evt) {
            base.MouseOut(evt);
        }
    }
}
