using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.ExtremeStorage
{
    public class LongSwitch : View
    {
        private const float InnerRowSpacing = 8;
        private readonly Func<bool> _getState;
        private readonly Action<bool> _setState;

        private bool State
        {
            get => _getState();
            set => _setState(value);
        }

        private readonly string _text;
        private readonly Vector2 _textSize;
        private readonly Color _textColor = Color.White;
        private readonly Color _textBorderColor = Color.Black;
        private readonly AnimationTimer _timer = new (4);

        public LongSwitch(Func<bool> getState, Action<bool> setState, string text)
        {
            _text = text;
            _textSize = GetFontSize(text);
            this._getState = getState;
            this._setState = setState;
            
            Spacing = new Vector2(0f, 4f);

            Width.Set(0f, 1f);
            Height.Set(40f, 0f);
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

            var dimensions = GetDimensions();
            var position = dimensions.Position();
            var center = dimensions.Center();
            var size = dimensions.Size();

            // 背景板
            var panelColor = IsMouseHovering ? UIColor.PanelBgLightHover : UIColor.PanelBgLight;
            SDFRactangle.NoBorder(position, size, new Vector4(8f), panelColor);

            // 开关
            var boxSize = new Vector2(48, 26);
            var boxPosition = new Vector2(position.X + size.X - boxSize.X - 6f, position.Y);
            
            Vector2 position1 = boxPosition + new Vector2(0, size.Y / 2 - boxSize.Y / 2);
            SDFRactangle.HasBorder(position1, boxSize, new Vector4(MathF.Min(boxSize.X, boxSize.Y) / 2), color, 2, color2);

            Vector2 boxSize2 = new(boxSize.Y - 10);
            Vector2 position2 = boxPosition + Vector2.Lerp(new Vector2(3 + 2, size.Y / 2 - boxSize2.Y / 2),
                new Vector2(boxSize.X - 3 - 2 - boxSize2.X, size.Y / 2 - boxSize2.Y / 2), _timer.Schedule);
            SDFGraphic.DrawRound(position2, boxSize2.X, color3);

            // 文字
            var text = GetText($"{_text}.Label");
            var textCenter = new Vector2(position.X + 12, center.Y - _textSize.Y / 2f + UIConfigs.Instance.GeneralFontOffsetY);
            DrawString(textCenter, text, _textColor, _textBorderColor);

            // 提示
            if (IsMouseHovering)
            {
                Main.instance.MouseText(GetText($"{_text}.Tooltip"));
            }
        }
    }
}