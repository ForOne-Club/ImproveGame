using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUISwitch : View
    {
        private const float InnerRowSpacing = 8;
        private readonly Func<bool> _getState;
        private readonly Action<bool> _setState;

        private bool State
        {
            get => _getState();
            set => _setState(value);
        }

        private readonly float _textScale;
        private readonly string _text;
        private readonly Vector2 _textSize;
        private readonly Color _textColor = Color.White;
        private readonly Color _textBorderColor = Color.Black;
        private readonly AnimationTimer _timer = new AnimationTimer(4);

        public SUISwitch(Func<bool> getState, Action<bool> setState, string text, float textScale = 1f)
        {
            _text = text;
            _textSize = GetFontSize(text);
            _textScale = textScale;
            _getState = getState;
            _setState = setState;

            PaddingLeft = 12 * textScale;
            PaddingRight = 12 * textScale;
            PaddingTop = 5 * textScale;
            PaddingBottom = 5 * textScale;

            Width.Pixels = (_textSize.X + InnerRowSpacing + 48) * textScale + this.HPadding();
            Height.Pixels = MathF.Max(_textSize.Y * textScale, 26) + this.VPadding();
        }

        public override void Update(GameTime gameTime)
        {
            _timer.Update();
            base.Update(gameTime);
            if (State)
            {
                _timer.TryOpen();
            }
            else
            {
                _timer.TryClose();
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            State = !State;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            Color color = Color.Lerp(UIColor.SwitchBg, UIColor.SwitchBgHover, _timer.Schedule);
            Color color2 = Color.Lerp(UIColor.SwitchBorder, UIColor.SwitchBorderHover, _timer.Schedule);
            Color color3 = Color.Lerp(UIColor.SwitchRound, UIColor.SwitchRoundHover, _timer.Schedule);

            Vector2 position = GetInnerDimensions().Position();
            Vector2 size = GetInnerDimensions().Size();
            Vector2 boxSize = new Vector2(48, 26) * _textScale;

            Vector2 position1 = position + new Vector2(0, size.Y / 2 - boxSize.Y / 2);
            SDFRactangle.HasBorder(position1, boxSize, new Vector4(MathF.Min(boxSize.X, boxSize.Y) / 2), color, 2, color2);

            Vector2 boxSize2 = new(boxSize.Y - 10 * _textScale);
            Vector2 position2 = position + Vector2.Lerp(new Vector2(3 + 2, size.Y / 2 - boxSize2.Y / 2),
                new Vector2(boxSize.X - 3 - 2 - boxSize2.X, size.Y / 2 - boxSize2.Y / 2), _timer.Schedule);
            SDFGraphic.NoBorderRound(position2, boxSize2.X, color3);

            DrawString(
                position + new Vector2(boxSize.X + InnerRowSpacing * _textScale,
                    size.Y / 2 - _textSize.Y * _textScale / 2 + UIConfigs.Instance.GeneralFontOffsetY * _textScale), _text,
                _textColor,
                _textBorderColor, _textScale);
        }
    }
}