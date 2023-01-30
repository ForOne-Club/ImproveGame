using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUIBackgroundImage : View
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

            Rounded = new Vector4(10f);
            BgColor = UIColor.TitleBg;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 position = GetInnerDimensions().Position();
            Vector2 size = GetInnerDimensions().Size();
            sb.Draw(Texture, position + size / 2 - textureSize / 2f, IsMouseHovering ? Color.White : Color.White * 0.5f);
        }

    }
}
