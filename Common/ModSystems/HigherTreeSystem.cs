using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ImproveGame.Common.ModSystems
{
    public class HigherTreeSystem : ModSystem
    {
        private void ModifyPalmTrees(ILContext il) {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchCall<WorldGen>("get_genRand"),
                                       i => i.Match(OpCodes.Ldc_I4_S, (sbyte)10));
            c.EmitDelegate((int min) => Config.PalmTreeMin);
            c.GotoNext(MoveType.After, i => i.Match(OpCodes.Ldc_I4_S, (sbyte)21));
            c.EmitDelegate((int max) => Config.PalmTreeMax + 1);
        }

        // 通过简单的IL修改一般树木高度
        private void ModifyMostTrees(ILContext il) {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(5));
            c.EmitDelegate((int min) => Config.MostTreeMin);
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(17));
            c.EmitDelegate((int max) => Config.MostTreeMax + 1);
        }

        public override void Load() {
            Terraria.IL_WorldGen.GrowTree += ModifyMostTrees;
            Terraria.IL_WorldGen.GrowPalmTree += ModifyPalmTrees;
            Terraria.On_WorldGen.SetGemTreeDrops += GemAlwaysTweak;
            SetTreeHeights(Config.GemTreeMin, Config.GemTreeMax, Config.MostTreeMin, Config.MostTreeMax);
        }

        private void GemAlwaysTweak(Terraria.On_WorldGen.orig_SetGemTreeDrops orig, int gemType, int seedType, Tile tileCache, ref int dropItem, ref int secondaryItem) {
            if (Config.GemTreeAlwaysDropGem) {
                dropItem = gemType;
                secondaryItem = seedType;
            }
            else {
                orig.Invoke(gemType, seedType, tileCache, ref dropItem, ref secondaryItem);
            }
        }

        public override void Unload() {
            SetTreeHeights(7, 12, 7, 12);
        }

        // 宝石树高度直接这么改就行了，很方便
        public static void SetTreeHeights(int gemMin, int gemMax, int mostMin, int mostMax) {
            WorldGen.GrowTreeSettings.Profiles.GemTree_Ruby.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Ruby.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Diamond.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Diamond.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Topaz.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Topaz.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Amethyst.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Amethyst.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Sappphire.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Sappphire.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Emerald.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Emerald.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Amber.TreeHeightMin = gemMin;
            WorldGen.GrowTreeSettings.Profiles.GemTree_Amber.TreeHeightMax = gemMax;
            WorldGen.GrowTreeSettings.Profiles.VanityTree_Sakura.TreeHeightMin = mostMin;
            WorldGen.GrowTreeSettings.Profiles.VanityTree_Sakura.TreeHeightMax = mostMax;
            WorldGen.GrowTreeSettings.Profiles.VanityTree_Willow.TreeHeightMin = mostMin;
            WorldGen.GrowTreeSettings.Profiles.VanityTree_Willow.TreeHeightMax = mostMax;
        }
    }
}
