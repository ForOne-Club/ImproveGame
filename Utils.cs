using ImproveGame.Common.Configs;
using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static ImproveGame.Common.GlobalItems.ImproveItem;

namespace ImproveGame
{
    public class Utils
    {
        // 获取配置
        public static ImproveConfigs GetConfig()
        {
            return ModContent.GetInstance<ImproveConfigs>();
        }

        // 获取平台总数
        public static bool GetPlatformCount(Item[] inv, ref int count)
        {
            bool consumable = true;
            for (int i = 0; i < 50; i++)
            {
                Item item = inv[i];
                if (item.createTile != -1 && TileID.Sets.Platforms[item.createTile])
                {
                    count += item.stack;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, Main.player[item.playerIndexTheItemIsReservedFor]))
                    {
                        consumable = false;
                    }
                }
            }
            return consumable;
        }

        // 获取平台总数
        public static bool GetWallCount(Item[] inv, ref int count)
        {
            bool consumable = true;
            for (int i = 0; i < 50; i++)
            {
                Item item = inv[i];
                if (item.createWall > 0)
                {
                    count += item.stack;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, Main.player[item.playerIndexTheItemIsReservedFor]))
                    {
                        consumable = false;
                    }
                }
            }
            return consumable;
        }

        // 获取背包第一个平台
        public static Item GetFirstPlatform(Player player)
        {
            for (int i = 0; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item.stack > 0 && item.createTile > -1 && TileID.Sets.Platforms[item.createTile])
                {
                    return item;
                }
            }
            return new Item();
        }

        // 获取背包第一个平台
        public static Item GetFirstWall(Player player)
        {
            for (int i = 0; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item.stack > 0 && item.createWall > 0)
                {
                    return item;
                }
            }
            return new Item();
        }

        // 加载前缀
        public static void LoadPrefixInfo()
        {
            //
            PrefixLevel.Add(1, 1);
            PrefixLevel.Add(2, 1);
            PrefixLevel.Add(3, 1);
            PrefixLevel.Add(4, 2);
            PrefixLevel.Add(5, 1);
            PrefixLevel.Add(6, 1);
            PrefixLevel.Add(7, 0);
            PrefixLevel.Add(8, 0);
            PrefixLevel.Add(9, 0);
            PrefixLevel.Add(10, 0);
            PrefixLevel.Add(11, 0);
            PrefixLevel.Add(12, 1);
            PrefixLevel.Add(13, 0);
            PrefixLevel.Add(14, 1);
            PrefixLevel.Add(15, 1);
            // 射手
            PrefixLevel.Add(16, 1);
            PrefixLevel.Add(17, 2);
            PrefixLevel.Add(18, 2);
            PrefixLevel.Add(19, 1);
            PrefixLevel.Add(20, 2);
            PrefixLevel.Add(21, 1);
            PrefixLevel.Add(22, 0);
            PrefixLevel.Add(23, 0);
            PrefixLevel.Add(24, 0);
            PrefixLevel.Add(25, 1);
            // 法师
            PrefixLevel.Add(26, 2);
            PrefixLevel.Add(27, 1);
            PrefixLevel.Add(28, 2);
            PrefixLevel.Add(29, 0);
            PrefixLevel.Add(30, 0);
            PrefixLevel.Add(31, 0);
            PrefixLevel.Add(32, 0);
            PrefixLevel.Add(33, 1);
            PrefixLevel.Add(34, 1);
            PrefixLevel.Add(35, 1);
            // 通用
            PrefixLevel.Add(36, 1);
            PrefixLevel.Add(37, 2);
            PrefixLevel.Add(38, 1);
            PrefixLevel.Add(39, 0);
            PrefixLevel.Add(40, 0);
            PrefixLevel.Add(41, 0);
            // 公共
            PrefixLevel.Add(42, 1);
            PrefixLevel.Add(43, 2);
            PrefixLevel.Add(44, 1);
            PrefixLevel.Add(45, 1);
            PrefixLevel.Add(46, 1);
            PrefixLevel.Add(47, 0);
            PrefixLevel.Add(48, 0);
            PrefixLevel.Add(49, 0);
            PrefixLevel.Add(50, 0);
            PrefixLevel.Add(51, 1);

            PrefixLevel.Add(52, 1);

            PrefixLevel.Add(53, 1);
            PrefixLevel.Add(54, 1);
            PrefixLevel.Add(55, 1);
            PrefixLevel.Add(56, 0);
            PrefixLevel.Add(57, 1);

            // 暴怒
            PrefixLevel.Add(58, 0);
            // 公共
            PrefixLevel.Add(59, 2);
            PrefixLevel.Add(60, 2);
            PrefixLevel.Add(61, 1);

            // 顶级前缀
            PrefixLevel.Add(81, 3);
            PrefixLevel.Add(82, 3);
            PrefixLevel.Add(83, 3);
            PrefixLevel.Add(84, 3);
            // 饰品
            PrefixLevel.Add(62, 1);
            PrefixLevel.Add(69, 1);
            PrefixLevel.Add(73, 1);
            PrefixLevel.Add(77, 1);
            PrefixLevel.Add(63, 2);
            PrefixLevel.Add(70, 2);
            PrefixLevel.Add(74, 2);
            PrefixLevel.Add(78, 2);
            PrefixLevel.Add(67, 2);
            PrefixLevel.Add(64, 3);
            PrefixLevel.Add(71, 3);
            PrefixLevel.Add(75, 3);
            PrefixLevel.Add(79, 3);
            PrefixLevel.Add(65, 4);
            PrefixLevel.Add(72, 4);
            PrefixLevel.Add(76, 4);
            PrefixLevel.Add(80, 4);
            PrefixLevel.Add(68, 4);
            PrefixLevel.Add(66, 4);
        }

        // 魔法移除物块方法
        public static void KillTiles(Player player, Rectangle rectangle)
        {
            // Main.NewText(Main.SmartCursorIsUsed);
            // 获得背包中最好的镐子
            Item item = player.GetBestPickaxe();
            int minI = rectangle.X;
            int maxI = rectangle.X + rectangle.Width - 1;
            int minJ = rectangle.Y;
            int maxJ = rectangle.Y + rectangle.Height - 1;
            for (int i = 0; i < player.hitTile.data.Length; i++)
            {
                HitTile.HitTileObject hitTileObject = player.hitTile.data[i];
                hitTileObject.timeToLive = 10000;
            }
            for (int i = minI; i <= maxI; i++)
            {
                for (int j = minJ; j <= maxJ; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (!Main.tileAxe[tile.TileType] && !Main.tileHammer[tile.TileType])
                    {
                        player.PickTile(i, j, item.pick);
                        player.hitTile.data[player.hitTile.HitObject(i, j, 1)].timeToLive = 10000;
                    }
                }
            }
            for (int i = 0; i < player.hitTile.data.Length; i++)
            {
                HitTile.HitTileObject hitTileObject = player.hitTile.data[i];
                hitTileObject.timeToLive = 60;
            }
        }

        // 判断物块是否相同
        public static bool IsSameTile(int i, int j, int tileType, int tileStyle)
        {
            return (Main.tile[i, j].TileType == tileType && Main.tile[i, j].TileFrameY != tileStyle * 18)
                             || Main.tile[i, j].TileType != tileType;
        }

        // Tiles 外围
        public static List<TileInfo> PrisonTiles(int tileType, int tileStyle = 0)
        {
            List<TileInfo> PrisonTiles = new List<TileInfo>();
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (i == 0 || i == 10)
                    {
                        PrisonTiles.Add(new(tileType, tileStyle, i, j)); // Tile
                    }
                    else
                    {
                        if (j == 0 && i != 5)
                        {
                            PrisonTiles.Add(new(tileType, tileStyle, i, j)); // Tile
                        }
                        else if (j == 0 && i == 5)
                        {
                            PrisonTiles.Add(new(30, 0, i, j));
                        }
                        if (i == 5 && j == 2)
                        {
                            PrisonTiles.Add(new(4, 0, i, j)); // 火把
                        }
                        if (j == 5)
                        {
                            if (i == 3)
                            {
                                PrisonTiles.Add(new(15, 0, i, j)); // 凳子
                            }
                            if (i == 5)
                            {
                                PrisonTiles.Add(new(14, 0, i, j)); // 桌子
                            }
                            if (i == 7)
                            {
                                PrisonTiles.Add(new(15, 0, i, j)); // 凳子
                            }
                        }
                    }
                }
            }
            return PrisonTiles;
        }

        // Walls 外围
        public static List<WallInfo> PrisonWalls(int wallType)
        {
            List<WallInfo> PrisonWalls = new List<WallInfo>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    PrisonWalls.Add(new(wallType, i, j));
                }
            }
            return PrisonWalls;
        }
    }
}
