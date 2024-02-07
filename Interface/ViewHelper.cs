namespace ImproveGame.Interface;

public static class ViewHelper
{
    public static View CreateHead(Color bgColor, float height, float rounded)
    {
        var view = new View
        {
            BgColor = bgColor,
            Rounded = new Vector4(rounded, rounded, 0, 0),
            PaddingTop = 1f,
        };
        view.SetPadding(15f, 0f);
        view.Width.Percent = 1f;
        view.Height.Pixels = height;

        return view;
    }

    public static View CreateTail(Color bgColor, float height, float rounded)
    {
        var view = new View
        {
            BgColor = bgColor,
            Rounded = new Vector4(0, 0, rounded, rounded),
            RelativeMode = RelativeMode.Horizontal,
            PreventOverflow = true,
            Spacing = new Vector2(4f),
            PaddingBottom = 1f,
        };
        view.SetPadding(15f, 0f);
        view.Width.Percent = 1f;
        view.Height.Pixels = height;

        return view;
    }
}
