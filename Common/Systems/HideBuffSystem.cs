using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Content.Items;
using Microsoft.Xna.Framework.Graphics;

namespace ImproveGame.Common.Systems
{
    public class HideBuffSystem : ModSystem
    {
        internal static bool[] BuffTypesShouldHide = new bool[BuffLoader.BuffCount];

        public override void PostSetupContent() {
            Array.Resize(ref BuffTypesShouldHide, BuffLoader.BuffCount);
        }

        public static void ClearHideBuffArray(){
            for (int i = 0; i < BuffTypesShouldHide.Length; i++) {
                BuffTypesShouldHide[i] = false;
            }
        }

        public static int HideBuffCount() {
            int count = 0;
            for (int i = 0; i < BuffTypesShouldHide.Length; i++) {
                if (BuffTypesShouldHide[i])
                    count++;
            }
            return count;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            // 专门用来清除ApplyBuffItem.BuffTypesShouldHide的
            ClearHideBuffArray();
            HideGlobalBuff.HidedBuffCountThisFrame = 0;

            var items = MyUtils.GetAllInventoryItemsList(Main.LocalPlayer, false);
            foreach (var item in items) {
                ApplyBuffItem.UpdateInventoryGlow(item);
                if (!item.IsAir && item.type == ModContent.ItemType<PotionBag>() &&
                    item.ModItem is not null && item.ModItem is PotionBag &&
                    (item.ModItem as PotionBag).storedPotions.Count > 0) {
                    var potionBag = item.ModItem as PotionBag;
                    foreach (var potion in from p in potionBag.storedPotions where p.stack >= 30 select p) {
                        BuffTypesShouldHide[potion.buffType] = true;
                    }
                }
            }
        }
    }
}
