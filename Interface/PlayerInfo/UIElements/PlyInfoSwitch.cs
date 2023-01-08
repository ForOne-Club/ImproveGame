using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoSwitch : HoverView
    {
        private readonly Func<bool> _expanded;
        private readonly Texture2D _openIcon;
        private readonly Color _background, _border;
        public PlyInfoSwitch(Color background, Color border, Func<bool> expanded)
        {
            this._background = background;
            this._border = border;
            this._expanded = expanded;
            Height.Pixels = Width.Pixels = 50f;
            _openIcon = GetTexture("UI/PlayerInfo/Open").Value;

            Relative = RelativeMode.Horizontal;
            Wrap = true;
            RoundMode = RoundMode.Round4;
            round4 = new Vector4(10f, 0, 0, 0);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position() - new Vector2(Extension.X, Extension.Y);
            Vector2 size = GetDimensions().Size() + new Vector2(Extension.X + Extension.Z, Extension.Y + Extension.W);
            PixelShader.DrawRoundRect(pos, size, round4, _background, 2, _border);

            if (_expanded is not null)
            {
                float rotation = _expanded() ? MathF.PI : 0;
                sb.Draw(_openIcon, pos + size / 2, null, Color.White, rotation, _openIcon.Size() / 2f, 1f, 0, 0f);
            }
        }
    }
}
