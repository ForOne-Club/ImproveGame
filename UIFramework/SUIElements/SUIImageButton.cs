using Terraria.ModLoader.UI;

namespace ImproveGame.UIFramework.SUIElements;

public class SUIImageButton (Texture2D texture, string hoverText = "", bool setSizeViaTexture = true)
    : SUIImage(texture, setSizeViaTexture)
{
    public bool UseBorderedMouseText = false;

    public override void DrawSelf(SpriteBatch sb)
    {
        var imageColor = ImageColor;
        ImageColor = HoverTimer.Lerp(imageColor * 0.4f, imageColor);

        base.DrawSelf(sb);

        ImageColor = imageColor;

        if (IsMouseHovering)
        {
            if (UseBorderedMouseText)
                UICommon.TooltipMouseText(hoverText);
            else
                Main.instance.MouseText(hoverText);
        }
    }
}