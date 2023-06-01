using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUIBackgroundImage : View
    {
        private Texture2D _texture;

        public Texture2D Texture
        {
            get => _texture;
            set
            {
                _texture = value;
            }
        }

        public Color Color;

        public SUIBackgroundImage(Texture2D texture)
        {
            Texture = texture;
            PaddingLeft = 14;
            PaddingRight = 14;
            PaddingTop = 5;
            PaddingBottom = 5;
            Width.Pixels = texture.Width + this.HPadding();
            Height.Pixels = texture.Height + this.VPadding();

            Rounded = new Vector4(10f);
            BgColor = UIColor.TitleBg;
            Color = Color.White;
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
            sb.Draw(Texture, position + size / 2 - Texture.Size() / 2f, Color);
        }

    }
}
