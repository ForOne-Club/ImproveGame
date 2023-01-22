using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUIBackgroundImage : UIElement
    {
        private Texture2D texture;
        private Vector2 textureSize;
        public Texture2D Texture
        {
            get => texture;
            set
            {
                texture = value;
                textureSize = value.Size();
            }
        }

        public SUIBackgroundImage(Texture2D texture)
        {
            Texture = texture;
            PaddingLeft = 14;
            PaddingRight = 14;
            PaddingTop = 5;
            PaddingBottom = 5;
            Width.Pixels = textureSize.X + this.HPadding();
            Height.Pixels = textureSize.Y + this.VPadding();
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle rectangle = GetDimensions();
            Vector2 position = rectangle.Position();
            Vector2 size = rectangle.Size();

            PixelShader.RoundedRectangle(position, size, 10, UIColor.TitleBg);

            rectangle = GetInnerDimensions();
            position = rectangle.Position();
            size = rectangle.Size();
            sb.Draw(Texture, position + size / 2 - textureSize / 2f, IsMouseHovering ? Color.White : Color.White * 0.5f);
        }

    }
}
