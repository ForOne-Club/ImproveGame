﻿using ImproveGame.UI.AmmoChainPanel;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework.UIElements;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameInput;
using Terraria.UI.Chat;

namespace ImproveGame.UI.GrabBagInfo;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Grab Bag Info GUI")]
public class GrabBagInfoGUI : BaseBody
{
    public static GrabBagInfoGUI Instance { get; private set; }

    public GrabBagInfoGUI() => Instance = this;

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

    public static int ItemID { get; private set; }

    #region Components

    // 主面板
    public SUIPanel MainPanel = new(UIStyle.PanelBorder, UIStyle.PanelBg)
    {
        Shaded = true,
        Draggable = true,
        FinallyDrawBorder = true,
        OverflowHidden = true
    };

    // 标题面板
    private View _titlePanel;

    // 标题
    private SUIText _title = new()
    {
        TextOrKey = "Mods.ImproveGame.UI.GrabBagInfo.Title",
        UseKey = true,
        IsLarge = true,
        TextScale = 0.5f,
        VAlign = 0.5f,
        TextBorder = 2f,
        PaddingLeft = 20f,
        DragIgnore = true
    };

    // 关闭按钮
    private SUICross _cross = new()
    {
        HAlign = 1f,
        VAlign = 0.5f,
        Width = { Pixels = 50f, Percent = 0f },
        Height = { Pixels = 0f, Precent = 1f },
        Rounded = new Vector4(0f, 10f, 0f, 0f),
        Border = 0f,
        BorderColor = Color.Transparent,
        CrossOffset = new Vector2(0f, 0f),
        CrossRounded = UIStyle.CrossThickness * 0.95f
    };

    // 当前物品展示
    public BaseItemSlot ItemSlot = new()
    {
        RelativeMode = RelativeMode.Vertical,
        DisplayItemInfo = true,
        DisplayItemStack = false,
        DragIgnore = true
    };

    // 当前物品展示
    public SUIText ItemNameText = new()
    {
        RelativeMode = RelativeMode.Horizontal,
        BgColor = Color.Black * 0.3f,
        BorderColor = Color.Black * 0.3f,
        TextScale = 0.4f,
        TextAlign = new Vector2(0.5f),
        IsLarge = true,
        Spacing = new Vector2(8f, 0f),
        Rounded = new Vector4(6f),
        DragIgnore = true
    };

    // 明细列表
    public SUIScrollView2 InfoScroll = new(Orientation.Vertical)
    {
        RelativeMode = RelativeMode.Vertical,
        Spacing = new Vector2(4f)
    };

    // 上半部分面板
    private View _upperPanel = new()
    {
        DragIgnore = true,
        RelativeMode = RelativeMode.Vertical
    };

    // 下半部分面板
    private View _lowerPanel = new()
    {
        DragIgnore = true,
        RelativeMode = RelativeMode.Vertical
    };

    #endregion

    public override void OnInitialize()
    {
        int panelLeft = 730;
        int panelTop = 160;
        int panelHeight = 316;
        int panelWidth = 380;

        Instance = this;

        // 主面板
        MainPanel.SetPadding(0f);
        MainPanel.SetPosPixels(panelLeft, panelTop)
            .SetSizePixels(panelWidth, panelHeight)
            .JoinParent(this);

        _titlePanel = ViewHelper.CreateHead(UIStyle.TitleBg * 0.75f, 46f, 10f);
        _titlePanel.RelativeMode = RelativeMode.Vertical;
        _titlePanel.SetPadding(0f);
        _titlePanel.JoinParent(MainPanel);

        // 标题
        _title.SetInnerPixels(_title.TextSize * _title.TextScale);
        _title.JoinParent(_titlePanel);

        // Cross
        _cross.OnLeftMouseDown += (_, _) => Close();
        _cross.OnUpdate += _ =>
        {
            _cross.BgColor = _cross.HoverTimer.Lerp(Color.Transparent, UIStyle.PanelBorder * 0.5f);
        };
        _cross.JoinParent(_titlePanel);

        _upperPanel.SetPadding(17f, 10f, 20f, 2f);
        _upperPanel.SetSize(0f, 60, 1f, 0f);
        _upperPanel.JoinParent(MainPanel);
        SetupUpperPanel();

        // MainPanel.MakeHorizontalSeparator();
        MainPanel.Append(new View()
        {
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(8f),
            Width = new StyleDimension(0f, 1f),
            Height = new StyleDimension(4f, 0f),
            BgColor = UIStyle.PanelBorder * 0.8f,
            Border = 1f,
            BorderColor = Color.Transparent,
        });

        _lowerPanel.SetPadding(0);
        _lowerPanel.SetSize(0f, 190, 1f, 0f);
        _lowerPanel.JoinParent(MainPanel);
        SetupLowerPanel();

        Recalculate();
    }

    private void SetupUpperPanel()
    {
        ItemSlot.SetSizePixels(48, 48);
        ItemSlot.JoinParent(_upperPanel);

        ItemNameText.SetSizePixels(290, 48);
        ItemNameText.JoinParent(_upperPanel);
    }

    private void SetupLowerPanel()
    {
        InfoScroll.ScrollBar.ShrinkIfNotHovering = true;
        InfoScroll.ScrollBar.Spacing = new Vector2(8f);
        InfoScroll.SetPadding(16, 8, 16, 8);
        InfoScroll.SetSize(0f, 0f, 1f, 1f);
        InfoScroll.JoinParent(_lowerPanel);
    }

    public void SetupDropRateInfoList()
    {
        InfoScroll.ListView.RemoveAllChildren();

        var itemDropRules = Main.ItemDropsDB.GetRulesForItemID(ItemID);
        var list = new List<DropRateInfo>();
        var ratesInfo = new DropRateInfoChainFeed(1f);
        foreach (IItemDropRule item in itemDropRules)
        {
            item.ReportDroprates(list, ratesInfo);
        }

        foreach (DropRateInfo item2 in list)
        {
            new GrabBagInfoPanel(item2).JoinParent(InfoScroll.ListView);
        }
    }

    public override void Update(GameTime gameTime)
    {
        StartTimer.Update();

        base.Update(gameTime);

        if (InfoScroll.ListView.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Grab Bag Info GUI");
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    public void Open(int itemID)
    {
        Enabled = true;
        StartTimer.Open();

        ItemID = itemID;
        ItemSlot.AirItem.SetDefaults(ItemID);
        SetupDropRateInfoList();

        string text = ItemSlot.AirItem.Name + "";
        ItemNameText.TextOrKey = text;
        float textWidth = ChatManager.GetStringSize(FontAssets.DeathText.Value, text, new Vector2(0.4f)).X;
        float panelWidth = ItemNameText.GetInnerDimensions().Width - 30;
        if (textWidth > panelWidth)
            ItemNameText.TextScale = 0.4f * (panelWidth / textWidth);
        else
            ItemNameText.TextScale = 0.4f;

        SoundEngine.PlaySound(SoundID.MenuOpen);
        Recalculate();
    }

    public void Close()
    {
        Enabled = false;
        StartTimer.Close();

        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    public override bool RenderTarget2DDraw => !StartTimer.Opened;
    public override float RenderTarget2DOpacity => StartTimer.Schedule;
    public override Vector2 RenderTarget2DOrigin => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DPosition => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DScale => new(0.95f + StartTimer.Lerp(0, 0.05f));
}