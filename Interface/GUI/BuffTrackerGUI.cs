using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Patches;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using System.Reflection;
using Terraria.GameInput;
using Terraria.ModLoader.UI;

namespace ImproveGame.Interface.GUI;

public class BuffTrackerGUI : ViewBody
{
    public override bool Display { get => Visible; set => Visible = value; }
    public static bool Visible { get; private set; }

    internal SUIPanel MainPanel;
    private BuffButtonList BuffList;
    public SUIScrollbar Scrollbar; // 拖动条
    private UISearchBar _searchBar;
    private SUIPanel _searchBoxPanel;
    private string _searchString;
    private bool _didClickSomething;
    private bool _didClickSearchBar;
    internal BuffTrackerBattler BuffTrackerBattler;
    private static int _oldBuffCount; // 用于及时更新列表

    public override bool CanPriority(UIElement target) => target != this;

    public override bool CanDisableMouse(UIElement target)
    {
        return (target != this && MainPanel.IsMouseHovering) || MainPanel.KeepPressed;
    }

    public override void OnInitialize()
    {
        MainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg, draggable: true)
        {
            Shaded = true,
            ShadowThickness = UIColor.ShadowThicknessThinnerer
        };
        MainPanel.SetPosPixels(630, 160).SetSizePixels(450, 220).Join(this);

        SUICross closeButton = new()
        {
            Left = new StyleDimension(-28f, 1f),
            Width = StyleDimension.FromPixels(28f),
            Height = StyleDimension.FromPixels(28f),
            BorderColor = Color.Transparent,
            BgColor = Color.Transparent
        };
        closeButton.OnLeftMouseDown += (_, _) => Close();
        MainPanel.Append(closeButton);

        UIHorizontalSeparator separator = new()
        {
            Top = StyleDimension.FromPixels(closeButton.Width.Pixels + 5f),
            Width = StyleDimension.FromPercent(1f),
            Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
        };
        MainPanel.Append(separator);

        BuffList = new BuffButtonList
        {
            Width = StyleDimension.FromPixelsAndPercent(-22f, 1f),
            Height = StyleDimension.FromPixelsAndPercent(-40f, 1f),
            Top = StyleDimension.FromPixels(40f),
            PaddingBottom = 4f,
            PaddingTop = 4f,
            ListPadding = 4f,
        };
        BuffList.SetPadding(2f);
        BuffList.ManualSortMethod = _ => { };
        MainPanel.Append(BuffList);

        Scrollbar = new SUIScrollbar
        {
            Left = {Pixels = -20f, Percent = 1f},
            Width = {Pixels = 18f},
            Top = {Pixels = 44f},
            Height = {Pixels = -48f, Percent = 1f}
        };
        Scrollbar.SetView(100f, 1000f);
        SetupScrollBar();
        MainPanel.Append(Scrollbar);

        BuffTrackerBattler = new BuffTrackerBattler();
        BuffTrackerBattler.Initialize();
        BuffTrackerBattler.MainPanel.SetPos(MainPanel.Left() - 92f, MainPanel.Top());
        Append(BuffTrackerBattler.MainPanel);

        UIElement searchArea = new()
        {
            Height = new StyleDimension(28f, 0f),
            Width = new StyleDimension(-42f, 1f)
        };
        searchArea.SetPadding(0f);
        MainPanel.Append(searchArea);
        AddSearchBar(searchArea);
        _searchBar.SetContents(null, forced: true);
    }

    private void SetupScrollBar(bool resetViewPosition = true)
    {
        float height = BuffList.GetInnerDimensions().Height;
        Scrollbar.SetView(height, BuffList.ListHeight);
        if (resetViewPosition)
            Scrollbar.ViewPosition = 0f;
    }

    public void SetupBuffButtons()
    {
        BuffList.Clear();

        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            if (!HideBuffSystem.BuffTypesShouldHide[i])
                continue;

            string internalName = BuffID.Search.GetName(i).ToLower(); // 《英文名》因为没法在非英语语言获取英文名，只能用内部名了
            string currentLanguageName = Lang.GetBuffName(i).ToLower();
            string searchString = _searchString?.Trim().ToLower();

            if (string.IsNullOrEmpty(_searchString) || internalName.Contains(searchString) ||
                currentLanguageName.Contains(searchString))
                BuffList.Add(new BuffButton(i));
        }

        Recalculate();
        SetupScrollBar(false);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
        if (MainPanel.GetOuterDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint()))
            Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Scrollbar is not null)
        {
            BuffList._innerList.Top.Set(-Scrollbar.ViewPosition, 0);
        }

        BuffList.Recalculate();

        if (MainPanel.IsMouseHovering || Scrollbar.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Buff Tracker GUI");
        }

        base.DrawSelf(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_didClickSomething && !_didClickSearchBar && _searchBar.IsWritingText)
            _searchBar.ToggleTakingText();

        _didClickSomething = false;
        _didClickSearchBar = false;

        BuffTrackerBattler.Update();
        // 刷新刷怪条
        BuffTrackerBattler.MainPanel.SetPosPixels(MainPanel.Left.Pixels - 92f, MainPanel.Top.Pixels);
        BuffTrackerBattler.MainPanel.Recalculate();

        if (_oldBuffCount != HideBuffSystem.HideBuffCount())
        {
            _oldBuffCount = HideBuffSystem.HideBuffCount();
            SetupBuffButtons();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Player player = Main.LocalPlayer;
        if (player.dead || !player.active)
        {
            Close();
            return;
        }

        base.Draw(spriteBatch);

        var panelDimensions = MainPanel.GetDimensions();

        if (HideBuffSystem.HideBuffCount() > 0)
        {
            return;
        }

        // 没Buff 显示提示
        Vector2 drawCenter = panelDimensions.Center();
        drawCenter.Y += 10f; // 加上顶栏的一半高度，保证绘制在下面区域的中央
        const float scale = 0.5f;
        string text = GetText("BuffTracker.NoInfBuff");
        // 设置都没开，加个提示
        if (!Config.NoConsume_Potion)
        {
            string textAlt = $"{GetText("BuffTracker.NoInfBuffAlt")}";
            float height = FontAssets.DeathText.Value.MeasureString(textAlt).Y * 0.5f;
            drawCenter.Y += height * 0.5f;
            float textAltWidth = FontAssets.DeathText.Value.MeasureString(textAlt).X * 0.5f;
            Vector2 originAlt = new(textAltWidth, 0f);
            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.DeathText.Value, textAlt, drawCenter.X, drawCenter.Y,
                Color.White, Color.Black, originAlt, scale);
            drawCenter.Y -= height;
        }

        float textWidth = FontAssets.DeathText.Value.MeasureString(text).X * 0.5f;
        Vector2 origin = new(textWidth, 0f);
        Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.DeathText.Value, text, drawCenter.X, drawCenter.Y,
            Color.White, Color.Black, origin, scale);
    }

    #region 搜索栏

    private void AddSearchBar(UIElement searchArea)
    {
        UIImageButton uIImageButton = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search"))
        {
            VAlign = 0.5f,
            HAlign = 0f
        };

        uIImageButton.OnLeftClick += Click_SearchArea;
        uIImageButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search_Border"));
        uIImageButton.SetVisibility(1f, 1f);
        searchArea.Append(uIImageButton);
        SUIPanel uIPanel = _searchBoxPanel = new SUIPanel(UIColor.SearchBarBorder, UIColor.SearchBarBg)
        {
            Left = new StyleDimension(4f, 0f),
            Width = new StyleDimension(0f - uIImageButton.Width.Pixels - 3f, 1f),
            Height = new StyleDimension(0f, 1f),
            VAlign = 0.5f,
            HAlign = 1f
        };
        uIPanel.SetPadding(0f);
        searchArea.Append(uIPanel);
        UISearchBar uISearchBar = _searchBar = new UISearchBar(Language.GetText("UI.PlayerNameSlot"), 0.8f)
        {
            Width = new StyleDimension(0f, 1f),
            Height = new StyleDimension(0f, 1f),
            HAlign = 0f,
            VAlign = 0.5f,
            IgnoresMouseInteraction = true
        };

        uIPanel.OnLeftClick += Click_SearchArea;
        uISearchBar.OnContentsChanged += OnSearchContentsChanged;
        uIPanel.Append(uISearchBar);
        uISearchBar.OnStartTakingInput += OnStartTakingInput;
        uISearchBar.OnEndTakingInput += OnEndTakingInput;
        uISearchBar.OnCanceledTakingInput += OnCanceledInput;
        UIImageButton uIImageButton2 = new(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel"))
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = new StyleDimension(-2f, 0f)
        };

        uIImageButton2.OnMouseOver += SearchCancelButton_OnMouseOver;
        uIImageButton2.OnLeftClick += SearchCancelButton_OnClick;
        uIPanel.Append(uIImageButton2);
    }

    private void SearchCancelButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
    {
        if (_searchBar.HasContents)
        {
            _searchBar.SetContents(null, forced: true);
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        else
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    private void SearchCancelButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    private void OnCanceledInput()
    {
        Main.LocalPlayer.ToggleInv();
    }

    private void Click_SearchArea(UIMouseEvent evt, UIElement listeningElement)
    {
        if (evt.Target.Parent != _searchBoxPanel)
        {
            _searchBar.ToggleTakingText();
            _didClickSearchBar = true;
        }
    }

    private void OnSearchContentsChanged(string contents)
    {
        _searchString = contents;
        SetupBuffButtons();
    }

    private void OnStartTakingInput()
    {
        _searchBoxPanel.BorderColor = UIColor.SearchBarBorderSelected;
    }

    private void OnEndTakingInput()
    {
        _searchBoxPanel.BorderColor = UIColor.SearchBarBorder;
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        AttemptStoppingUsingSearchbar(evt);
    }

    private void AttemptStoppingUsingSearchbar(UIMouseEvent evt)
    {
        _didClickSomething = true;
    }

    #endregion

    /// <summary>
    /// 打开GUI界面
    /// </summary>
    public void Open()
    {
        MainPanel.KeepPressed = false;
        Visible = true;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        SetupBuffButtons();
    }

    /// <summary>
    /// 关闭GUI界面
    /// </summary>
    public void Close()
    {
        Visible = false;
        Main.blockInput = false;
        SoundEngine.PlaySound(SoundID.MenuClose);
        Main.blockInput = false;
    }
}

public class BuffButtonList : UIList
{
    public float ListHeight;

    public override void RecalculateChildren()
    {
        base.RecalculateChildren();
        int buffCount = 0;
        float pixels = 4.0f;
        foreach (var u in _items)
        {
            u.SetPos(buffCount * 36.8f + 2f, pixels);
            buffCount++;
            u.Recalculate();
            if (buffCount >= 11f)
            {
                buffCount = 0;
                pixels += 36.8f;
            }
        }

        ListHeight = pixels + 36.8f;
    }
}

public class BuffButton : UIElement
{
    internal readonly int BuffId;

    public BuffButton(int buffId)
    {
        BuffId = buffId;
        this.SetSize(32f, 32f);
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
        base.MouseOver(evt);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        SoundEngine.PlaySound(SoundID.MenuTick);
        InfBuffPlayer.Get(Main.LocalPlayer).ToggleInfBuff(BuffId);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        bool buffEnabled = InfBuffPlayer.CheckInfBuffEnable(BuffId);

        var drawPosition = GetDimensions().Position();
        Asset<Texture2D> buffAsset = TextureAssets.Buff[BuffId];
        Texture2D texture = buffAsset.Value;
        float grayScale = buffEnabled ? 1f : .4f;

        spriteBatch.Draw(texture, GetDimensions().Position(), new Color(grayScale, grayScale, grayScale));

        if (!IsMouseHovering)
        {
            return;
        }

        // 绘制边框
        drawPosition.X -= 2;
        drawPosition.Y -= 2;
        spriteBatch.Draw(GetTexture("UI/Buff_HoverBorder").Value, drawPosition, Color.White);

        string buffName = Lang.GetBuffName(BuffId);
        string buffTooltip = Main.GetBuffTooltip(Main.LocalPlayer, BuffId);
        int rare = 0;
        if (Main.meleeBuff[BuffId])
            rare = -10;

        HideGlobalBuff.IsDrawingBuffTracker = true;
        BuffLoader.ModifyBuffText(BuffId, ref buffName, ref buffTooltip, ref rare);
        HideGlobalBuff.IsDrawingBuffTracker = false;
        
        string mouseText = $"{buffName}\n{buffTooltip}";

        if (buffEnabled)
            mouseText += $"\n{GetText("BuffTracker.LeftClickDisable")}";
        else
            mouseText += $"\n{GetText("BuffTracker.LeftClickEnable")}";

        UICommon.TooltipMouseText(mouseText);
    }
}