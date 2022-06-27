using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.GlobalTiles
{
    public class TreeGlobalTile : GlobalTile
    {
        public override void Load() {
            IL.Terraria.WorldGen.ShakeTree += TweakShakeTree;
        }

        // 源码，在最后：
        // if (flag) {
        //     [摇树有物品出现，执行一些特效]
        // }
        // 搞到这个flag, 如果为false(没东西)就加水果, 然后让他读到true
        // IL_0DAF: ldloc.s   flag
        // IL_0DB1: brfalse.s IL_0E12
        // 这两行就可以精确找到, 因为其他地方没有相同的
        // 值得注意的是，代码开始之前有这个：
        // treeShakeX[numTreeShakes] = x;
        // treeShakeY[numTreeShakes] = y;
        // numTreeShakes++;
        // 所以我们可以直接用了，都不需要委托获得x, y
        private void TweakShakeTree(ILContext il) {
            try {
                ILCursor c = new(il);

                if (!c.TryGotoNext(MoveType.Before,
                                   i => i.Match(OpCodes.Ldloc_S),
                                   i => i.Match(OpCodes.Brfalse_S))) {
                    ErrorTweak();
                    return;
                }

                c.Index++;
                c.EmitDelegate<Func<bool, bool>>((shackSucceed) => {
                    if (!shackSucceed && MyUtils.Config.ShakeTreeFruit) {
                        int x = WorldGen.treeShakeX[WorldGen.numTreeShakes - 1];
                        int y = WorldGen.treeShakeY[WorldGen.numTreeShakes - 1];
                        int tileType = Main.tile[x, y].TileType;
                        TreeTypes treeType = WorldGen.GetTreeType(tileType);

                        // 获取到顶部
                        y--;
                        while (y > 10 && Main.tile[x, y].HasTile && TileID.Sets.IsShakeable[Main.tile[x, y].TileType]) {
                            y--;
                        }
                        y++;

                        int fruit = MyUtils.GetShakeTreeFruit(treeType);
                        if (fruit > -1) {
                            Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, fruit);
                            shackSucceed = true;
                        }
                    }
                    return shackSucceed;
                });

            }
            catch {
                ErrorTweak();
                return;
            }
        }

        private static void ErrorTweak() {
            string exception = "Something went wrong in TweakShakeTree(), please contact with the mod developers.";
            if (GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive)
                exception = "TweakShakeTree()发生错误，请联系Mod制作者";
            throw new Exception(exception);
        }

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
                            int treeTileType = 587;
                            switch (style) {
                                case 0:
                                    treeTileType = 583;
                                    break;
                                case 1:
                                    treeTileType = 584;
                                    break;
                                case 2:
                                    treeTileType = 585;
                                    break;
                                case 3:
                                    treeTileType = 586;
                                    break;
                                case 4:
                                    treeTileType = 587;
                                    break;
                                case 5:
                                    treeTileType = 588;
                                    break;
                                case 6:
                                    treeTileType = 589;
                                    break;
                            }

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
