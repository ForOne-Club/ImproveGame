using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Common.Configs.Elements;

public class ThemeColorElement : EnumElement
{
    // 主面板
    public SUIPanel MainPanel;
    public BaseGrid ItemGrid;
    public SUIScrollBar Scrollbar;

    private ThemeType _valueLastTick; // 检测Value有没有变，变了就重设UI

    public ThemeType Value
    {
        get => (ThemeType)_getValue.Invoke();
        set => _setValue.Invoke((int)value);
    }

    public override void OnBind()
    {
        base.OnBind();
        Height.Set(280f, 0f);

        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            HAlign = 0.5f
        };
        MainPanel.SetPadding(0f);
        MainPanel.SetPosPixels(0f, 38f);
        MainPanel.SetSize(480f, -60f, 0f, 1f);
        MainPanel.JoinParent(this);
        ResetPreviewUI();
    }

    private void MakePreviewGUI()
    {
        var titlePanel = ViewHelper.CreateHead(Color.Black * 0.25f, 45f, 10f);
        titlePanel.SetPadding(0f);
        titlePanel.JoinParent(MainPanel);

        // 标题
        var title = new SUIText
        {
            IsLarge = true,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.ThemePreview.Title",
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
        cross.JoinParent(titlePanel);

        var itemsPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical,
            OverflowHidden = true
        };
        itemsPanel.SetPadding(15, 15, 14, 14);
        itemsPanel.SetSize(0f, 164f, 1f, 0f);
        itemsPanel.JoinParent(MainPanel);

        ItemGrid = new BaseGrid();
        ItemGrid.SetBaseValues(-1, 8, new Vector2(4f), new Vector2(48f));
        ItemGrid.JoinParent(itemsPanel);

        Scrollbar = new SUIScrollBar
        {
            Left = {Pixels = -24f, Precent = 1f},
            Height = {Precent = 1f},
            Width = {Pixels = 20f}
        };
        Scrollbar.JoinParent(itemsPanel);

        for (var i = 0; i < 32; i++)
        {
            var itemSlot = new BaseItemSlot();
            itemSlot.SetSizePixels(48f, 48f);
            itemSlot.JoinParent(ItemGrid);
        }

        ItemGrid.CalculateAndSetSize();
        ItemGrid.CalculateAndSetChildrenPosition();
        ItemGrid.Recalculate();
        Scrollbar.SetView(itemsPanel.GetInnerSizePixels().Y, ItemGrid.Height.Pixels);
    }

    private void ResetPreviewUI()
    {
        MainPanel.RemoveAllChildren();
        var curThemeType = UIConfigs.Instance.ThemeType;
        UIStyle.SetUIColors(Value);

        MainPanel.BorderColor = UIStyle.PanelBorder;
        MainPanel.BgColor = UIStyle.PanelBg;
        MainPanel.ShadowColor = MainPanel.BorderColor * 0.35f;
        MakePreviewGUI();

        UIStyle.SetUIColors(curThemeType);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        MainPanel.DrawSelf(spriteBatch);
        MainPanel.DrawChildren(spriteBatch);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);

        if (!MainPanel.IsMouseHovering)
            return;
        // 下滑: -, 上滑: +
        Scrollbar.BarTopBuffer += evt.ScrollWheelValue * 0.4f;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (-Scrollbar.BarTop == ItemGrid.DatumPoint.Y)
            return;

        ItemGrid.DatumPoint.Y = -Scrollbar.BarTop;
        ItemGrid.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_valueLastTick != Value)
            ResetPreviewUI();

        _valueLastTick = Value;

        if (!MainPanel.IsMouseHovering)
            return;

        PlayerInput.LockVanillaMouseScroll("ImproveGame: Theme Preview GUI");
        Main.LocalPlayer.mouseInterface = true;
    }
}