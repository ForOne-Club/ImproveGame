using ImproveGame.Content.Functions;
using ImproveGame.Packets.WorldFeatures;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace ImproveGame.UI.WorldFeature;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "World Feature GUI")]
public class WorldFeatureGUI : BaseBody
{
    public static WorldFeatureGUI Instance { get; private set; }

    public WorldFeatureGUI() => Instance = this;

    public override bool IsNotSelectable => StartTimer.AnyClose;

    public override bool Enabled
    {
        get => StartTimer.Closing || _enabled;
        set => _enabled = value;
    }

    private bool _enabled;

    public override bool CanSetFocusTarget(UIElement target)
        => (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;

    /// <summary>
    /// 启动关闭动画计时器
    /// </summary>
    public AnimationTimer StartTimer = new(3);

    // 主面板
    public SUIPanel MainPanel;

    public override void OnInitialize()
    {
        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            Draggable = true
        };
        MainPanel.SetPadding(0f);
        MainPanel.SetPosPixels(250, 280)
            .SetSizePixels(300, 306)
            .JoinParent(this);

        var titlePanel = ViewHelper.CreateHead(Color.Black * 0.25f, 45f, 10f);
        titlePanel.SetPadding(0f);
        titlePanel.JoinParent(MainPanel);

        // 标题
        var title = new SUIText
        {
            IsLarge = true,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.WorldFeature.Title",
            TextAlign = new Vector2(0f, 0.5f),
            TextScale = 0.45f,
            Height = StyleDimension.Fill,
            Width = StyleDimension.Fill,
            DragIgnore = true,
            Left = new StyleDimension(16f, 0f)
        };
        title.JoinParent(titlePanel);

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
            cross.BgColor = cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        cross.OnLeftMouseDown += (_, _) => Close();
        cross.JoinParent(titlePanel);

        var contentPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical
        };
        contentPanel.SetPadding(16, 14, 16, 14);
        contentPanel.SetSize(0f, 0f, 1f, 1f);
        contentPanel.JoinParent(MainPanel);

        var festivalPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(0f, 6f)
        };
        festivalPanel.SetSize(0f, 100f, 1f, 0f);
        festivalPanel.JoinParent(contentPanel);

        var seedPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical
        };
        seedPanel.SetSize(0f, -100f, 1f, 1f);
        seedPanel.JoinParent(contentPanel);

        SetupFestivalButtons(festivalPanel);
        SetupSeedButtons(seedPanel);
    }

    private void SetupFestivalButtons(UIElement parent)
    {
        var icon = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Tags_Shadow");

        var halloweenSwitch = new LongSwitch(
            () => ForceFestivalSystem.ForceHalloween,
            FestivalPacket.SetHalloween,
            "UI.WorldFeature.Halloween")
        {
            ResetAnotherPosition = true,
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(0f, 4f)
        };
        halloweenSwitch.SetIcon(icon, new SpriteFrame(16, 5, 13, 2));
        halloweenSwitch.JoinParent(parent);

        var xMasSwitch = new LongSwitch(
            () => ForceFestivalSystem.ForceXMas,
            FestivalPacket.SetXMas,
            "UI.WorldFeature.XMas")
        {
            RelativeMode = RelativeMode.Vertical
        };
        xMasSwitch.SetIcon(icon, new SpriteFrame(16, 5, 14, 2));
        xMasSwitch.JoinParent(parent);
    }

    private void SetupSeedButtons(UIElement parent)
    {
        parent.Append(new SeedButton(SeedType.Drunk, 0, 0));
        parent.Append(new SeedButton(SeedType.Bees, 68, 0));
        parent.Append(new SeedButton(SeedType.Ftw, 136, 0));
        parent.Append(new SeedButton(SeedType.Anniversary, 204, 0));
        parent.Append(new SeedButton(SeedType.DontStarve, 0, 68));
        parent.Append(new SeedButton(SeedType.Traps, 68, 68));
        parent.Append(new SeedButton(SeedType.Remix, 136, 68));
        parent.Append(new SeedButton(SeedType.Zenith, 204, 68));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        StartTimer.Update();

        if (MainPanel.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: World Feature GUI");
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    public void Open()
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Enabled = true;
        StartTimer.Open();
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Enabled = false;
        StartTimer.Close();
    }

    public override bool RenderTarget2DDraw => !StartTimer.Opened;
    public override float RenderTarget2DOpacity => StartTimer.Schedule;
    public override Vector2 RenderTarget2DOrigin => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DPosition => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DScale => new Vector2(0.95f + StartTimer.Lerp(0, 0.05f));
}