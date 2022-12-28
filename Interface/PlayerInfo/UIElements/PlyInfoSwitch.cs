using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseUIEs;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoSwitch : HoverEffect
    {
        public Func<bool> Opened;
        public Texture2D open;
        private Color background;
        public PlyInfoSwitch(Color background)
        {
            this.background = background;
            Height.Pixels = 40f;
            open = GetTexture("UI/PlayerInfo/Open").Value;

            Relative = true;
            Layout = RelativeMode.Horizontal;
            Wrap = true;
            Interval = new Vector2(10f);

            border = true;
            startWidth = 6;
            endWidth = 4;
            endColor = new Color(0, 0, 0, 0.5f);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, round, background);

            if (Opened is not null)
            {
                float rotation;
                if (Opened())
                {
                    rotation = MathF.PI;
                }
                else
                {
                    rotation = 0;
                }
                sb.Draw(open, pos + size / 2, null, Color.White, rotation, open.Size() / 2f, 1f, 0, 0f);
            }
        }
    }
}
