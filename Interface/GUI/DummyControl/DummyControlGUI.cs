using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.DummyControl;

public class DummyControlGUI : ViewBody
{
    #region ViewBody
    public override bool Display { get => IsVisible; set => IsVisible = value; }

    public override bool CanDisableMouse(UIElement target)
    {
        return Window.IsMouseHovering;
    }

    public override bool CanPriority(UIElement target)
    {
        return Window.IsMouseHovering;
    }
    #endregion

    public static bool IsVisible { get; set; } = true;

    public SUIPanel Window;

    public override void OnInitialize()
    {
        Window = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg);
        Window.Draggable = true;
        Window.SetAlign(0.5f, 0.5f);
        Window.SetPadding(0f);
        Window.SetSizePixels(360f, 480f);
        Window.Join(this);

        #region 标题栏
        var TitleView = new View
        {
            DragIgnore = true,
            BgColor = UIColor.TitleBg2,
            Border = 2f,
            BorderColor = UIColor.PanelBorder,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
        };
        TitleView.SetPadding(0);
        TitleView.Width.Precent = 1f;
        TitleView.Height.Pixels = 42f;
        TitleView.Join(Window);

        var Title = new SUITitle(GetText("UI.PlayerStats.Control"), 0.42f)
        {
            VAlign = 0.5f
        };
        Title.SetPadding(15f, 0f, 10f, 0f);
        Title.SetInnerPixels(Title.TextSize);
        Title.Join(TitleView);

        var Cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f
        };
        Cross.Width.Pixels = 42f;
        Cross.Height.Set(0f, 1f);
        Cross.Join(TitleView);

        Cross.OnLeftMouseDown += (_, _) =>
        {
            RemoveChild(Window);
            SoundEngine.PlaySound(SoundID.MenuClose);
        };
        #endregion
    }
}
