using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class AutoTrashItemSlot : View
{
    public readonly List<Item> items;
    public readonly int Index;
    public readonly Item AirItem = new Item();

    public Item Item
    {
        get
        {
            if (items.IndexInRange(Index))
            {
                return items[Index];
            }

            return AirItem;
        }
    }

    public AutoTrashItemSlot(List<Item> items, int index)
    {
        SetSizePixels(52f, 52f);
        this.items = items;
        Index = index;
        SetRoundedRectangleValues(UIColor.ItemSlotBg, 2f, UIColor.ItemSlotBorder, new Vector4(10f));
    }

    public override void MouseDown(UIMouseEvent evt)
    {
        Main.playerInventory = true;

        if (Main.mouseItem is not null && !Main.mouseItem.IsAir)
        {
            AutoTrashPlayer.Instance.AddToLastItem(Main.mouseItem);
            AutoTrashPlayer.Instance.AddToAutoTrash(Main.mouseItem.type);
            Main.mouseItem = new Item();
            SoundEngine.PlaySound(SoundID.Grab);
        }
        else if (!Item.IsAir)
        {
            Main.mouseItem = Item;
            AutoTrashPlayer.Instance.RmoveItemFromLastItem(Main.mouseItem);
            AutoTrashPlayer.Instance.RemoveItemFromAutoTrash(Main.mouseItem.type);
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (Item.IsAir)
        {
            return;
        }

        if (IsMouseHovering)
        {
            Main.hoverItemName = Item.Name;
            Main.HoverItem = Item.Clone();
        }

        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensionsSize();

        BigBagItemSlot.DrawItemIcon(Main.spriteBatch, Item, Color.White, GetInnerDimensions());

        if (Item.stack > 0)
        {
            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(Item.stack.ToString()) * 0.75f;
            Vector2 textPos = pos + new Vector2(size.X * 0.18f, (size.Y - textSize.Y) * 0.9f);
            DrawString(textPos, Item.stack.ToString(), Color.White, Color.Black, 0.75f);
        }
    }
}
