using ImproveGame.Content.Functions.Construction;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;

public partial class CreateWand
{
    private static TagCompound CreateStructureTagFromColors(Color[] colors, int width, int height)
    {
        var Tag = new TagCompound
        {
            { "BuildTime", "" },
            { "ModVersion", "" },
            { "Width", (short)(width - 1) },
            { "Height", (short)(height - 1) },
            { "OriginX", (short)0 },
            { "OriginY", (short)0 }
        };
        Dictionary<string, ushort> entries = [];
        List<TileDefinition> data = [];
        List<string> signTexts = [];

        TileInfo[,] sorts = new TileInfo[width, height];
        HashSet<Point> multitileOverrideCoords = [];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var sort = Color2TileInfo(colors[x + y * width]);
                if (multitileOverrideCoords.Contains(new(x, y))) 
                {
                    sorts[x, y].HasWall = sort.HasWall;
                    continue;
                }
                sorts[x, y] = sort;


                switch (sort.Sort)
                {
                    // 3x2
                    case TileSort.Table:
                    case TileSort.Dresser:
                    case TileSort.Piano:
                    case TileSort.Sofa:
                    case TileSort.Campfire:
                        for (int u = 0; u < 3; u++)
                        {
                            for (int v = 0; v < 2; v++)
                            {
                                sorts[x + u - 1, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x + u - 1, y - v));
                            }
                        }
                        break;

                    // 2x2
                    case TileSort.Chest:
                        for (int u = 0; u < 2; u++)
                        {
                            for (int v = 0; v < 2; v++)
                            {
                                sorts[x + u, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x + u, y - v));
                            }
                        }
                        break;
                    case TileSort.Sink:
                        for (int u = 0; u < 2; u++)
                        {
                            for (int v = 0; v < 2; v++)
                            {
                                sorts[x - u, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x - u, y - v));
                            }
                        }
                        break;

                    case TileSort.Candelabra:
                        for (int u = 0; u < 2; u++)
                        {
                            for (int v = 0; v < 2; v++)
                            {
                                sorts[x - u, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x - u, y - v));
                            }
                        }
                        break;

                    // 3x4
                    case TileSort.Bookcase:
                        for (int u = 0; u < 3; u++)
                        {
                            for (int v = 0; v < 4; v++)
                            {
                                sorts[x + u - 1, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x + u - 1, y - v));
                            }
                        }
                        break;

                    // 4x2
                    case TileSort.Bed:
                    case TileSort.Bathtub:
                        for (int u = 0; u < 4; u++)
                        {
                            for (int v = 0; v < 2; v++)
                            {
                                sorts[x + u - 1, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x + u - 1, y - v));
                            }
                        }
                        break;

                    // 1x2
                    case TileSort.Chair:
                    case TileSort.Toilet:
                        sorts[x, y - 1].Sort = sort.Sort;
                        break;

                    // 2x1
                    case TileSort.Workbench:
                        sorts[x + 1, y].Sort = TileSort.Workbench;
                        multitileOverrideCoords.Add(new(x + 1, y));
                        break;

                    // 1x3
                    case TileSort.Lamp:
                    case TileSort.Door:
                        sorts[x, y - 1].Sort = sort.Sort;
                        sorts[x, y - 2].Sort = sort.Sort;
                        break;

                    // 3x3
                    case TileSort.Chandelier:
                        for (int u = 0; u < 3; u++)
                        {
                            for (int v = 0; v < 3; v++)
                            {
                                sorts[x + u - 1, y + v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x + u - 1, y + v));
                            }
                        }
                        break;

                    // 2x5
                    case TileSort.Clock:
                        for (int u = 0; u < 2; u++)
                        {
                            for (int v = 0; v < 5; v++)
                            {
                                sorts[x + u, y - v].Sort = sort.Sort;
                                multitileOverrideCoords.Add(new(x + u, y - v));
                            }
                        }
                        break;

                    // 1x2
                    case TileSort.Lantern:
                        sorts[x, y + 1].Sort = sort.Sort;
                        multitileOverrideCoords.Add(new(x, y + 1));
                        break;


                }
            }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileInfo sort = sorts[x, y];
                short tileIndex = (short)(sort.Sort switch
                {
                    TileSort.Block => TileID.WoodBlock,
                    TileSort.Platform => TileID.Platforms,
                    TileSort.Torch => TileID.Torches,
                    TileSort.Chair => TileID.Chairs,
                    TileSort.Table => TileID.Tables,
                    TileSort.Workbench => TileID.WorkBenches,
                    TileSort.Bed => TileID.Beds,
                    TileSort.Door => TileID.ClosedDoor,
                    TileSort.Chest => TileID.Containers,
                    TileSort.Bookcase => TileID.Bookcases,
                    TileSort.Bathtub => TileID.Bathtubs,
                    TileSort.Candelabra => TileID.Candelabras,
                    TileSort.Candle => TileID.Candles,
                    TileSort.Chandelier => TileID.Chandeliers,
                    TileSort.Clock => TileID.GrandfatherClocks,
                    TileSort.Dresser => TileID.Dressers,
                    TileSort.Lamp => TileID.Lamps,
                    TileSort.Lantern => TileID.HangingLanterns,
                    TileSort.Piano => TileID.Pianos,
                    TileSort.Sink => TileID.Sinks,
                    TileSort.Sofa => TileID.Benches,
                    TileSort.Toilet => TileID.Chairs,
                    TileSort.Campfire => TileID.Campfire,
                    _ => -1
                });
                short wallIndex = sort.HasWall ? (short)WallID.Wood : (short)-1;
                // 设置为Solid
                var extraDatas = new BitsByte(b5: true);//TileDefinition.GetExtraData(tile);
                var extraDatas2 = new BitsByte();
                var tile = new Tile(); // TODO 提供墙壁帧等信息
                FramingTiles(x, y, sort, sorts, width, height, out var tfx, out var tfy, out var wfx, out var wfy);
                tile.TileFrameX = (short)tfx;
                tile.TileFrameY = (short)tfy;
                tile.WallFrameX = wfx;
                tile.WallFrameY = wfy;
                data.Add(
                    new TileDefinition(
                        tileIndex,
                        wallIndex,
                        tile,
                        extraDatas,
                        extraDatas2
                    ));
            }
        }

        Tag.Add("SignTexts", signTexts);
        Tag.Add("StructureData", data);

        var stringList = new List<string>();
        var indexList = new List<ushort>();
        Tag.Add("EntriesName", stringList);
        Tag.Add("EntriesType", indexList);
        return Tag;
    }

    private static void FramingTiles(
        int i,
        int j,
        TileInfo current,
        TileInfo[,] datas,
        int width,
        int height,
        out int tileFrameX,
        out int tileFrameY,
        out int wallFrameX,
        out int wallFrameY)
    {

        static void MultiTileFraming(
            TileInfo[,] datas,
            TileSort sort,
            int i,
            int j,
            int width,
            int height,
            out int tileFrameX,
            out int tileFrameY)
        {
            int offX = 0;
            int offY = 0;
            while (i >= offX && datas[i - offX, j].Sort == sort)
                offX++;
            offX--;

            while (j >= offY && datas[i, j - offY].Sort == sort)
                offY++;
            offY--;
            offX %= width;
            if (offY >= height) offY = height - 1;
            tileFrameX = offX * 18;
            tileFrameY = offY * 18;
        }

        TileSort sort = current.Sort;
        wallFrameX = wallFrameY = 0;
        switch (current.Sort)
        {
            case TileSort.Block:
                {
                    bool leftLinked = i > 0 && datas[i - 1, j].Sort is TileSort.Block;
                    bool rightLinked = i < width - 1 && datas[i + 1, j].Sort is TileSort.Block;
                    bool upLinked = j > 0 && datas[i, j - 1].Sort is TileSort.Block;
                    bool downLinked = j < height - 1 && datas[i, j + 1].Sort is TileSort.Block;
                    int index = 0;
                    if (leftLinked) index += 1 << 0;
                    if (rightLinked) index += 1 << 1;
                    if (upLinked) index += 1 << 2;
                    if (downLinked) index += 1 << 3;
                    if (index != 15)
                        (tileFrameX, tileFrameY) = index switch
                        {
                            0 => (Main.rand.Next(9, 12), 3), // 不和任意方块连接
                            1 => (12, Main.rand.Next(0, 3)), // 仅和左侧相连
                            2 => (9, Main.rand.Next(0, 3)), // 仅和右侧相连
                            3 => (Main.rand.Next(6, 9), 4), // 和左右侧相连
                            4 => (Main.rand.Next(6, 9), 3), // 仅和上侧相连
                            5 => (Main.rand.Next([1, 3, 5]), 4), // 和左上侧相连
                            6 => (Main.rand.Next([0, 2, 4]), 4), // 和右上侧相连
                            7 => (Main.rand.Next(1, 4), 2), // 和左右上侧相连
                            8 => (Main.rand.Next(6, 9), 0), // 仅和下侧相连
                            9 => (Main.rand.Next([1, 3, 5]), 3), // 和左下侧相连
                            10 => (Main.rand.Next([0, 2, 4]), 3), // 和右下侧相连
                            11 => (Main.rand.Next(1, 4), 0), // 和左右下侧相连
                            12 => (5, Main.rand.Next(0, 3)), // 和上下侧相连
                            13 => (4, Main.rand.Next(0, 3)), // 和左上下侧相连
                            14 => (0, Main.rand.Next(0, 3)), // 和右上下侧相连
                            15 => (Main.rand.Next(6, 9), 1), // 和上下左右相连
                            _ => (9, 3)
                        };
                    else
                    {
                        bool leftUp = i > 0 && j > 0 && datas[i - 1, j - 1].Sort is TileSort.Block;
                        bool leftDown = i > 0 && j < height - 1 && datas[i - 1, j + 1].Sort is TileSort.Block;
                        bool rightUp = i < width - 1 && j > 0 && datas[i + 1, j - 1].Sort is TileSort.Block;
                        bool rightDown = i < width - 1 && j < height - 1 && datas[i + 1, j + 1].Sort is TileSort.Block;

                        if ((leftUp && rightDown) ||
                            (rightUp && leftDown))
                        {
                            // 1001
                            // 1011
                            // 1101
                            // 1111
                            // 0110
                            // 0111
                            // 1110

                            (tileFrameX, tileFrameY) = (Main.rand.Next(1, 4), 1);
                        }
                        else if (leftUp && leftDown)
                        {
                            // 0011

                            (tileFrameX, tileFrameY) = (11, Main.rand.Next(0, 3));
                        }
                        else if (rightUp && rightDown)
                        {
                            // 1100
                            (tileFrameX, tileFrameY) = (10, Main.rand.Next(0, 3));
                        }
                        else if (leftUp || rightUp)
                        {
                            // 0001
                            // 0101

                            // 0100
                            (tileFrameX, tileFrameY) = (Main.rand.Next(6, 9), 2);
                        }
                        else
                        {
                            // 0000
                            // 0010
                            // 1000
                            // 1010
                            (tileFrameX, tileFrameY) = (Main.rand.Next(6, 9), 1);
                        }

                    }
                    tileFrameX *= 18;
                    tileFrameY *= 18;
                    break;
                }

            // 1x2
            case TileSort.Chair:
                {
                    tileFrameX = 18;
                    tileFrameY = j > 0 && datas[i, j - 1].Sort is TileSort.Chair ? 18 : 0;
                    break;
                }
            case TileSort.Toilet:
                {
                    tileFrameX = 18;
                    tileFrameY = j > 0 && datas[i, j - 1].Sort is TileSort.Toilet ? 58 : 40;
                    break;
                }
            case TileSort.Lantern:
                {
                    tileFrameX = 0;
                    tileFrameY = j > 0 && datas[i, j - 1].Sort is TileSort.Lantern ? 18 : 0;
                    break;
                }

            // 3x2
            case TileSort.Table:
            case TileSort.Dresser:
            case TileSort.Piano:
            case TileSort.Sofa:
            case TileSort.Campfire:
                {
                    MultiTileFraming(datas, sort, i, j, 3, 2, out tileFrameX, out tileFrameY);
                    break;
                }

            // 2x2
            case TileSort.Chest:
            case TileSort.Candelabra:
            case TileSort.Sink:
                {
                    MultiTileFraming(datas, sort, i, j, 2, 2, out tileFrameX, out tileFrameY);
                    break;
                }

            // 3x4
            case TileSort.Bookcase:
                {
                    MultiTileFraming(datas, sort, i, j, 3, 4, out tileFrameX, out tileFrameY);
                    break;
                }

            // 4x2
            case TileSort.Bed:
            case TileSort.Bathtub:
                {
                    MultiTileFraming(datas, sort, i, j, 4, 2, out tileFrameX, out tileFrameY);
                    tileFrameX += 72;
                    break;
                }


            case TileSort.Platform:
                {
                    tileFrameY = 0;
                    int index = 0;
                    if (i > 0)
                    {
                        var info = datas[i - 1, j];
                        if (info.Sort is TileSort.Block)
                            index += 1;
                        if (info.Sort is TileSort.Platform)
                            index += 2;
                    }
                    if (i < width - 1)
                    {
                        var info = datas[i + 1, j];
                        if (info.Sort is TileSort.Block)
                            index += 3;
                        if (info.Sort is TileSort.Platform)
                            index += 6;
                    }
                    tileFrameX = index switch
                    {
                        1 => 6,
                        2 => 1,
                        3 => 7,
                        5 => 4,
                        6 => 2,
                        7 => 3,
                        8 => 0,
                        0 or 4 or _ => 5
                    };
                    tileFrameX *= 18;
                    break;
                }
            case TileSort.Torch:
                {
                    bool leftLinked = i > 0 && datas[i - 1, j].Sort is TileSort.Block;
                    bool rightLinked = i < width - 1 && datas[i + 1, j].Sort is TileSort.Block;
                    if (leftLinked)
                        tileFrameX = 1;
                    else if (rightLinked)
                        tileFrameX = 2;
                    else tileFrameX = 0;
                    tileFrameX *= 22;
                    tileFrameY = 0;
                    break;
                }

            // 1x3
            case TileSort.Lamp:
            case TileSort.Door:
                {
                    MultiTileFraming(datas, sort, i, j, 1, 3, out tileFrameX, out tileFrameY);
                    if (sort is TileSort.Door)
                        tileFrameX = Main.rand.Next(0, 3) * 18;
                    break;
                }

            // 2x1
            case TileSort.Workbench:
                {
                    MultiTileFraming(datas, sort, i, j, 2, 1, out tileFrameX, out tileFrameY);
                    break;
                }

            // 3x3
            case TileSort.Chandelier:
                {
                    MultiTileFraming(datas, sort, i, j, 3, 3, out tileFrameX, out tileFrameY);
                    break;
                }

            // 2x5
            case TileSort.Clock:
                {
                    MultiTileFraming(datas, sort, i, j, 2, 5, out tileFrameX, out tileFrameY);
                    break;
                }

            case TileSort.Candle:
            default:
                tileFrameX = tileFrameY = 0;
                break;
        }


        if (current.HasWall)
        {
            bool leftLinked = i > 0 && datas[i - 1, j].HasWall;
            bool rightLinked = i < width - 1 && datas[i + 1, j].HasWall;
            bool upLinked = j > 0 && datas[i, j - 1].HasWall;
            bool downLinked = j < height - 1 && datas[i, j + 1].HasWall;
            int index = 0;
            if (leftLinked) index += 1 << 0;
            if (rightLinked) index += 1 << 1;
            if (upLinked) index += 1 << 2;
            if (downLinked) index += 1 << 3;
            (wallFrameX, wallFrameY) = index switch
            {
                0 => (Main.rand.Next(9, 12), 3), // 不和任意方块连接
                1 => (12, Main.rand.Next(0, 3)), // 仅和左侧相连
                2 => (9, Main.rand.Next(0, 3)), // 仅和右侧相连
                3 => (Main.rand.Next(6, 9), 4), // 和左右侧相连
                4 => (Main.rand.Next(6, 9), 3), // 仅和上侧相连
                5 => (Main.rand.Next([1, 3, 5]), 4), // 和左上侧相连
                6 => (Main.rand.Next([0, 2, 4]), 4), // 和右上侧相连
                7 => (Main.rand.Next(1, 4), 2), // 和左右上侧相连
                8 => (Main.rand.Next(6, 9), 0), // 仅和下侧相连
                9 => (Main.rand.Next([1, 3, 5]), 3), // 和左下侧相连
                10 => (Main.rand.Next([0, 2, 4]), 3), // 和右下侧相连
                11 => (Main.rand.Next(1, 4), 0), // 和左右下侧相连
                12 => (5, Main.rand.Next(0, 3)), // 和上下侧相连
                13 => (4, Main.rand.Next(0, 3)), // 和左上下侧相连
                14 => (0, Main.rand.Next(0, 3)), // 和右上下侧相连
                15 => (Main.rand.Next(1, 4), 1), // 和上下左右相连
                _ => (9, 3)
            };
            wallFrameX *= 36;
            wallFrameY *= 36;
        }
    }
}
