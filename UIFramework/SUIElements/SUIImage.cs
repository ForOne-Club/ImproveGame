using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UIFramework.SUIElements
{
    public class SUIImage : TimerView
    {
        public Texture2D Texture { get; set; }
        public Vector2 ImagePosition = new Vector2();
        public Vector2 ImagePercent = new Vector2();
        public Vector2 ImageAlign = new Vector2();
        public Vector2 ImageOrigin = new Vector2();
        public float ImageScale = 1f;
        public Color ImageColor = Color.White;
        public bool TickSound = true;

        public Rectangle? SourceRectangle { get; set; } = null;

        public SUIImage(Texture2D texture, bool setSizeViaTexture = true)
        {
            Texture = texture;

            if (setSizeViaTexture && Texture != null)
            {
                Width.Pixels = Texture.Width + this.HPadding();
                Height.Pixels = Texture.Height + this.VPadding();
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            if (TickSound)
                SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);

            if (Texture != null)
            {
                Vector2 position = GetDimensions().Position();
                Vector2 size = GetDimensions().Size();

                Vector2 imagePosition = position + ImagePosition + size * ImagePercent;
                imagePosition += (size - Texture.Size()) * ImageAlign;

                sb.Draw(Texture, imagePosition, SourceRectangle, ImageColor,
                    0f, Texture.Size() * ImageOrigin, ImageScale, 0f, 0f);
            }
        }

    }
}
