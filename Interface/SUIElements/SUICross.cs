using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUICross : HoverView
    {
        public float forkSize;
        public float crossRound;
        public float crossBorder;
        public Color beginBg, endBg, crossColor;

        public SUICross(float forkSize)
        {
            crossRound = 4.5f;
            this.crossBorder = 2;
            this.forkSize = forkSize;
            Width.Pixels = 50;
            Height.Pixels = 50;
            crossColor = UIColor.Cross;
            beginBg = UIColor.TitleBg * 0.5f;
            endBg = UIColor.TitleBg;

            Rounded = new Vector4(10f);
            Border = 2;
            BorderColor = UIColor.PanelBorder;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            BgColor = Color.Lerp(beginBg, endBg, hoverTimer.Schedule);
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Color fork = Color.Lerp(Color.Transparent, crossColor, hoverTimer.Schedule);
            Vector2 forkPos = pos + (size - new Vector2(forkSize)) / 2f;
            SDFGraphic.DrawCross(forkPos, forkSize, crossRound, fork, crossBorder, BorderColor);
        }
    }
}
