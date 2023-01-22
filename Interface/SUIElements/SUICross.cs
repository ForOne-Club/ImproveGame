using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUICross : HoverView
    {
        public float forkSize;
        public float crossRound;
        public float border, crossBorder;
        public Color beginBg, endBg, borderColor, crossColor;

        public SUICross(float forkSize)
        {
            crossRound = 4.5f;
            this.border = 2;
            this.crossBorder = 2;
            this.forkSize = forkSize;
            Width.Pixels = 50;
            Height.Pixels = 50;
            borderColor = UIColor.PanelBorder;
            crossColor = UIColor.Cross;
            beginBg = UIColor.TitleBg * 0.5f;
            endBg = UIColor.TitleBg;
            Round = 10f;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            Color background = Color.Lerp(beginBg, endBg, hoverTimer.Schedule);
            Color fork = Color.Lerp(Color.Transparent, crossColor, hoverTimer.Schedule);

            switch (RoundMode)
            {
                case RoundMode.Round:
                    PixelShader.RoundedRectangle(pos, size, Round, background, border, borderColor);
                    break;
                case RoundMode.Round4:
                    PixelShader.RoundedRectangle(pos, size, Round4, background, border, borderColor);
                    break;
            }
            Vector2 forkPos = pos + size / 2 - new Vector2(forkSize / 2);
            PixelShader.DrawCross(forkPos, forkSize, crossRound, fork, crossBorder, borderColor);
        }
    }
}
