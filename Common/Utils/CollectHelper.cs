using Terraria.Enums;
using Terraria.Utilities;
using Terraria.ID;

namespace ImproveGame.Common.Utils
{
    // 这里主要放一些战利表，比如摇树可能掉落、普通箱子战利品之类的
    public class CollectHelper : ModSystem
    {
        public override void PostSetupContent()
        {
        }

        public static List<Item> GetShimmerResult(Item input, out int stackRequired)
        {
            stackRequired = 1;
            int shimmerEquivalentType = input.GetShimmerEquivalentType();
            int decraftingRecipeIndex = ShimmerTransforms.GetDecraftingRecipeIndex(shimmerEquivalentType);
            switch (shimmerEquivalentType)
            {
                case 1326 when NPC.downedMoonlord:
                    return new List<Item> {new(5335)};
                case 779 when NPC.downedMoonlord:
                    return new List<Item> {new(5134)};
                case 3031 when NPC.downedMoonlord:
                    return new List<Item> {new(5364)};
                case 5364 when NPC.downedMoonlord:
                    return new List<Item> {new(3031)};
                case 3461:
                    {
                        short type = Main.GetMoonPhase() switch
                        {
                            MoonPhase.QuarterAtRight => 5407,
                            MoonPhase.HalfAtRight => 5405,
                            MoonPhase.ThreeQuartersAtRight => 5404,
                            MoonPhase.Full => 5408,
                            MoonPhase.ThreeQuartersAtLeft => 5401,
                            MoonPhase.HalfAtLeft => 5403,
                            MoonPhase.QuarterAtLeft => 5402,
                            _ => 5406
                        };

                        return new List<Item> {new(type)};
                    }
                default:
                    {
                        if (input.createTile is TileID.MusicBoxes)
                        {
                            return new List<Item> {new(576)};
                        }

                        if (ItemID.Sets.ShimmerTransformToItem[shimmerEquivalentType] > 0)
                        {
                            return new List<Item> {new(ItemID.Sets.ShimmerTransformToItem[shimmerEquivalentType])};
                        }

                        if (decraftingRecipeIndex >= 0)
                        {
                            Recipe recipe = Main.recipe[decraftingRecipeIndex];
                            stackRequired = recipe.createItem.stack;
                            List<Item> enumerable = recipe.requiredItem;
                            if (recipe.customShimmerResults != null)
                                enumerable = recipe.customShimmerResults;
                            return enumerable;
                        }

                        break;
                    }
            }

            return null;
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
                case TreeTypes.Palm or TreeTypes.PalmCorrupt or TreeTypes.PalmCrimson or TreeTypes.PalmHallowed:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Coconut : ItemID.Banana;
                case TreeTypes.Corrupt or TreeTypes.PalmCorrupt:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Elderberry : ItemID.BlackCurrant;
                case TreeTypes.Crimson or TreeTypes.PalmCrimson:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.BloodOrange : ItemID.Rambutan;
                case TreeTypes.Hallowed or TreeTypes.PalmHallowed:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.Dragonfruit : ItemID.Starfruit;
                case TreeTypes.Ash:
                    return (!WorldGen.genRand.NextBool(2)) ? ItemID.SpicyPepper : ItemID.Pomegranate;
                default:
                    return -1;
            }
        }
    }
}
