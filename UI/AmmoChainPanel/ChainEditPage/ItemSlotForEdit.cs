using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Core;
using ImproveGame.UIFramework.BaseViews;
using Microsoft.Xna.Framework.Input;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class ItemSlotForEdit : BaseItemSlot
{
    public int Index;
    public ChainEditPage Parent;
    public Item RealItem;

    public override Item Item { get => RealItem; set => RealItem = value; }

    public ItemSlotForEdit(int index, ChainEditPage parent)
    {
        Index = index;
        Parent = parent;
        RelativeMode = RelativeMode.Horizontal;
        Spacing = new Vector2(2);
        AlwaysDisplayItemStack = true;
        SetSizePixels(60, 60);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        int selectedType = Parent.SelectedAmmoType;
        if (selectedType is ItemID.None || selectedType == RealItem.type)
            Parent.EditingChain.Chain.RemoveAt(Index);
        else
            Parent.EditingChain.Chain[Index].ItemData.Item.SetDefaults(Parent.SelectedAmmoType);

        Parent.ShouldResetCurrentChain = true;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);

        if (!IsMouseHovering || ItemSlot.ShiftInUse)
            return;

        int value = Math.Clamp(evt.ScrollWheelValue / 120, -10, 10);
        ref int times = ref Parent.EditingChain.Chain[Index].Times;
        times += value;
        times = Math.Clamp(Parent.EditingChain.Chain[Index].Times, 1, 999);
        RealItem.stack = times;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        DisplayItemInfo = Main.keyState.IsKeyDown(Main.FavoriteKey);

        if (!IsMouseHovering || !Parent.EditingChain.Chain.IndexInRange(Index))
            return;

        ref int times = ref Parent.EditingChain.Chain[Index].Times;
        if (Main.keyState.IsKeyDown(Keys.Up))
            times += 4;
        if (Main.keyState.IsKeyDown(Keys.Down))
            times -= 4;
        times = Math.Clamp(Parent.EditingChain.Chain[Index].Times, 1, 999);
        RealItem.stack = times;
    }
}