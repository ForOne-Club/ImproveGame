using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class SelectableAmmoSlot : BaseItemSlot
{
    public AnimationTimer FavoritedTimer = new AnimationTimer(3);
    public ChainEditPage Parent;
    public Item RealItem;

    public override Item Item { get => RealItem; set => RealItem = value; }

    public SelectableAmmoSlot(Item item, ChainEditPage parent)
    {
        RealItem = item;
        Parent = parent;
        RelativeMode = RelativeMode.Vertical;
        PreventOverflow = true;
        Spacing = new Vector2(2);
        DisplayItemStack = false;
        SetSizePixels(44, 44);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        FavoritedTimer.Update();

        if (Parent.SelectedAmmoType == Item.type)
            FavoritedTimer.Open();
        else
            FavoritedTimer.Close();

        BorderColor = FavoritedTimer.Lerp(UIStyle.ItemSlotBorder, UIStyle.ItemSlotBorderFav);
        BgColor = UIStyle.TrashSlotBg;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (!IsMouseHovering)
            return;
        Main.HoverItem = Item;
        Main.hoverItemName = Item.Name;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        Parent.SelectedAmmoType = Parent.SelectedAmmoType == Item.type
            ? ItemID.None
            : Item.type;
    }
}