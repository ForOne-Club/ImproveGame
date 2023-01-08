using ImproveGame.Common.ModHooks;
using ImproveGame.Common.Packets.NetAutofisher;
using ImproveGame.Content.Items;
using ImproveGame.Interface.BannerChest;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;

namespace ImproveGame.Common.Players
{
    public class ShiftClickSlotPlayer : ModPlayer
    {
        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            var item = inventory[slot];

            if (item.ModItem is IItemOverrideHover overrideHover && overrideHover.OverrideHover(inventory, context, slot))
                return true;

            if (Main.LocalPlayer.chest == -1 & Main.LocalPlayer.talkNPC == -1 && context == ItemSlot.Context.InventoryItem
                && ItemSlot.ShiftInUse && !item.IsAir && !item.favorited)
            {
                if (ArchitectureGUI.Visible &&
                    UISystem.Instance.ArchitectureGUI.ItemSlot.Any(s =>
                                                 s.Value.CanPlaceItem(item) &&
                                                 CanPlaceInSlot(s.Value.Item, item) != 0))
                {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                    return true;
                }
                // 旗帜收纳箱 + 药水袋
                if (PackageGUI.Visible &&
                    ((PackageGUI.storageType is PackageGUI.StorageType.Banners && ItemToBanner(item) != -1) ||
                    (PackageGUI.storageType is PackageGUI.StorageType.Potions && item.buffType > 0 && item.consumable)))
                {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                    return true;
                }
                if (BigBagGUI.Visible && Main.LocalPlayer.TryGetModPlayer<DataPlayer>(out var dataPlayer) &&
                    dataPlayer.SuperVault.Any(s => CanPlaceInSlot(s, item) != 0))
                {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                    return true;
                }
                if (AutofisherGUI.Visible && AutofishPlayer.LocalPlayer.TryGetAutofisher(out var fisher) && fisher.fish.Any(s => CanPlaceInSlot(s, item) != 0))
                {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Shift左键单击物品栏
        /// </summary>
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (Player.chest == -1 & Player.talkNPC == -1 && context == ItemSlot.Context.InventoryItem &&
                !inventory[slot].IsAir && !inventory[slot].favorited)
            {
                if (PackageGUI.Visible)
                {
                    // 旗帜
                    if (PackageGUI.storageType is PackageGUI.StorageType.Banners && ItemToBanner(inventory[slot]) != -1)
                    {
                        UISystem.Instance.PackageGUI.Package.PutInPackage(ref inventory[slot]);
                        Recipe.FindRecipes();
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    // 药水
                    else if (PackageGUI.storageType is PackageGUI.StorageType.Potions && inventory[slot].buffType > 0 && inventory[slot].consumable)
                    {
                        UISystem.Instance.PackageGUI.Package.PutInPackage(ref inventory[slot]);
                        Recipe.FindRecipes();
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    return true; // 阻止原版代码运行
                }

                if (BigBagGUI.Visible)
                {
                    inventory[slot] = ItemStackToInventory(Player.GetModPlayer<DataPlayer>().SuperVault, inventory[slot], false);
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.Grab);
                    return true; // 阻止原版代码运行
                }

                if (AutofisherGUI.Visible && AutofishPlayer.LocalPlayer.TryGetAutofisher(out var fisher))
                {
                    inventory[slot] = ItemStackToInventory(fisher.fish, inventory[slot], false);
                    AutofisherGUI.RequireRefresh = true;
                    UISystem.Instance.AutofisherGUI.RefreshItems();
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        for (int i = 0; i <= 14; i++)
                            ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher, (byte)i).Send(runLocally: false);
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.Grab);
                    return true; // 阻止原版代码运行
                }

                if (!inventory[slot].IsAir && ArchitectureGUI.Visible)
                {
                    foreach (var itemSlot in from s in UISystem.Instance.ArchitectureGUI.ItemSlot
                                             where s.Value.CanPlaceItem(inventory[slot])
                                             select s)
                    {
                        // 放到建筑GUI里面
                        ref Item slotItem = ref itemSlot.Value.Item;
                        ref Item placeItem = ref inventory[slot];

                        byte placeMode = CanPlaceInSlot(slotItem, placeItem);

                        // type不同直接切换吧
                        if (placeMode == 1)
                        {
                            itemSlot.Value.SwapItem(ref placeItem);
                            SoundEngine.PlaySound(SoundID.Grab);
                            Recipe.FindRecipes();
                            // 和魔杖实例同步
                            itemSlot.Value.ItemChange();
                            return true; // 阻止原版代码运行
                        }
                        // type相同，里面的能堆叠，放进去
                        if (placeMode == 2)
                        {
                            int stackAvailable = slotItem.maxStack - slotItem.stack;
                            int stackAddition = Math.Min(placeItem.stack, stackAvailable);
                            placeItem.stack -= stackAddition;
                            slotItem.stack += stackAddition;
                            SoundEngine.PlaySound(SoundID.Grab);
                            Recipe.FindRecipes();
                            // 和魔杖实例同步
                            itemSlot.Value.ItemChange();
                            return true; // 阻止原版代码运行
                        }
                    }
                }
            }
            return false;
        }
    }
}
