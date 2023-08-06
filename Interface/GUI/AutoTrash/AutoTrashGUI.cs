using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Common.ModSystems;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.ExtremeStorage;
using ImproveGame.Interface.SUIElements;
using Terraria.ModLoader.UI;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class AutoTrashGUI : ViewBody
{
    #region 抽象实现
    
    public override bool Display
    {
        get => UIConfigs.Instance.QoLAutoTrash && (Main.playerInventory || !ExpanTimer.CompleteClose);
        set { }
    }

    public override bool CanDisableMouse(UIElement target)
    {
        return MainPanel.IsMouseHovering;
    }

    public override bool CanPriority(UIElement target)
    {
        return MainPanel.IsMouseHovering;
    }
    #endregion

    public static bool Hidden;
    public SUIPanel MainPanel;
    public BaseGrid ItemSlotGrid;
    public SUIBackgroundImage Image;
    public BaseGrid AutoDiscardItemsGrid;

    public override void OnInitialize()
    {
        if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out AutoTrashPlayer atPlayer))
        {
            int hNumber = atPlayer.MaxCapacity;
            int vNumber = 1;

            MainPanel = new SUIPanel(UIColor.ItemSlotBorder, UIColor.PanelBg, 12f)
            {
                OverflowHidden = true,
                Shaded = true,
                ShadowThickness = 20
            };

            ItemSlotGrid = new BaseGrid();
            ItemSlotGrid.SetBaseValues(vNumber, hNumber, new Vector2(6f), new Vector2(48f));
            ItemSlotGrid.Join(MainPanel);

            for (int i = 0; i < hNumber; i++)
            {
                TrashItemSlot itemSlot = new TrashItemSlot(atPlayer.TrashItems, i);
                itemSlot.Join(ItemSlotGrid);
            }

            ItemSlotGrid.CalculateWithSetGridSize();
            ItemSlotGrid.CalculateWithSetChildrenPosition();
            ItemSlotGrid.Recalculate();

            Texture2D setting = ModAsset.Setting.Value;
            Texture2D settingHover = ModAsset.SettingHover.Value;

            Image = new SUIBackgroundImage(setting);
            Image.SetPadding(8f);
            Image.SetInnerPixels(setting.Width, setting.Height);
            Image.SetRoundedRectangleValues(default, default, default, default);
            Image.Left.Pixels = ItemSlotGrid.Right();

            Image.OnMouseOver += (_, _) => Image.Texture = settingHover;
            Image.OnMouseOut += (_, _) => Image.Texture = setting;
            Image.OnLeftMouseDown += (_, _) =>
            {
                if (!OpenTiemr.AnyOpen) OpenTiemr.Open(); else OpenTiemr.Close();
            };

            Image.Join(MainPanel);

            View view = new View();
            view.SetSize(-4f, 2f, 1f, 0f);
            view.HAlign = 0.5f;
            view.SetPosPixels(0f, Image.BottomPixels() + 8f);
            view.SetRoundedRectangleValues(Color.White * 0.8f, 0f, default, new Vector4(1f));
            view.Join(MainPanel);

            AutoDiscardItemsGrid = new BaseGrid();
            AutoDiscardItemsGrid.SetBaseValues(3, hNumber + 2, new Vector2(4f), new Vector2(43));
            AutoDiscardItemsGrid.Top.Pixels = view.Bottom() + 6f;
            AutoDiscardItemsGrid.Join(MainPanel);
            RefreshAutoDiscardItemsGrid();

            MainPanel.SetInnerPixels(Image.RightPixels(), AutoDiscardItemsGrid.Bottom());
            MainPanel.SetPos(80f, -(MainPanel.Height.Pixels + 20f), 0f, 1f);
            MainPanel.Join(this);
        }
    }

    public void RefreshAutoDiscardItemsGrid()
    {
        AutoDiscardItemsGrid.RemoveAllChildren();

        for (int i = 0; i < AutoTrashPlayer.Instance.AutoDiscardItems.Count; i++)
        {
            AutoDiscardItemSlot itemSlot = new AutoDiscardItemSlot(AutoTrashPlayer.Instance.AutoDiscardItems, i);
            itemSlot.Join(AutoDiscardItemsGrid);
        }

        AutoDiscardItemsGrid.CalculateWithSetGridSize();
        AutoDiscardItemsGrid.CalculateWithSetChildrenPosition();
        AutoDiscardItemsGrid.Recalculate();
    }

    /// <summary>
    /// 开启背包页面时候的动画 
    /// </summary>
    public AnimationTimer ExpanTimer = new AnimationTimer(3);
    public AnimationTimer OpenTiemr = new AnimationTimer(3);

    public override void Update(GameTime gameTime)
    {
        bool recalculate = false;

        if (AutoTrashPlayer.Instance.AutoDiscardItems.Count != AutoDiscardItemsGrid.Children.Count())
        {
            RefreshAutoDiscardItemsGrid();
            recalculate = true;
        }

        float panelHeight = OpenTiemr.Lerp(Image.Bottom(), AutoDiscardItemsGrid.Bottom());

        if (MainPanel.Height.Pixels - MainPanel.VPadding() != panelHeight)
        {
            MainPanel.Height.Pixels = panelHeight + MainPanel.VPadding();
            recalculate = true;
        }

        float panelTop = ExpanTimer.Lerp(20, -MainPanel.Height.Pixels - 20f);

        if (MainPanel.Top.Pixels != panelTop)
        {
            MainPanel.Top.Pixels = panelTop;
            recalculate = true;
        }

        if (recalculate)
        {
            MainPanel.Recalculate();
        }

        if (Main.playerInventory && !ExtremeStorageGUI.VisibleAndExpanded && !Hidden && !Main.recBigList)
        {
            ExpanTimer.Open();
        }
        else
        {
            ExpanTimer.Close();
        }

        OpenTiemr.Update();
        ExpanTimer.Update();

        if (GetDimensions().X != Main.screenWidth || GetDimensions().Y != Main.screenHeight)
        {
            Recalculate();
        }

        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        Vector2 pos = MainPanel.GetDimensions().Position();
        Vector2 size = MainPanel.GetDimensionsSize();

        SDFRectangle.HasBorder(pos - new Vector2(2f), size + new Vector2(4f), new Vector4(14f), Color.White * 0.25f, 2f, Color.White);
        SDFRectangle.HasBorder(pos + new Vector2(2f), size - new Vector2(4f), new Vector4(10f), Color.Transparent, 2f, Color.White);

        if (!Image.IsMouseHovering)
            return;

        TryGetKeybindString(KeybindSystem.AutoTrashKeybind, out var keybind);
        UICommon.TooltipMouseText(GetText("UI.AutoTrash.Introduction", keybind));
    }
}
