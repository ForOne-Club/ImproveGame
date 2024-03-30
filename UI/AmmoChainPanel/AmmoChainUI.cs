using ImproveGame.Common.Configs;
using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI.AmmoChainPanel;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Ammo Chain")]
public class AmmoChainUI : BaseBody
{
    public static AmmoChainUI Instance { get; private set; }
    public AmmoChainUI() => Instance = this;
    public override bool IsNotSelectable => StartTimer.AnyClose;

    public override bool Enabled
    {
        get => StartTimer.Closing || _enabled;
        set => _enabled = value;
    }
    private bool _enabled;

    public override bool CanSetFocusTarget(UIElement target)
        => target != this && (MainPanel.IsMouseHovering || MainPanel.IsLeftMousePressed);

    /// <summary>
    /// 启动关闭动画计时器
    /// </summary>
    public AnimationTimer StartTimer = new(3);

    #region Components
    // 主面板
    public SUIPanel MainPanel;

    // 标题面板
    private View _titlePanel;

    // 标题
    private SUIText _title;

    // 关闭按钮
    private SUICross _cross;

    // 当前显示页
    private PageContainer _shownPage;

    // 武器页
    private WeaponPage.WeaponPage _weaponPage;

    // 编辑页
    private ChainEditPage.ChainEditPage _chainEditPage;
    #endregion

    public override void OnInitialize()
    {
        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            Draggable = true,
            FinallyDrawBorder = true
        };
        MainPanel.SetPadding(0f);
        MainPanel.SetPosPixels(610, 320)
            .SetSizePixels(600, 460)
            .JoinParent(this);

        _titlePanel = ViewHelper.CreateHead(UIStyle.TitleBg * 0.75f, 50f, 10f);
        _titlePanel.RelativeMode = RelativeMode.Vertical;
        _titlePanel.SetPadding(0f);
        _titlePanel.JoinParent(MainPanel);

        // 标题
        _title = new SUIText
        {
            TextOrKey = "Mods.ImproveGame.UI.AmmoChain.Title",
            UseKey = true,
            IsLarge = true,
            TextScale = 0.55f,
            VAlign = 0.5f,
            TextBorder = 2f,
            PaddingLeft = 20f,
            DragIgnore = true
        };
        _title.SetInnerPixels(_title.TextSize * _title.TextScale);
        _title.JoinParent(_titlePanel);

        // Cross
        _cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Width = { Pixels = 55f, Percent = 0f },
            Height = { Pixels = 0f, Precent = 1f },
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            Border = 0f,
            BorderColor = Color.Transparent,
            CrossOffset = new Vector2(1f, 0f),
            CrossRounded = UIStyle.CrossThickness * 0.95f
        };
        _cross.OnLeftMouseDown += (_, _) => Close();
        _cross.OnUpdate += _ =>
        {
            _cross.BgColor = _cross.HoverTimer.Lerp(Color.Transparent, UIStyle.PanelBorder * 0.5f);
        };
        _cross.JoinParent(_titlePanel);

        _weaponPage = new WeaponPage.WeaponPage();
        _chainEditPage = new ChainEditPage.ChainEditPage();

        _shownPage = new PageContainer();
        _shownPage.JoinParent(MainPanel);
        _shownPage.SetPage(_weaponPage);
    }

    public override void Update(GameTime gameTime)
    {
        StartTimer.Update();
        base.Update(gameTime);
        if (MainPanel.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Ammo Chain UI");
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    public void Open()
    {
        Enabled = true;
        StartTimer.Open();

        _weaponPage.ReloadPresetsElement();
        _weaponPage.SetupPreview();

        SoundEngine.PlaySound(SoundID.MenuOpen);
        Main.playerInventory = true;
    }

    public void Close()
    {
        Enabled = false;
        StartTimer.Close();

        SoundEngine.PlaySound(SoundID.MenuClose);
        AdditionalConfig.Save();
    }

    public void GoToWeaponPage()
    {
        _shownPage.SetPage(_weaponPage);
        _weaponPage.ReloadPresetsElement();
    }

    public void StartEditingChain(AmmoChain chain, bool isCreatingAChain, string chainName)
    {
        _chainEditPage = new ChainEditPage.ChainEditPage();
        _shownPage.SetPage(_chainEditPage);
        _chainEditPage.IsCreatingAChain = isCreatingAChain;
        _chainEditPage.EditingChain = chain;
        _chainEditPage.ChainName = chainName;
        _chainEditPage.SetupCurrentChain(_chainEditPage.EditingChain);
    }

    public override bool RenderTarget2DDraw => !StartTimer.Opened;
    public override float RenderTarget2DOpacity => StartTimer.Schedule;
    public override Vector2 RenderTarget2DOrigin => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DPosition => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DScale => new Vector2(0.95f + StartTimer.Lerp(0, 0.05f));
}