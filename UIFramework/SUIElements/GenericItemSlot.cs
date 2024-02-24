using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UIFramework.SUIElements;

public class GenericItemSlot(IList<Item> items, int index) : BaseItemSlot
{
    protected readonly IList<Item> _items = items;
    protected readonly int _index = index;

    public override Item Item
    {
        get => (_items.Count > 0 && _index < _items.Count) ? _items[_index] : AirItem;
        set
        {
            if (_items.Count > 0 && _index < _items.Count)
            {
                _items[_index] = value;
            }
        }
    }

    /*public void MouseLBDownItemInteraction()
    {
        if (Main.LocalPlayer.ItemAnimationActive) { return; }

        if (Item.IsAir)
        {
            Item = Main.mouseItem.Clone();
            Main.mouseItem.TurnToAir();
        }
        else
        {
            if (Main.mouseItem.IsAir)
            {
                Main.mouseItem = Item.Clone();
                Item.TurnToAir();
            }
            else
            {
                if (Item.stack < Item.maxStack)
                {
                    ItemLoader.TryStackItems(Item, Main.mouseItem, out _);
                }
            }
        }
    }*/
}
