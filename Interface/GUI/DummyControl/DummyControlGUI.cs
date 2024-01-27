using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.DummyControl;

public class DummyControlGUI : BaseBody
{
    #region ViewBody
    public override bool Enabled { get => IsVisible; set => IsVisible = value; }

    public override bool CanDisableMouse(UIElement target)
    {
        return Window?.IsMouseHovering ?? false;
    }

    public override bool CanPriority(UIElement target)
    {
        return Window?.IsMouseHovering ?? false;
    }
    #endregion

    public static bool IsVisible { get; set; } = false;

    public SUIPanel Window;
    public View ContentView;

    public override void OnInitialize()
    {
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

        SUIText title = new SUIText
        {
            VAlign = 0.5f,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.DummyControl.Name"
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
            // RemoveChild(Window);
            SoundEngine.PlaySound(SoundID.MenuClose);
        };
        #endregion

        ContentView = new View();
        ContentView.Top.Pixels = titleBar.BottomPixels();
        ContentView.Width.Percent = 1f;
        ContentView.Height.Pixels = 360f - 42f;
        ContentView.SetPadding(6f, 4f);
        ContentView.Join(Window);

        ST = new SUIText()
        {
            TextOrKey = "Hello tML. Hello tML. Hello tML. Hello tML. Hello tML. Hello tML.",
            TextBorderColor = Color.Transparent,
            Relative = RelativeMode.Horizontal,
            IsWrapped = true,
            Wrap = true
        };
        ST.SetRoundedRectangleValues(new Color(0, 155, 255) * 0.5f, 2f, Color.Transparent, new Vector4(UIColor.ItemSlotBorderRound));
        ST.SetPadding(15f, 5f);
        ST.TextAlign = new Vector2(0.5f);
        ST.Width.Precent = 1f;
        ST.Height.Pixels = 100f;
        ST.Join(ContentView);

        ST = new SUIText()
        {
            TextOrKey = "Hello tML.",
            TextBorderColor = Color.Transparent,
            Relative = RelativeMode.Horizontal,
            Wrap = true
        };
        ST.SetRoundedRectangleValues(new Color(255, 155, 0) * 0.5f, 2f, Color.Transparent, new Vector4(UIColor.ItemSlotBorderRound));
        ST.SetPadding(15f, 5f);
        ST.SetInnerPixels(ST.TextSize);
        ST.Join(ContentView);

        ST = new SUIText()
        {
            TextOrKey = "Hello tML.",
            TextBorderColor = Color.Transparent,
            Relative = RelativeMode.Horizontal,
            Wrap = true
        };
        ST.SetRoundedRectangleValues(new Color(255, 0, 155) * 0.5f, 2f, Color.Transparent, new Vector4(UIColor.ItemSlotBorderRound));
        ST.SetPadding(15f, 5f);
        ST.SetInnerPixels(ST.TextSize);
        ST.Join(ContentView);
    }

    /// <summary>
    /// 第一个文本
    /// </summary>
    public SUIText ST;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}
