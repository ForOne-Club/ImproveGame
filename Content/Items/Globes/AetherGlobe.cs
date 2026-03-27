using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Items.Globes.Core;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes;

public class AetherGlobe() : OnceForAllGlobe()
{
    public class AetherGlobeProj() : OnceForAllGlobeProj<AetherGlobe>(default)
    {
        public override Color? GetEffectColor() => Main.DiscoColor;
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Shimmer;
        public override bool NotFoundCheck() => StructureDatas.ShimmerPosition == default;
        public override void ExtraCheckWhenNotRecorded()
        {
            Point16 position = Point16.Zero;
            for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    var tile = Framing.GetTileSafely(i, j);
                    if (tile.LiquidType != LiquidID.Shimmer)
                        continue;

                    bool skip = false;
                    for (int x = i - 2; x <= i + 2; x++)
                    {
                        for (int y = j - 2; y <= j + 2; y++)
                        {
                            tile = Framing.GetTileSafely(x, y);
                            // 一旦检测区域里有不符合条件的就退出循环并打上标记
                            if (tile.LiquidType != LiquidID.Shimmer)
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
                        continue;

                    position = new Vector2(i, j).ToPoint16();
                    StructureDatas.ShimmerPosition = position;
                    break;
                }
                if (position != Point16.Zero)
                    break;
            }
        }
        public override Point16[] Positions => [StructureDatas.ShimmerPosition];
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.StoneBlock, 80)
            .AddRecipeGroup(RecipeSystem.AnyGem, 6)
            .AddTile(TileID.WorkBenches);
}