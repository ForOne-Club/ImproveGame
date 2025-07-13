using ImproveGame.Content.Items.Globes.Core;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes;

public class TempleGlobe : OnceForAllGlobe
{
    public class TempleGlobeProj() : OnceForAllGlobeProj<TempleGlobe>(new(210, 105, 24))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Temple;
        public override bool NotFoundCheck() => StructureDatas.TemplePosition == default;
        public override Point16[] Positions => [StructureDatas.TemplePosition];
        public override void ExtraCheckWhenNotRecorded()
        {
            for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    var tile = Framing.GetTileSafely(i, j);

                    if (tile.TileType is TileID.LihzahrdAltar) 
                    {
                        StructureDatas.TemplePosition = new(i, j);
                        return;
                    }
                }
            }
        }
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.MudBlock, 100)
            .AddIngredient(ItemID.JungleSpores, 3)
            .AddIngredient(ItemID.Stinger, 3)
            .AddTile(TileID.WorkBenches);
}
