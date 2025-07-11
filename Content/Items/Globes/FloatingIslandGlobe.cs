using ImproveGame.Content.Items.Globes.Core;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes;

public class FloatingIslandGlobe : OnceForAllGlobe
{
    public class FloatingIslandGlobeProj() : OnceForAllGlobeProj<FloatingIslandGlobe>(new(34, 170, 82))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.FloatingIslands;
        public override bool NotFoundCheck() => StructureDatas.SkyHousePositions.Count is 0 && StructureDatas.SkyLakePositions.Count is 0;
        public override Point16[] Positions => [.. StructureDatas.SkyHousePositions];
        public override Point16[] PositionsAnother => [.. StructureDatas.SkyLakePositions];
        public override void ExtraCheckWhenNotRecorded()
        {
            for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    if (ExtraCheckSkyHouse(i, j))
                        StructureDatas.SkyHousePositions.Add(new(i, j));
                    else if (ExtraCheckSkyLake(i, j) || ExtraCheckSkyLake2(i,j))
                        StructureDatas.SkyLakePositions.Add(new(i, j));
                    else
                        continue;
                }
            }
        }

        public static bool ExtraCheckSkyHouse(int i, int j)
        {
            #region 天域箱检测
            var tile = Framing.GetTileSafely(i, j);
            // 箱子帧图横向是18一格，然后天域箱是第14个箱子，(14 - 1) * 2 * 18即得到对应横向帧468
            if (tile.TileType != TileID.Containers || tile.TileFrameX != 18 * 26)
                return false;
            #endregion

            #region 前记录检测
            foreach (var p in StructureDatas.SkyHousePositions)
                if (MathF.Abs(p.X - i) < 100 && MathF.Abs(p.Y - j) < 100)
                    return false;
            #endregion

            #region 检测箱底是否日盘块
            bool skip = false;
            for (int x = 0; x < 2; x++)
            {
                tile = Framing.GetTileSafely(i + x, j + 2);
                if (tile.TileType != TileID.Sunplate)
                {
                    skip = true;
                    break;
                }
            }
            if (skip)
                return false;

            #endregion


            #region 十字范围检测
            bool hasSunPlateTileX = false;
            bool hasSunPlateWallX = false;
            bool hasSunPlateTileY = false;
            bool hasSunPlateWallY = false;
            bool hasDirtTileY = false;
            bool hasCloudTileY = false;
            for (int k = -20; k <= 20; k++)
            {
                tile = Framing.GetTileSafely(i + k, j);

                if (tile.TileType == TileID.Sunplate)
                    hasSunPlateTileX = true;

                if (tile.WallType == WallID.DiscWall)
                    hasSunPlateWallX = true;

                tile = Framing.GetTileSafely(i, j + k);

                if (tile.TileType == TileID.Sunplate)
                    hasSunPlateTileY = true;

                if (tile.WallType == WallID.DiscWall)
                    hasSunPlateWallY = true;

                if (tile.TileType == TileID.Dirt)
                    hasDirtTileY = true;

                if (tile.TileType == TileID.Cloud)
                    hasCloudTileY = true;

                // 按说可以做个提前退出循环的检测的
                // 但是我感觉这里条件这么多要是作那样的检测每次都得算一堆bool
                // 性能反而差的
            }


            return hasSunPlateTileX
                && hasSunPlateWallX
                && hasSunPlateTileY
                && hasSunPlateWallY
                && hasDirtTileY
                && hasCloudTileY;
            #endregion
        }

        static bool ExtraCheckSkyLake(int i, int j)
        {
            #region 水体检测
            var tile = Framing.GetTileSafely(i, j);
            if (tile.LiquidAmount == 0 || tile.LiquidType != LiquidID.Water)
                return false;
            #endregion

            #region 前记录检测
            foreach (var p in StructureDatas.SkyLakePositions)
                if (MathF.Abs(p.X - i) < 100 && MathF.Abs(p.Y - j) <100) return false;

            #endregion

            #region 检测7x7水域
            bool skip = false;
            for (int x = i - 3; x <= i + 3; x++)
            {
                for (int y = j - 3; y <= j + 3; y++)
                {
                    tile = Framing.GetTileSafely(x, y);
                    // 一旦检测区域里有不符合条件的就退出循环并打上标记
                    if (tile.LiquidAmount == 0 || tile.LiquidType != LiquidID.Water)
                    {
                        skip = true;
                        break;
                    }
                }
                // 通过标记退出外层循环
                if (skip)
                    break;
            }
            // 通过标记移动至下一个检测点
            if (skip)
                return false;
            #endregion

            #region 检测十字区域内是否含有云块
            for (int k = -20; k <= 20; k++)
            {
                tile = Framing.GetTileSafely(i + k, j);
                if (tile.TileType == TileID.Cloud)
                    return true;
                tile = Framing.GetTileSafely(i, j + k);
                if (tile.TileType == TileID.Cloud)
                    return true;
            }
            #endregion
            return false;
        }

        static bool ExtraCheckSkyLake2(int i, int j) 
        {


            #region 云检测
            var tile = Framing.GetTileSafely(i, j);
            if (tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud) return false;
            tile = Framing.GetTileSafely(i, j - 1);
            if (tile.LiquidAmount == 0 || tile.LiquidType != LiquidID.Water) return false;
            #endregion

            #region 前记录检测
            foreach (var p in StructureDatas.SkyLakePositions)
                if (MathF.Abs(p.X - i) < 100 && MathF.Abs(p.Y - j) < 100) return false;

            #endregion

            int count = 0;
            for (int k = 1; k < 15; k++) 
            {
                tile = Framing.GetTileSafely(i, j - k);
                if (tile.LiquidAmount > 0 && tile.LiquidType is LiquidID.Water)
                    count++;
                if (count > 6)
                    return true;
            }

            return false;

        }
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 18)
            .AddRecipeGroup(RecipeGroupID.Wood, 100)
            .AddIngredient(ItemID.Rope, 100)
            .AddTile(TileID.WorkBenches);
}