using ImproveGame.Content.Items;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI.LiquidWandUI;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Liquid Wand UI")]
public class LiquidWandUI : BaseBody
{
    public static LiquidWandUI Instance { get; private set; }
    public LiquidWandUI() => Instance = this;
    public override bool Enabled { get => Visible; set => Visible = value; }
    public static bool Visible { get; private set; }

    public override bool CanSetFocusTarget(UIElement target)
        => (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;

    // 主面板
    public SUIPanel MainPanel;

    // 标题面板
    private View TitlePanel;

    // 液体按钮
    private AbsorbElement AbsorbButton;
    private LiquidElement WaterButton;
    private LiquidElement LavaButton;
    private LiquidElement HoneyButton;
    private LiquidElement ShimmerButton;

    private LiquidWand _wand;

    public override void OnInitialize()
    {
        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Rounded = new Vector4(10f),
            Shaded = true,
            Draggable = true,
            FinallyDrawBorder = true
        };
        MainPanel.SetPadding(1.5f);
        MainPanel.SetPosPixels(590, 120)
            .SetSizePixels(260, 250)
            .JoinParent(this);

        TitlePanel = ViewHelper.CreateHead(Color.Black * 0.25f, 45f, 10f);
        TitlePanel.SetPadding(0f);
        TitlePanel.JoinParent(MainPanel);

        // 标题
        var title = new SUIText
        {
            IsLarge = true,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.LiquidWandUI.Name",
            TextAlign = new Vector2(0f, 0.5f),
            TextScale = 0.45f,
            Height = StyleDimension.Fill,
            Width = StyleDimension.Fill,
            DragIgnore = true,
            Left = new StyleDimension(16f, 0f)
        };
        title.JoinParent(TitlePanel);

        var cross = new SUICross
        {
            HAlign = 1f,
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f,
            Border = 0f,
            BorderColor = Color.Transparent,
            BgColor = Color.Transparent,
        };
        cross.CrossOffset.X = 1f;
        cross.Width.Pixels = 46f;
        cross.Height.Set(0f, 1f);
        cross.OnUpdate += _ =>
        {
            cross.BgColor = cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        };
        cross.OnLeftMouseDown += (_, _) => Close();
        cross.JoinParent(TitlePanel);

        AbsorbButton = new AbsorbElement();
        AbsorbButton.JoinParent(MainPanel);

        WaterButton = new LiquidElement(LiquidID.Water);
        WaterButton.JoinParent(MainPanel);

        LavaButton = new LiquidElement(LiquidID.Lava);
        LavaButton.JoinParent(MainPanel);

        HoneyButton = new LiquidElement(LiquidID.Honey);
        HoneyButton.JoinParent(MainPanel);

        ShimmerButton = new LiquidElement(LiquidID.Shimmer);
        ShimmerButton.Infinite = true;
        ShimmerButton.JoinParent(MainPanel);

        // 这里这个是必要的，因为开了毛玻璃调整光照会运行OnInitalize
        ValidateWand();
    }

    public override void Update(GameTime gameTime)
    {
        ValidateWand();

        base.Update(gameTime);

        if (!MainPanel.IsMouseHovering)
            return;

        Main.LocalPlayer.mouseInterface = true;
    }

    private void ValidateWand()
    {
        if (_wand == null)
        {
            Enabled = false;
            return;
        }

        if (_wand.IsAdvancedWand && (ShimmerButton.Hide || MainPanel.Height.Pixels != 300))
        {
            WaterButton.Infinite = true;
            LavaButton.Infinite = true;
            HoneyButton.Infinite = true;

            MainPanel.Height.Pixels = 300;
            ShimmerButton.Hide = false;
            Recalculate();
        }

        if (!_wand.IsAdvancedWand && (!ShimmerButton.Hide || MainPanel.Height.Pixels != 250))
        {
            WaterButton.Infinite = false;
            LavaButton.Infinite = false;
            HoneyButton.Infinite = false;

            MainPanel.Height.Pixels = 250;
            ShimmerButton.Hide = true;
            Recalculate();
        }
    }

    public void Open(LiquidWand wand)
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Visible = true;

        _wand = wand;
        ValidateWand();
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Visible = false;
    }
}