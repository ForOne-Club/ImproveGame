using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.AmmoChainPanel.WeaponPage;

public class ItemSlotForPreview :BaseItemSlot
{
    private Item _realItem;

    public override Item Item { get => _realItem; set => _realItem = value; }

    public ItemSlotForPreview(Item item)
    {
        _realItem = item;
        AlwaysDisplayItemStack = true;
        DisplayItemInfo = true;
        SetSizePixels(80, 80);
    }
}