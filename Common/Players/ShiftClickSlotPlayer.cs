using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImproveGame.Common.Players
{
    public class ShiftClickSlotPlayer : ModPlayer
    {
        public bool ApplyHover(Item[] inv, int context, int slot) {
            if (Main.LocalPlayer.chest == -1 & Main.LocalPlayer.talkNPC == -1 && context == ItemSlot.Context.InventoryItem
                && ItemSlot.ShiftInUse && !inv[slot].IsAir && !inv[slot].favorited) {
                if (ArchitectureGUI.Visible &&
                    UISystem.Instance.ArchitectureGUI.ItemSlot.Any(s =>
                                                 s.Value.CanPlaceItem(inv[slot]) &&
                                                 MyUtils.CanPlaceInSlot(s.Value.Item, inv[slot]) != 0)) {
                    Main.cursorOverride = 9;
                    return true;
                }
                if (BigBagGUI.Visible && Main.LocalPlayer.TryGetModPlayer<DataPlayer>(out var dataPlayer) &&
                    dataPlayer.SuperVault.Any(s => MyUtils.CanPlaceInSlot(s, inv[slot]) != 0)) {
                    Main.cursorOverride = 9;
                    return true;
                }
                if (AutofisherGUI.Visible && AutofishPlayer.LocalPlayer.TryGetAutofisher(out var fisher) && fisher.fish.Any(s => MyUtils.CanPlaceInSlot(s, inv[slot]) != 0)) {
                    Main.cursorOverride = 9;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Shift左键单击物品栏
        /// </summary>
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot) {
            if (Player.chest == -1 & Player.talkNPC == -1 && context == ItemSlot.Context.InventoryItem &&
                !inventory[slot].IsAir && !inventory[slot].favorited) {
                if (BigBagGUI.Visible) {
                    inventory[slot] = MyUtils.ItemStackToInventory(Player.GetModPlayer<DataPlayer>().SuperVault, inventory[slot], false);
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.Grab);
                    return true; // 阻止原版代码运行
                }

                if (AutofisherGUI.Visible && AutofishPlayer.LocalPlayer.TryGetAutofisher(out var fisher)) {
                    inventory[slot] = MyUtils.ItemStackToInventory(fisher.fish, inventory[slot], false);
                    UISystem.Instance.AutofisherGUI.RefreshItems();
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.Grab);
                    return true; // 阻止原版代码运行
                }

                if (!inventory[slot].IsAir && ArchitectureGUI.Visible) {
                    foreach (var itemSlot in from s in UISystem.Instance.ArchitectureGUI.ItemSlot
                                             where s.Value.CanPlaceItem(inventory[slot])
                                             select s) {
                        // 放到建筑GUI里面
                        ref Item slotItem = ref itemSlot.Value.Item;
                        ref Item placeItem = ref inventory[slot];

                        byte placeMode = MyUtils.CanPlaceInSlot(slotItem, placeItem);

                        // type不同直接切换吧
                        if (placeMode == 1) {
                            itemSlot.Value.SwapItem(ref placeItem);
                            SoundEngine.PlaySound(SoundID.Grab);
                            Recipe.FindRecipes();
                            // 和魔杖实例同步
                            itemSlot.Value.ItemChange();
                            return true; // 阻止原版代码运行
                        }
                        // type相同，里面的能堆叠，放进去
                        if (placeMode == 2) {
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
