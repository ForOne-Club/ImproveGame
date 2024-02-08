using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.DummyControl;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Dummy Control")]
public class DummyControlGUI : BaseBody
{
    #region ViewBody
    public override bool Enabled { get => IsVisible; set => IsVisible = value; }

    public override bool CanSetFocusTarget(UIElement target)
    {
        return Window?.IsMouseHovering ?? false;
    }
    #endregion

    public static bool IsVisible { get; set; } = false;

    public SUIPanel Window;
    public View ContentView;

    public override void OnInitialize()
    {
        Window = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Draggable = true
        };
        Window.SetAlign(0.5f, 0.5f);
        Window.SetPadding(0f);
        Window.SetSizePixels(360f, 360f);
        Window.JoinParent(this);

        #region 标题栏
        var titleBar = new View
        {
            DragIgnore = true,
            BgColor = UIStyle.TitleBg2,
            Border = 2f,
            BorderColor = UIStyle.PanelBorder,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
        };
        titleBar.SetPadding(0);
        titleBar.Width.Precent = 1f;
        titleBar.Height.Pixels = 42f;
        titleBar.JoinParent(Window);

        SUIText title = new SUIText
        {
            VAlign = 0.5f,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.DummyControl.Name"
        };
        title.SetPadding(15f, 0f, 10f, 0f);
        title.SetInnerPixels(title.TextSize);
        title.JoinParent(titleBar);

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
        cross.JoinParent(titleBar);

        cross.OnLeftMouseDown += (_, _) =>
        {
            // RemoveChild(Window);
            SoundEngine.PlaySound(SoundID.MenuClose);
        };
        #endregion

        ContentView = new View();
        ContentView.Top.Pixels = titleBar.BottomPixels;
        ContentView.Width.Percent = 1f;
        ContentView.Height.Pixels = 360f - 42f;
        ContentView.SetPadding(6f, 4f);
        ContentView.JoinParent(Window);

        ST = new SUIText()
        {
            TextOrKey = "Hello tML. Hello tML. Hello tML. Hello tML. Hello tML. Hello tML.",
            TextBorderColor = Color.Transparent,
            RelativeMode = RelativeMode.Horizontal,
            IsWrapped = true,
            PreventOverflow = true
        };
        ST.SetRoundedRectProperties(new Color(0, 155, 255) * 0.5f, 2f, Color.Transparent, new Vector4(UIStyle.ItemSlotBorderRound));
        ST.SetPadding(15f, 5f);
        ST.TextAlign = new Vector2(0.5f);
        ST.Width.Precent = 1f;
        ST.Height.Pixels = 100f;
        ST.JoinParent(ContentView);

        ST = new SUIText()
        {
            TextOrKey = "Hello tML.",
            TextBorderColor = Color.Transparent,
            RelativeMode = RelativeMode.Horizontal,
            PreventOverflow = true
        };
        ST.SetRoundedRectProperties(new Color(255, 155, 0) * 0.5f, 2f, Color.Transparent, new Vector4(UIStyle.ItemSlotBorderRound));
        ST.SetPadding(15f, 5f);
        ST.SetInnerPixels(ST.TextSize);
        ST.JoinParent(ContentView);

        ST = new SUIText()
        {
            TextOrKey = "Hello tML.",
            TextBorderColor = Color.Transparent,
            RelativeMode = RelativeMode.Horizontal,
            PreventOverflow = true
        };
        ST.SetRoundedRectProperties(new Color(255, 0, 155) * 0.5f, 2f, Color.Transparent, new Vector4(UIStyle.ItemSlotBorderRound));
        ST.SetPadding(15f, 5f);
        ST.SetInnerPixels(ST.TextSize);
        ST.JoinParent(ContentView);
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
