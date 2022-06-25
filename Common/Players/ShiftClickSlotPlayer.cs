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
        public override void Load() {
            On.Terraria.UI.ItemSlot.OverrideHover_ItemArray_int_int += ApplyHover;
        }

        private void ApplyHover(On.Terraria.UI.ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            if (Main.LocalPlayer.chest == -1 & Main.LocalPlayer.talkNPC == -1 &&
                ItemSlot.ShiftInUse && !inv[slot].IsAir && !inv[slot].favorited) {
                if (ArchitectureGUI.Visible && context == ItemSlot.Context.InventoryItem &&
                    ArchitectureGUI.ItemSlot.Any(s => s.Value.CanPlaceItem(inv[slot]))) {
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
            if (Player.chest == -1 & Player.talkNPC == -1 && !inventory[slot].IsAir && !inventory[slot].favorited) {
                if (ArchitectureGUI.Visible && context == ItemSlot.Context.InventoryItem) {
                    foreach (var itemSlot in from s in ArchitectureGUI.ItemSlot
                                             where s.Value.CanPlaceItem(inventory[slot])
                                             select s) {
                        // 放到建筑GUI里面
                        ref Item slotItem = ref itemSlot.Value.Item;
                        if (slotItem.IsAir) {
                            slotItem = inventory[slot].Clone();
                            inventory[slot].TurnToAir();
                            // 调用这个才能存物品
                            itemSlot.Value.ItemChange();
                        }
                        else if (slotItem.stack < slotItem.maxStack && ItemLoader.CanStack(slotItem, inventory[slot])) {
                            int stackAvailable = slotItem.maxStack - slotItem.stack;
                            int stackAddition = Math.Min(inventory[slot].stack, stackAvailable);
                            inventory[slot].stack -= stackAddition;
                            slotItem.stack += stackAddition;
                            // 调用这个才能存物品
                            itemSlot.Value.ItemChange();
                        }
                        SoundEngine.PlaySound(SoundID.Grab);
                        Recipe.FindRecipes();
                    }
                }
            }
            return base.ShiftClickSlot(inventory, context, slot);
        }
    }
}
