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

    public static void MakeHorizontalSeparator(this View parent, int widthOffset = -16, int height = 10, int spacing = 6)
    {
        View separatorArea = new()
        {
            Height = new StyleDimension(height, 0f),
            Width = new StyleDimension(widthOffset, 1f),
            HAlign = 0.5f,
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(0, spacing)
        };
        separatorArea.JoinParent(parent);
        separatorArea.Append(new UIHorizontalSeparator
        {
            Width = StyleDimension.FromPercent(1f),
            Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
        });
    }

    public static void MakeVerticalSeparator(this View parent, int heightOffset = -16, int width = 10, int spacing = 6)
    {
        View separatorArea = new()
        {
            Height = new StyleDimension(heightOffset, 1f),
            Width = new StyleDimension(width, 0f),
            VAlign = 0.5f,
            DragIgnore = true,
            RelativeMode = RelativeMode.Horizontal,
            Spacing = new Vector2(spacing, 0)
        };
        separatorArea.JoinParent(parent);
        separatorArea.Append(new UIVerticalSeparator
        {
            HAlign = 0.5f,
            Width = StyleDimension.FromPixels(1f),
            Height = StyleDimension.FromPercent(1f),
            Color = new Color(30, 45, 84) * 0.9f
        });
    }
}
