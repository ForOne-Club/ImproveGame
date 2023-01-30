using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 图片按钮
    /// </summary>
    public class SUIPictureButton : HoverView
    {
        private static readonly float IconAndTextSpacing = 6f;
        private readonly Texture2D _texture;
        private readonly string _text;
        private Vector2 _textSize;

        public SUIPictureButton(Texture2D texture, string text)
        {
            _texture = texture;
            _text = text;
            _textSize = FontAssets.MouseText.Value.MeasureString(text);
            SetPadding(18f, 0f);
            SetInnerPixels(_texture.Size().X + _textSize.X + 4 + IconAndTextSpacing, 40f);
            OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            Color borderColor = Color.Lerp(UIColor.PanelBorder, UIColor.ItemSlotBorderFav, hoverTimer.Schedule);
            PixelShader.RoundedRectangle(pos, size, 10, UIColor.ButtonBg, 2, borderColor);

            Vector2 innerPos = GetInnerDimensions().Position();
            Vector2 innerSize = GetInnerDimensionsSize();

            Vector2 texturePos = innerPos + new Vector2(0, (innerSize.Y - _texture.Size().Y) / 2);
            spriteBatch.Draw(_texture, texturePos, Color.White);

            // Because border is 2, so: X + 2.
            Vector2 textPos = innerPos + new Vector2(_texture.Size().X + 2 + IconAndTextSpacing, (innerSize.Y - _textSize.Y) / 2);
            textPos.Y += UIConfigs.Instance.GeneralFontOffsetY;
            TrUtils.DrawBorderString(spriteBatch, _text, textPos, Color.White);
        }
    }
}
