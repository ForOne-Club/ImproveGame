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
            Rounded = new Vector4(10f, 0, 0, 0);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position() ;
            Vector2 size = GetDimensions().Size() ;
            PixelShader.RoundedRectangle(pos, size, Rounded, _background, 2, _border);

            if (_expanded is not null)
            {
                float rotation = _expanded() ? MathF.PI : 0;
                sb.Draw(_openIcon, pos + size / 2, null, Color.White, rotation, _openIcon.Size() / 2f, 1f, 0, 0f);
            }
        }
    }
}
