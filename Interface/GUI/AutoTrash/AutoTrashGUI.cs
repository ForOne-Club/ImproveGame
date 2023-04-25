using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class AutoTrashGUI : ViewBody
{
    public override bool Display
    {
        get => true;
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

    public SUIPanel MainPanel;
    public BaseGrid BaseGrid;

    public override void OnInitialize()
    {
        if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out AutoTrashPlayer atPlayer))
        {
            int hNumber = atPlayer.MaxCapacity;
            int vNumber = 1;

            MainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg);
            MainPanel.SetInnerPixels(52f * hNumber + 8f * (hNumber - 1), 52f * vNumber + 8f * (vNumber - 1));
            MainPanel.SetPos(10f, -(MainPanel.Height.Pixels + 10f), 0f, 1f);
            MainPanel.Join(this);

            BaseGrid = new BaseGrid();
            BaseGrid.SetGridValues(hNumber, new Vector2(8f), new Vector2(52f));
            BaseGrid.Join(MainPanel);

            for (int i = 0; i < hNumber; i++)
            {
                AutoTrashItemSlot itemSlot = new AutoTrashItemSlot(atPlayer.LastItems, i);
                itemSlot.Join(BaseGrid);
                Console.WriteLine($"AutoTrashItemSlot: {i}");
            }

            BaseGrid.CalculateAndSetGridSizePixels();
        }

    }
}
