using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUICross : TimerView
    {
        public float CrossSize, CrossRounded, CrossBorder;
        public Color CrossBorderColor, CrossBeginColor, CrossEndColor;

        public SUICross()
        {
            SetSizePixels(50f, 50f);

            CrossSize = 24f;
            CrossRounded = 4.6f;
            CrossBeginColor = UIColor.Cross * 0.5f;
            CrossEndColor = UIColor.Cross;
            CrossBorder = 2;
            CrossBorderColor = UIColor.PanelBorder;

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
            CrossBorderColor = HoverTimer.Lerp(UIColor.PanelBorder, UIColor.ItemSlotBorderFav);
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Color fork = HoverTimer.Lerp(CrossBeginColor, CrossEndColor);
            Vector2 forkPos = pos + (size - new Vector2(CrossSize)) / 2f;
            SDFGraphic.DrawCross(forkPos, CrossSize, CrossRounded, fork, CrossBorder, CrossBorderColor);
        }
    }
}
