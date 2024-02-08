using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using PinyinNet;
using Terraria.GameInput;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI;

public class BuffTrackerGUI : BaseBody
{
    public override bool Enabled { get => Visible; set => Visible = value; }
    public static bool Visible { get; private set; }

    internal SUIPanel MainPanel;
    private BuffButtonList BuffList;
    public SUIScrollBar Scrollbar; // 拖动条
    private SUISearchBar _searchBar;
    private string SearchContent => _searchBar.SearchContent;
    internal BuffTrackerBattler BuffTrackerBattler;
    private static int _oldBuffCount; // 用于及时更新列表

    public override bool CanSetFocusTarget(UIElement target)
    {
        return (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;
    }

    public override void OnInitialize()
    {
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg, draggable: true)
        {
            Shaded = true,
            ShadowThickness = UIStyle.ShadowThicknessThinnerer
        };
        MainPanel.SetPosPixels(630, 160).SetSizePixels(450, 220).JoinParent(this);

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

        Scrollbar = new SUIScrollBar
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

        _searchBar = new SUISearchBar(true, false)
        {
            Height = new StyleDimension(28f, 0f),
            Width = new StyleDimension(-42f, 1f),
            DragIgnore = true
        };
        // _searchBar.OnDraw += SearchBarOnDraw;
        _searchBar.OnSearchContentsChanged += _ => SetupBuffButtons();
        _searchBar.JoinParent(MainPanel);

        OnLeftMouseDown += (_, _) => TryCancelInput();
        OnRightMouseDown += (_, _) => TryCancelInput();
        OnMiddleMouseDown += (_, _) => TryCancelInput();
        OnXButton1MouseDown += (_, _) => TryCancelInput();
        OnXButton2MouseDown += (_, _) => TryCancelInput();
    }

    private void TryCancelInput() {
        if (!MainPanel.IsMouseHovering)
            _searchBar.AttemptStoppingUsingSearchbar();
        foreach (var element in MainPanel.Children)
            if (element is not SUISearchBar && element.IsMouseHovering)
                _searchBar.AttemptStoppingUsingSearchbar();
    }

    private void SetupScrollBar(bool resetViewPosition = true)
    {
        float height = BuffList.GetInnerDimensions().Height;
        Scrollbar.SetView(height, BuffList.ListHeight);
        if (resetViewPosition)
            Scrollbar.BarTop = 0f;
    }

    public void SetupBuffButtons()
    {
        BuffList.Clear();

        string searchString = SearchContent?.Trim().ToLower();
        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            if (!HideBuffSystem.BuffTypesShouldHide[i])
                continue;

            if (string.IsNullOrEmpty(SearchContent) || Match(searchString, i))
                BuffList.Add(new BuffButton(i));
        }

        Recalculate();
        SetupScrollBar(false);
    }

    private bool Match(string searchString, int buffId)
    {
        string currentLanguageName = RemoveSpaces(Lang.GetBuffName(buffId)).ToLower();

        if (currentLanguageName.Contains(searchString))
            return true;

        if (Language.ActiveCulture.Name is not "zh-Hans") return false;

        string pinyin = RemoveSpaces(PinyinConvert.GetPinyinForAutoComplete(currentLanguageName));
        return pinyin.Contains(searchString);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
        if (MainPanel.GetOuterDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint()))
            Scrollbar.BarTopBuffer += evt.ScrollWheelValue;
    }

    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        base.DrawChildren(spriteBatch);

        // 不被遮挡
        Vector2 position = _searchBar.GetDimensions().Position();
        position.Y += 60f;
        Main.instance.DrawWindowsIMEPanel(position, 0f);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Scrollbar is not null)
        {
            BuffList._innerList.Top.Set(-Scrollbar.BarTop, 0);
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

    /// <summary>
    /// 打开GUI界面
    /// </summary>
    public void Open()
    {
        MainPanel.IsLeftMousePressed = false;
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

        ListHeight = pixels;
        if (buffCount != 0)
            ListHeight += 36.8f;
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