using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.Content.Items;

public partial class CreateWand
{
    private static void FramingTiles(
        int i,
        int j,
        TileSort current,
        TileSort[,] datas,
        int width,
        int height,
        out int tileFrameX,
        out int tileFrameY,
        out int wallFrameX,
        out int wallFrameY)
    {
        wallFrameX = wallFrameY = 0;
        switch (current)
        {
            case TileSort.Block:
                {
                    bool leftLinked = i > 0 && datas[i - 1, j] is TileSort.Block;
                    bool rightLinked = i < width - 1 && datas[i + 1, j] is TileSort.Block;
                    bool upLinked = j > 0 && datas[i, j - 1] is TileSort.Block;
                    bool downLinked = j < height - 1 && datas[i, j + 1] is TileSort.Block;
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
                        bool leftUp = i > 0 && j > 0 && datas[i - 1, j - 1] is TileSort.Block;
                        bool leftDown = i > 0 && j < height - 1 && datas[i - 1, j + 1] is TileSort.Block;
                        bool rightUp = i < width - 1 && j > 0 && datas[i + 1, j - 1] is TileSort.Block;
                        bool rightDown = i < width - 1 && j < height - 1 && datas[i + 1, j + 1] is TileSort.Block;

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
            case TileSort.Chair:
                {
                    tileFrameX = 18;
                    tileFrameY = j > 0 && datas[i, j - 1] is TileSort.Chair ? 18 : 0;
                    break;
                }
            case TileSort.Table:
                {
                    int offX = 0;
                    int offY = 0;

                    while (i >= offX && datas[i - offX, j] is TileSort.Table)
                        offX++;
                    offX--;

                    while (j >= offY && datas[i, j - offY] is TileSort.Table)
                        offY++;
                    offY--;
                    offX %= 3;
                    if (offY > 1) offY = 1;
                    tileFrameX = offX * 18;
                    tileFrameY = offY * 18;
                    break;
                }
            case TileSort.Platform:
                {
                    tileFrameY = 0;
                    int index = 0;
                    if (i > 0)
                    {
                        var sort = datas[i - 1, j];
                        if (sort is TileSort.Block)
                            index += 1;
                        if (sort is TileSort.Platform)
                            index += 2;
                    }
                    if (i < width - 1)
                    {
                        var sort = datas[i + 1, j];
                        if (sort is TileSort.Block)
                            index += 3;
                        if (sort is TileSort.Platform)
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
                    bool leftLinked = i > 0 && datas[i - 1, j] is TileSort.Block;
                    bool rightLinked = i < width - 1 && datas[i + 1, j] is TileSort.Block;
                    if (leftLinked)
                        tileFrameX = 1;
                    else if (rightLinked)
                        tileFrameX = 2;
                    else tileFrameX = 0;
                    tileFrameX *= 22;
                    tileFrameY = 0;
                    break;
                }
            case TileSort.Door:
                {
                    int offX = Main.rand.Next(0, 3);
                    int offY = 0;

                    while (j >= offY && datas[i, j - offY] is TileSort.Door)
                        offY++;
                    offY--;
                    if (offY > 2) offY = 2;
                    tileFrameX = offX * 18;
                    tileFrameY = offY * 18;
                    break;
                }
            case TileSort.Bed:
                {
                    int offX = 0;
                    int offY = 0;

                    while (i >= offX && datas[i - offX, j] is TileSort.Bed)
                        offX++;
                    offX--;

                    while (j >= offY && datas[i, j - offY] is TileSort.Bed)
                        offY++;
                    offY--;
                    offX %= 4;
                    offX += 4;
                    if (offY > 1) offY = 1;
                    tileFrameX = offX * 18;
                    tileFrameY = offY * 18;
                    break;
                }
            case TileSort.Workbench:
                {
                    int offX = 0;
                    while (i >= offX && datas[i - offX, j] is TileSort.Workbench)
                        offX++;
                    offX--;
                    offX %= 2;
                    tileFrameX = offX * 18;
                    tileFrameY = 0;
                    break;
                }
            default:
                tileFrameX = tileFrameY = 0;
                break;
        }


        if (ShouldPlaceWall(current))
        {
            bool leftLinked = i > 0 && ShouldPlaceWall(datas[i - 1, j]);
            bool rightLinked = i < width - 1 && ShouldPlaceWall(datas[i + 1, j]);
            bool upLinked = j > 0 && ShouldPlaceWall(datas[i, j - 1]);
            bool downLinked = j < height - 1 && ShouldPlaceWall(datas[i, j + 1]);
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
