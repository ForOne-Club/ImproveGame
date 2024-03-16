using ImproveGame.Common.ModPlayers;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI.Autofisher;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Autofisher Free Filter")]
public class FreeFilter : BaseBody
{
    public static FreeFilter Instance { get; private set; }
    public FreeFilter() => Instance = this;

    #region 抽象实现

    public override bool Enabled { get => Visible; set => Visible = value; }
    public static bool Visible { get; set; }

    public override bool CanSetFocusTarget(UIElement target) => Window.IsMouseHovering;

    #endregion

    public SUIPanel Window;
    public View TitleView;
    public SUIText Title;
    public SUICross Cross;
    public FreeFilterItemGrid FreeFilterItemGrid;

    public override void OnInitialize()
    {
        // 窗口
        Window = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            Draggable = true,
            FinallyDrawBorder = true,
            IsAdaptiveHeight = true
        };

        Window.SetPadding(0);
        Window.Width.Pixels = (44f + 4f) * 4f - 4f + 16f + 7f + 6f + 5f;
        Window.SetPosPixels(574, Main.instance.invBottom + 60);
        Window.JoinParent(this);

        #region 标题

        TitleView = new View
        {
            DragIgnore = true,
            BgColor = Color.Lerp(Color.Black, UIStyle.TitleBg, 0.5f) * 0.5f,
            Border = 1.5f,
            BorderColor = Color.Transparent,
            Rounded = new Vector4(12f, 12f, 0f, 0f),
        };
        TitleView.SetPadding(0);
        TitleView.Width.Precent = 1f;
        TitleView.Height.Pixels = 44f;
        TitleView.JoinParent(Window);
        
        Title = new SUIText
        {
            DragIgnore = true,
            IsLarge = true,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.Autofisher.PerItemFilter",
            TextAlign = new Vector2(0f, 0.5f),
            TextScale = 0.45f,
            Height = StyleDimension.Fill,
            Width = StyleDimension.Fill,
            Left = new StyleDimension(16f, 0f)
        };
        Title.SetSizePercent(1f, 1f);
        Title.JoinParent(TitleView);

        Cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Rounded = new Vector4(0f, 12f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f,
            Border = 1.5f,
            BorderColor = Color.Transparent,
            BgColor = Color.Transparent,
        };
        Cross.CrossOffset.X = 1f;
        Cross.Width.Pixels = 46f;
        Cross.Height.Set(0f, 1f);
        Cross.OnUpdate += _ =>
        {
            Cross.BgColor = Cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        };
        Cross.OnLeftMouseDown += (_, _) =>
        {
            Enabled = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        };
        Cross.JoinParent(TitleView);

        #endregion

        FreeFilterItemGrid = new FreeFilterItemGrid
        {
            RelativeMode = RelativeMode.Vertical
        };
        FreeFilterItemGrid.SetPadding(7f, 4f, 6f, 8f);
        FreeFilterItemGrid.Width.Percent = 1f;
        FreeFilterItemGrid.SetInnerPixels(0f, (44f + 4f) * 4f - 4f);
        FreeFilterItemGrid.JoinParent(Window);
    }

    public override void Update(GameTime gameTime)
    {
        if (Window.IsMouseHovering)
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Autofisher Free Filter");
        
        if (AutofishPlayer.LocalPlayer.Autofisher is null)
            Enabled = false;

        base.Update(gameTime);
    }
}