using ImproveGame.Content.Items.Globes.Core;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes;

public class PyramidGlobe : OnceForAllGlobe
{
    public class PyramidGlobeProj() : OnceForAllGlobeProj<PyramidGlobe>(new(197, 174, 79))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Pyramids;
        public override bool NotFoundCheck() => StructureDatas.PyramidPositions.Count is 0;
        public override Point16[] Positions => [.. StructureDatas.PyramidPositions];

        public override void ExtraCheckWhenNotRecorded()
        {
            for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    if (ExtraCheckPyramid(i, j))
                        StructureDatas.PyramidPositions.Add(new(i, j));
                }
            }
        }

        static bool ExtraCheckPyramid(int i, int j)
        {
            #region 金箱检测
            var tile = Framing.GetTileSafely(i, j);
            // 箱子帧图横向是18一格，然后金箱是第2个箱子，(2 - 1) * 2 * 18即得到对应横向帧36
            if (tile.TileType != TileID.Containers || tile.TileFrameX != 18 * 2)
                return false;
            #endregion

            #region 前记录检测
            foreach (var p in StructureDatas.PyramidPositions)
                if (MathF.Abs(p.X - i) < 100 && MathF.Abs(p.Y - j) < 100) return false;

            #endregion

            #region 检测箱底是否砂岩砖
            bool skip = false;
            for (int x = 0; x < 2; x++)
            {
                tile = Framing.GetTileSafely(i + x, j + 2);
                if (tile.TileType != TileID.SandstoneBrick)
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
            for (int k = -20; k <= 20; k++)
            {
                tile = Framing.GetTileSafely(i + k, j);

                if (tile.TileType == TileID.SandstoneBrick)
                    hasSunPlateTileX = true;

                if (tile.WallType == WallID.SandstoneBrick)
                    hasSunPlateWallX = true;

                tile = Framing.GetTileSafely(i, j + k);

                if (tile.TileType == TileID.SandstoneBrick)
                    hasSunPlateTileY = true;

                if (tile.WallType == WallID.SandstoneBrick)
                    hasSunPlateWallY = true;

            }
            return hasSunPlateTileX
                    && hasSunPlateWallX
                    && hasSunPlateTileY
                    && hasSunPlateWallY;
            #endregion
        }
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddRecipeGroup(RecipeGroups.Sand, 50)
            .AddTile(TileID.WorkBenches);
}
