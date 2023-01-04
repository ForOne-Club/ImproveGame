using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoSwitch : HoverView
    {
        public Func<bool> IsOpen;
        public Texture2D OpenIcon;
        public Color background, border;
        public PlyInfoSwitch(Color background, Color border)
        {
            this.background = background;
            this.border = border;
            Height.Pixels = 46f;
            OpenIcon = GetTexture("UI/PlayerInfo/Open").Value;

            Relative = RelativeMode.Horizontal;
            Wrap = true;
            round = 10f;
            RoundMode = RoundMode.Round4;
            round4 = new Vector4(0, 0, 10f, 10f);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position() - new Vector2(Extension.X, Extension.Y);
            Vector2 size = GetDimensions().Size() + new Vector2(Extension.X + Extension.Z, Extension.Y + Extension.W);
            PixelShader.DrawRoundRect(pos, size, round4, background, 3, border);

            if (IsOpen is not null)
            {
                float rotation = IsOpen() ? MathF.PI : 0;
                sb.Draw(OpenIcon, pos + size / 2, null, Color.White, rotation, OpenIcon.Size() / 2f, 1f, 0, 0f);
            }
        }
    }
}
