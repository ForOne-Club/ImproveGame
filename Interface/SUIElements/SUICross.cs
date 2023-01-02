using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUICross : HoverEffect
    {
        public float forkSize;
        public float radius;
        public float border;

        public SUICross(float forkSize)
        {
            radius = 3.7f;
            border = 2;
            this.forkSize = forkSize;
            Width.Pixels = forkSize + 20;
            Height.Pixels = forkSize + 10;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Color background = Color.Lerp(UIColor.TitleBg * 0.5f, UIColor.TitleBg * 1f, hoverTimer.Schedule);
            Color fork = Color.Lerp(Color.Transparent, UIColor.Cross, hoverTimer.Schedule);

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 10f, background, 3f, UIColor.PanelBorder);
            Vector2 forkPos = pos + size / 2 - new Vector2(forkSize / 2);
            PixelShader.DrawCross(forkPos, forkSize, radius, fork, border, UIColor.PanelBorder);
        }
    }
}
