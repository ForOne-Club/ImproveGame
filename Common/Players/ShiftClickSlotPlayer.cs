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
        // 现在还没开放OverrideHover的接口，只能On了
        // 等我的PR: https://github.com/tModLoader/tModLoader/pull/2620 被接受就可以改掉了
        public override void Load() {
            On.Terraria.UI.ItemSlot.OverrideHover_ItemArray_int_int += ApplyHover;
        }

        private void ApplyHover(On.Terraria.UI.ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            if (Main.LocalPlayer.chest == -1 & Main.LocalPlayer.talkNPC == -1 && context == ItemSlot.Context.InventoryItem
                && ItemSlot.ShiftInUse && !inv[slot].IsAir && !inv[slot].favorited) {
                if (ArchitectureGUI.Visible &&
                    ArchitectureGUI.ItemSlot.Any(s =>
                                                 s.Value.CanPlaceItem(inv[slot]) &&
                                                 MyUtils.CanPlaceInSlot(s.Value.Item, inv[slot]) != 0)) {
                    Main.cursorOverride = 9;
                    return;
                }
                if (BigBagGUI.Visible && Main.LocalPlayer.TryGetModPlayer<DataPlayer>(out var modPlayer) &&
                    modPlayer.SuperVault.Any(s => MyUtils.CanPlaceInSlot(s, inv[slot]) != 0)) {
                    Main.cursorOverride = 9;
                    return;
                }
            }
            orig.Invoke(inv, context, slot);
        }

        /// <summary>
        /// Shift左键单击物品栏
        /// </summary>
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot) {
            if (Player.chest == -1 & Player.talkNPC == -1 && context == ItemSlot.Context.InventoryItem &&
                !inventory[slot].IsAir && !inventory[slot].favorited) {
                if (BigBagGUI.Visible) {
                    inventory[slot] = MyUtils.ItemStackToInventory(Player.GetModPlayer<DataPlayer>().SuperVault, inventory[slot], false);
                    SoundEngine.PlaySound(SoundID.Grab);
                    return true; // 阻止原版代码运行
                }

                if (!inventory[slot].IsAir && ArchitectureGUI.Visible) {
                    foreach (var itemSlot in from s in ArchitectureGUI.ItemSlot
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
                            return true; // 阻止原版代码运行
                        }
                    }
                }
            }
            return false;
        }
    }
}
