using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Items;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.ExtremeStorage;
using Terraria.DataStructures;

namespace ImproveGame.Content.Patches
{
    public class HideBuffSystem : ModSystem
    {
        internal static bool[] BuffTypesShouldHide = new bool[BuffLoader.BuffCount];

        public override void PostSetupContent()
        {
            Array.Resize(ref BuffTypesShouldHide, BuffLoader.BuffCount);
        }

        public static int HideBuffCount()
        {
            int count = 0;
            for (int i = 0; i < BuffTypesShouldHide.Length; i++)
            {
                if (BuffTypesShouldHide[i])
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 与 <see cref="InfBuffPlayer.PostUpdateBuffs"/> 相关，要更改的时候记得改那边
        /// </summary>
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            Array.Clear(BuffTypesShouldHide, 0, BuffTypesShouldHide.Length);
            HideGlobalBuff.HidedBuffCountThisFrame = 0;

            SetupShouldHideArrayFromPlayer(Main.LocalPlayer);
            if (Config.ShareInfBuffs)
                CheckTeamPlayers(Main.LocalPlayer.whoAmI, SetupShouldHideArrayFromPlayer);

            // 从TE中获取所有的无尽Buff物品
            foreach ((int _, TileEntity tileEntity) in TileEntity.ByID)
            {
                if (tileEntity is not TEExtremeStorage {UseUnlimitedBuffs: true} storage)
                {
                    continue;
                }

                var alchemyItems = storage.FindAllNearbyChestsWithGroup(ItemGroup.Alchemy);
                alchemyItems.ForEach(i => SetupShouldHideArray(InfBuffPlayer.GetAvailableItemsFromItems(Main.chest[i].item)));
            }
        }

        private static void SetupShouldHideArrayFromPlayer(Player player) =>
            SetupShouldHideArray(InfBuffPlayer.Get(player).AvailableItems);
        
        /// <summary>
        /// 更新InventoryGlow和BuffTypesShouldHide
        /// </summary>
        private static void SetupShouldHideArray(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                ApplyBuffItem.UpdateInventoryGlow(item);

                int buffType = ApplyBuffItem.GetItemBuffType(item);
                if (buffType is not -1)
                    BuffTypesShouldHide[buffType] = true;

                if (!item.IsAir && item.ModItem is PotionBag potionBag && potionBag.StoredPotions.Count > 0)
                    foreach (var potion in from p in potionBag.StoredPotions where p.stack >= Config.NoConsume_PotionRequirement select p)
                        BuffTypesShouldHide[potion.buffType] = true;
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