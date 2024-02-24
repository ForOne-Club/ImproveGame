using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Items.ItemContainer;

namespace ImproveGame.Content.Functions.PortableBuff
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
            SetupShouldHideArray(InfBuffPlayer.Get(Main.LocalPlayer).ExStorageAvailableItems);
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
                var buffTypes = ApplyBuffItem.GetItemBuffType(item);
                buffTypes.ForEach((Action<int>)(buffType =>
                {
                    if (buffType is not -1)
                        BuffTypesShouldHide[buffType] = true;

                    if (!item.IsAir && item.ModItem is PotionBag potionBag && potionBag.ItemContainer.Count > 0)
                        foreach (var potion in from p in potionBag.ItemContainer where p.stack >= Config.NoConsume_PotionRequirement select p)
                            BuffTypesShouldHide[(int)potion.buffType] = true;
                }));
            }
        }
    }
}