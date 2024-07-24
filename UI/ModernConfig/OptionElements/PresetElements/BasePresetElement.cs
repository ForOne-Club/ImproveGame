using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.UI.ModernConfig.OptionElements.PresetElements;

public class BasePresetElement : TimerView
{
    public string Tooltip;

    public BasePresetElement(string label, string tooltip, bool hasLabelElement = true)
    {
        Tooltip = tooltip;
        Width.Set(0f, 1f);
        Height.Set(46f, 0f);
        Rounded = new Vector4(12f);
        SetPadding(12, 4);

        RelativeMode = RelativeMode.Vertical;
        OverflowHidden = true;

        if (!hasLabelElement)
            return;

        var labelElement = new SlideText(label);
        labelElement.JoinParent(this);
    }

    // 为了让UI之间实际上无间隔，防止鼠标滑过时Tooltip文字闪现，这里重写绘制，而不使用Spacing
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        var position = dimensions.Position();
        var size = dimensions.Size();

        // 这里修改这两个值，而不使用Spacing
        position.Y += 3f;
        size.Y -= 6f;

        // 背景板
        var panelColor = HoverTimer.Lerp(UIStyle.PanelBgLight, UIStyle.PanelBgLightHover);

        SDFRectangle.NoBorder(position, size, new Vector4(8f), panelColor * 0.8f);

        // 提示
        if (IsMouseHovering)
        {
            TooltipPanel.SetText(Tooltip);
        }
    }
}