using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI.OpenBag;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Common.Configs.Elements;

public class ThemeColorElement : EnumElement
{
    // 主面板
    public SUIPanel MainPanel;
    private SUIPanel TitlePanel;
    private SUITitle Title;
    private SUICross Cross;
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
        MainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
        {
            Shaded = true,
            HAlign = 0.5f
        };
        MainPanel.SetPadding(0f);
        MainPanel.SetPosPixels(0f, 38f);
        MainPanel.SetSize(480f, -60f, 0f, 1f);
        MainPanel.Join(this);
        ResetPreviewUI();
    }

    private void MakePreviewGUI()
    {
        TitlePanel = new SUIPanel(UIColor.PanelBorder, UIColor.TitleBg2)
        {
            DragIgnore = true,
            Width = {Pixels = 0f, Precent = 1f},
            Height = {Pixels = 50f, Precent = 0f},
            Rounded = new Vector4(10f, 10f, 0f, 0f),
            Relative = RelativeMode.Vertical,
            Spacing = new Vector2(0f, 8f)
        };
        TitlePanel.SetPadding(0f);
        TitlePanel.Join(MainPanel);

        // 标题
        Title = new SUITitle(GetText("UI.ThemePreview.Title"), 0.5f)
        {
            VAlign = 0.5f
        };
        Title.Join(TitlePanel);

        // Cross
        Cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Height = {Pixels = 0f, Precent = 1f},
            Rounded = new Vector4(0f, 10f, 0f, 0f)
        };
        Cross.Join(TitlePanel);

        var itemsPanel = new View
        {
            DragIgnore = true,
            Relative = RelativeMode.Vertical,
            OverflowHidden = true
        };
        itemsPanel.SetPadding(15, 15, 14, 14);
        itemsPanel.SetSize(0f, 164f, 1f, 0f);
        itemsPanel.Join(MainPanel);

        ItemGrid = new BaseGrid();
        ItemGrid.SetBaseValues(-1, 8, new Vector2(4f), new Vector2(48f));
        ItemGrid.Join(itemsPanel);

        Scrollbar = new SUIScrollBar
        {
            Left = {Pixels = -24f, Precent = 1f},
            Height = {Precent = 1f},
            Width = {Pixels = 20f}
        };
        Scrollbar.Join(itemsPanel);

        for (var i = 0; i < 32; i++)
        {
            var itemSlot = new BaseItemSlot();
            itemSlot.SetSizePixels(48f, 48f);
            itemSlot.Join(ItemGrid);
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
        UIColor.SetUIColors(Value);

        MainPanel.BorderColor = UIColor.PanelBorder;
        MainPanel.BgColor = UIColor.PanelBg;
        MainPanel.ShadowColor = MainPanel.BorderColor * 0.35f;
        MakePreviewGUI();

        UIColor.SetUIColors(curThemeType);
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