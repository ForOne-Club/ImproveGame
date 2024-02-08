using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UIFramework;

public static class ViewHelper
{
    public static View CreateHead(Color bgColor, float height, float rounded)
    {
        var view = new View
        {
            BgColor = bgColor,
            Rounded = new Vector4(rounded, rounded, 0, 0),
            DragIgnore = true,
        };
        view.SetPadding(15f, 0f);
        view.Width.Percent = 1f;
        view.Height.Pixels = height;

        return view;
    }

    public static View CreateTail(Color bgColor, float height, float rounded, float spacing = 4f)
    {
        var view = new View
        {
            BgColor = bgColor,
            Rounded = new Vector4(0, 0, rounded, rounded),
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(spacing),
            PaddingBottom = 1f,
            DragIgnore = true,
        };
        view.SetPadding(15f, 0f);
        view.Width.Percent = 1f;
        view.Height.Pixels = height;

        return view;
    }
}
