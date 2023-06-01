using ImproveGame.Common.Animations;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class TrashItemSlot : BaseItemSlot
{
    public readonly List<Item> TrashItems;
    public readonly int Index;

    public override Item Item
    {
        get
        {
            if (TrashItems.IndexInRange(Index))
            {
                return TrashItems[Index];
            }

            return AirItem;
        }
    }

    public TrashItemSlot(List<Item> items, int index)
    {
        this.TrashItems = items;
        Index = index;
        SetBaseItemSlotValues(true, true);
        SetSizePixels(52f, 52f);
        SetRoundedRectangleValues(UIColor.ItemSlotBg * 0.85f, 2f, UIColor.ItemSlotBorder * 0.85f, new Vector4(10f));
    }

    public override void MouseDown(UIMouseEvent evt)
    {
        Main.playerInventory = true;

        if (!(Main.mouseItem?.IsAir ?? true))
        {
            if (!AutoTrashPlayer.Instance.AutoDiscardItems.Any(item => item.type == Main.mouseItem.type))
            {
                AutoTrashPlayer.Instance.AutoDiscardItems.Add(new Item(Main.mouseItem.type));
            }

            AutoTrashPlayer.Instance.StackToLastItemsWithCleanUp(Main.mouseItem);
            Main.mouseItem = new Item();
            SoundEngine.PlaySound(SoundID.Grab);
        }
        else if (!Item.IsAir)
        {
            Main.mouseItem = Item;
            TrashItems.RemoveAt(Index);
            AutoTrashPlayer.Instance.CleanUpTrashItems();
            AutoTrashPlayer.Instance.AutoDiscardItems.RemoveAll(item => item.type == Main.mouseItem.type);
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }
}
