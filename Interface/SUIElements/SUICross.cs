using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements;

public class SUICross : TimerView
{
    public float CrossSize, CrossRounded, CrossBorder;
    public Color CrossBorderColor, CrossBorderHoverColor, CrossBeginColor, CrossEndColor;

    public SUICross()
    {
        SetSizePixels(50f, 50f);

        CrossSize = 24f;
        CrossRounded = UIStyle.CrossThickness;
        CrossBeginColor = UIStyle.Cross * 0.5f;
        CrossEndColor = UIStyle.Cross;
        CrossBorder = UIStyle.CrossBorderSize;
        CrossBorderColor = UIStyle.PanelBorder;
        CrossBorderHoverColor = UIStyle.ItemSlotBorderFav;

        // MarginRight = 2f;
        Rounded = new Vector4(10f);
        Border = 2;
        BorderColor = UIStyle.PanelBorder;
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        var borderColor = HoverTimer.Lerp(CrossBorderColor, CrossBorderHoverColor);
        base.DrawSelf(sb);
        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();
        Color fork = HoverTimer.Lerp(CrossBeginColor, CrossEndColor);
        Vector2 forkPos = pos + (size - new Vector2(CrossSize)) / 2f;
        SDFGraphic.HasBorderCross(forkPos, CrossSize, CrossRounded, fork, CrossBorder, borderColor);
    }
}
