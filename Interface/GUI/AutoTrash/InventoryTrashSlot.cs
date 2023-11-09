using ImproveGame.Common.Players;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class InventoryTrashSlot : BaseItemSlot
{
    public float TrashScale = 1f;
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

    public InventoryTrashSlot(List<Item> items, int index)
    {
        TrashItems = items;
        Index = index;
        SetBaseItemSlotValues(true, true);
        SetSizePixels(44, 44);
        SetRoundedRectangleValues(new Color(84, 115, 130) * 0.8f, 2f, new Color(28, 28, 28) * 0.8f, new Vector4(12f * 0.8f));
        Rounded = new Vector4(10f);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if (Main.LocalPlayer.ItemAnimationActive)
        {
            return;
        }

        Main.playerInventory = true;

        if (Main.mouseItem?.IsAir ?? true)
        {
            if (!Item.IsAir)
            {
                Main.mouseItem = Item;
                TrashItems.RemoveAt(Index);
                AutoTrashPlayer.Instance.CleanUpTrash();
                AutoTrashPlayer.Instance.AutoDiscardItems.RemoveAll(item => item.type == Main.mouseItem.type);
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }
        else
        {
            if (AutoTrashPlayer.Instance.AutoDiscardItems.All(item => item.type != Main.mouseItem.type))
            {
                AutoTrashPlayer.Instance.AutoDiscardItems.Add(new Item(Main.mouseItem.type));
            }

            AutoTrashPlayer.Instance.StackToLastItemsWithCleanUp(Main.mouseItem);
            Main.mouseItem = new Item();
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
        spriteBatch.Draw(textureTrash, pos + size / 2f, null, Color.White * 0.5f, 0f, textureTrash.Size() / 2f, TrashScale, 0, 0);
    }
}
