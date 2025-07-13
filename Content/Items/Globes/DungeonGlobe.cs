using ImproveGame.Content.Items.Globes.Core;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes;

public class DungeonGlobe : OnceForAllGlobe
{
    public class DungeonGlobeProj() : OnceForAllGlobeProj<DungeonGlobe>(new(76, 95, 109))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Dungeon;
        public override bool NotFoundCheck() => StructureDatas.DungeonPosition == default;
        public override Point16[] Positions => [StructureDatas.DungeonPosition];
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddRecipeGroup(RecipeGroupID.Sand, 10)
            .AddIngredient(ItemID.DirtBlock, 30)
            .AddIngredient(ItemID.StoneBlock, 50)
            .AddTile(TileID.WorkBenches);
}
