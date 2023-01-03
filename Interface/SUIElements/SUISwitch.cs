using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUISwitch : View
    {
        private const float InnerRowSpacing = 8;
        public Func<bool> GetState;
        public Action<bool> SetState;
        public bool State
        {
            get => GetState();
            set => SetState(value);
        }
        private string text;
        public float textScale;
        private Vector2 textSize;
        public Color textColor = Color.White;
        public Color textBorderColor = Color.Black;
        private readonly AnimationTimer timer = new(4);

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textSize = MouseTextSize(value);
            }
        }

        public SUISwitch(Func<bool> GetState, Action<bool> SetState, string text, float textScale = 1f)
        {
            Text = text;
            this.textScale = textScale;
            this.GetState = GetState;
            this.SetState = SetState;

            PaddingLeft = 12 * textScale;
            PaddingRight = 12 * textScale;
            PaddingTop = 5 * textScale;
            PaddingBottom = 5 * textScale;

            Width.Pixels = (textSize.X + InnerRowSpacing + 48) * textScale + this.HPadding();
            Height.Pixels = MathF.Max(textSize.Y * textScale, 26) + this.VPadding();
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update();
            base.Update(gameTime);
            if (State)
            {
                timer.TryOpen();
            }
            else
            {
                timer.TryClose();
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            State = !State;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Color color = Color.Lerp(UIColor.SwitchBg, UIColor.SwitchBgHover, timer.Schedule);
            Color color2 = Color.Lerp(UIColor.SwitchBorder, UIColor.SwitchBorderHover, timer.Schedule);
            Color color3 = Color.Lerp(UIColor.SwitchRound, UIColor.SwitchRoundHover, timer.Schedule);

            Vector2 position = GetInnerDimensions().Position();
            Vector2 size = GetInnerDimensions().Size();
            Vector2 boxSize = new Vector2(48, 26) * textScale;

            Vector2 position1 = position + new Vector2(0, size.Y / 2 - boxSize.Y / 2);
            PixelShader.DrawRoundRect(position1, boxSize, MathF.Min(boxSize.X, boxSize.Y) / 2, color, 3, color2);

            Vector2 boxSize2 = new(boxSize.Y - 10 * textScale);
            Vector2 position2 = position + Vector2.Lerp(new Vector2(3 + 2, size.Y / 2 - boxSize2.Y / 2), new Vector2(boxSize.X - 3 - 2 - boxSize2.X, size.Y / 2 - boxSize2.Y / 2), timer.Schedule);
            PixelShader.DrawRound(position2, boxSize2.X, color3);

            DrawString(position + new Vector2(boxSize.X + InnerRowSpacing * textScale, size.Y / 2 - textSize.Y * textScale / 2 + UIConfigs.Instance.TextDrawOffsetY * textScale), text, Color.White,
                textBorderColor, textScale);
        }
    }
}
