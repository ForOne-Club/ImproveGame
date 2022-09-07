using Terraria.Enums;
using Terraria.Utilities;

namespace ImproveGame.Common.Utils
{
    // 这里主要放一些战利表，比如摇树可能掉落、普通箱子战利品之类的
    public class CollectHelper : ModSystem
    {
        internal static bool[] ItemCanRightClick;

        public override void PostSetupContent()
        {
            ItemCanRightClick = ItemID.Sets.Factory.CreateBoolSet(false,
                ItemID.HerbBag,
                ItemID.CanOfWorms,
                ItemID.Oyster,
                ItemID.CapricornLegs,
                ItemID.CapricornTail,
                ItemID.GoodieBag,
                ItemID.LockBox,
                ItemID.ObsidianLockbox,
                ItemID.Present,
                ItemID.BluePresent,
                ItemID.GreenPresent,
                ItemID.YellowPresent
            );
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                if (ItemID.Sets.IsFishingCrate[i] || ItemID.Sets.BossBag[i])
                {
                    ItemCanRightClick[i] = true;
                }
            }
        }

        /// <summary>
        /// 获取摇树水果（原版）
        /// </summary>
        /// <param name="treeType">树木TreeTypes</param>
        /// <returns>摇树物品ID</returns>
        public static int GetShakeTreeFruit(TreeTypes treeType) {
            switch (treeType) {
                case TreeTypes.Forest:
                    WeightedRandom<short> weightedRandom = new();
                    weightedRandom.Add(ItemID.Apple);
                    weightedRandom.Add(ItemID.Apricot);
                    weightedRandom.Add(ItemID.Peach);
                    weightedRandom.Add(ItemID.Grapefruit);
                    weightedRandom.Add(ItemID.Lemon);
                    return weightedRandom.Get();
                case TreeTypes.Snow:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Plum : ItemID.Cherry;
                case TreeTypes.Jungle:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Mango : ItemID.Pineapple;
                case TreeTypes.Palm:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Coconut : ItemID.Banana;
                case TreeTypes.Corrupt:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Elderberry : ItemID.BlackCurrant;
                case TreeTypes.Crimson:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.BloodOrange : ItemID.Rambutan;
                case TreeTypes.Hallowed:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Dragonfruit : ItemID.Starfruit;
                default:
                    return -1;
            }
        }
    }
}
