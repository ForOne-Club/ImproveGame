using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.DummyControl;

public class DummyControlGUI : ViewBody
{
    #region ViewBody
    public override bool Display { get => false; set => IsVisible = value; }

    public override bool CanDisableMouse(UIElement target)
    {
        return Window?.IsMouseHovering ?? false;
    }

    public override bool CanPriority(UIElement target)
    {
        return Window?.IsMouseHovering ?? false;
    }
    #endregion

    public static bool IsVisible { get; set; } = true;

    public SUIPanel Window;
    public View ContentView;

    public override void OnInitialize()
    {
#if DEBUG
        Window = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
        {
            Draggable = true
        };
        Window.SetAlign(0.5f, 0.5f);
        Window.SetPadding(0f);
        Window.SetSizePixels(360f, 360f);
        Window.Join(this);

        #region 标题栏
        var titleBar = new View
        {
            DragIgnore = true,
            BgColor = UIColor.TitleBg2,
            Border = 2f,
            BorderColor = UIColor.PanelBorder,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
        };
        titleBar.SetPadding(0);
        titleBar.Width.Precent = 1f;
        titleBar.Height.Pixels = 42f;
        titleBar.Join(Window);

        var title = new SUITitle(GetText("UI.DummyControl.Name"), 0.42f)
        {
            VAlign = 0.5f
        };
        title.SetPadding(15f, 0f, 10f, 0f);
        title.SetInnerPixels(title.TextSize);
        title.Join(titleBar);

        var cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f
        };
        cross.Width.Pixels = 42f;
        cross.Height.Set(0f, 1f);
        cross.Join(titleBar);

        cross.OnLeftMouseDown += (_, _) =>
        {
            RemoveChild(Window);
            SoundEngine.PlaySound(SoundID.MenuClose);
        };
        #endregion

        ContentView = new View();
        ContentView.Top.Pixels = titleBar.BottomPixels();
        ContentView.Width.Percent = 1f;
        ContentView.Height.Pixels = 360f - 42f;
        ContentView.SetPadding(6f, 4f);
        ContentView.Join(Window);

        var uiText = new SUIText();
        uiText.Width.Percent = 1f;
        uiText.Height.Pixels = 100f;
        uiText.SetPadding(8f);
        uiText.SetRoundedRectangleValues(default, 2f, UIColor.PanelBorder, new Vector4(8f));
        uiText.TextOrKey = "[c/0099ff:你好 123] [centeritem/88:3] 提示 123, 提示 123, 提示 123, 提示 123, 提示 123, 提示 123, 提示 123";
        uiText.IsWrapped = true;
        uiText.TextOrigin = new Vector2(0.5f);
        uiText.TextPercentOffset = new Vector2(0.5f);
        uiText.Join(ContentView);
#endif
    }
}
