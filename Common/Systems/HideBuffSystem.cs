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
                    foreach (var potion in from p in potionBag.storedPotions where p.stack >= MyUtils.Config.NoConsume_PotionRequirement select p) {
                        BuffTypesShouldHide[potion.buffType] = true;
                    }
                }
            }
        }

        public static bool HasCampfire { get; set; }
        public static bool HasHeartLantern { get; set; }
        public static bool HasSunflower { get; set; }
        public static bool HasGardenGnome { get; set; }
        public static bool HasStarInBottle { get; set; }

        // 这个一般来说只会在客户端执行
        public override void ResetNearbyTileEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // 原版就没拿Buff判断篝火心灯，所以这里得专门判断
            if (HasCampfire)
                Main.SceneMetrics.HasCampfire = true;
            if (HasHeartLantern)
                Main.SceneMetrics.HasHeartLantern = true;
            // 顺便带上其他的好了
            if (HasSunflower)
                Main.SceneMetrics.HasSunflower = true;
            if (HasGardenGnome)
                Main.SceneMetrics.HasGardenGnome = true;
            if (HasStarInBottle)
                Main.SceneMetrics.HasStarInBottle = true;

            HasCampfire = false;
            HasHeartLantern = false;
            HasSunflower = false;
            HasGardenGnome = false;
            HasStarInBottle = false;
        }
    }
}
