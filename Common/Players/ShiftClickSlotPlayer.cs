using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using Terraria;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;

namespace ImproveGame.Common.Players
{
    public class ShiftClickSlotPlayer : ModPlayer
    {
        /// <summary>
        /// Shift左键单击物品栏
        /// </summary>
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot) {
            if (Player.chest == -1 & Player.talkNPC == -1) {
                if (ArchitectureGUI.Visible && context == ItemSlot.Context.InventoryItem) {
                    foreach (var itemSlot in from s in ArchitectureGUI.ItemSlot
                                             where s.Value.CanPlaceItem(inventory[slot])
                                             select s) {
                        // 放到建筑GUI里面
                        Utils.Swap(ref inventory[slot], ref itemSlot.Value.Item);
                        SoundEngine.PlaySound(SoundID.Grab);
                        Recipe.FindRecipes();
                    }
                }
            }
            return base.ShiftClickSlot(inventory, context, slot);
        }
    }
}
