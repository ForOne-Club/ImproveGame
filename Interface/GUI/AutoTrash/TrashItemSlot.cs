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
        SetSizePixels(48, 48);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (Main.LocalPlayer.ItemAnimationActive) return;
        
        Main.playerInventory = true;

        if (!(Main.mouseItem?.IsAir ?? true))
        {
            if (AutoTrashPlayer.Instance.AutoDiscardItems.All(item => item.type != Main.mouseItem.type))
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

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        
        if (!Item.IsAir)
            return;

        var dimensions = GetDimensions();
        Vector2 pos = dimensions.Position();
        Vector2 size = dimensions.Size();
        var textureTrash = ModAsset.Trash.Value;
        spriteBatch.Draw(textureTrash, pos + size / 2f, null, Color.White * 0.2f, 0f, textureTrash.Size() / 2f, 1f, 0, 0);
    }
}
