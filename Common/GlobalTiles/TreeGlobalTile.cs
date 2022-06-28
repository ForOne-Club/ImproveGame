using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Common.GlobalTiles
{
    public class TreeGlobalTile : GlobalTile
    {
        // 秉承着能不用IL就不用的稳定至上原则，这里直接RandomUpdate了（其实就是懒）
        public override void RandomUpdate(int i, int j, int type) {
            // HasTile为false一般不可能，但还是判断一下
            if (!MyUtils.Config.TreeGrowFaster || !Main.tile[i, j].HasTile) {
                return;
            }
            // 原版试了一次，我们这里多尝试4次不就5倍了吗
            for (int time = 0; time < 4; time++) {
                switch (type) {
                    case TileID.Saplings:
                        if ((Main.tile[i, j].TileFrameX < 324 || Main.tile[i, j].TileFrameX >= 540) ? WorldGen.GrowTree(i, j) : WorldGen.GrowPalmTree(i, j) && WorldGen.PlayerLOS(i, j))
                            WorldGen.TreeGrowFXCheck(i, j);
                        return;
                    case TileID.VanityTreeSakuraSaplings:
                    case TileID.VanityTreeWillowSaplings:
                        // 这俩树木的树木ID都是种子ID+1, 直接放一起了
                        if (WorldGen.genRand.NextBool(5) && WorldGen.TryGrowingTreeByType(type + 1, i, j) && WorldGen.PlayerLOS(i, j))
                            WorldGen.TreeGrowFXCheck(i, j);
                        return;
                    case TileID.GemSaplings:
                        if (WorldGen.genRand.NextBool(5)) {
                            int style = Main.tile[i, j].TileFrameX / 54;
                            int treeTileType = TileID.TreeTopaz + style; // 六种宝石树ID连一起的

                            if (WorldGen.TryGrowingTreeByType(treeTileType, i, j) && WorldGen.PlayerLOS(i, j))
                                WorldGen.TreeGrowFXCheck(i, j);
                        }
                        return;
                }
                // Mod的，让他自己执行自己的RandomUpdate
                if (TileID.Sets.TreeSapling[type]) {
                    TileLoader.GetTile(type)?.RandomUpdate(i, j);
                }
            }
        }
    }
}
