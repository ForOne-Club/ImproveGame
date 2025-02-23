using ImproveGame.Common.Conditions;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;
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
            .AddTile(TileID.WorkBenches)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}

public class TempleGlobe : OnceForAllGlobe
{
    public class TempleGlobeProj() : OnceForAllGlobeProj<TempleGlobe>(new(210, 105, 24))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Temple;
        public override bool NotFoundCheck() => StructureDatas.TemplePosition == default;
        public override Point16[] Positions => [StructureDatas.TemplePosition];
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.MudBlock, 100)
            .AddIngredient(ItemID.JungleSpores, 3)
            .AddIngredient(ItemID.Stinger, 3)
            .AddTile(TileID.WorkBenches)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}

public class PyramidGlobe : OnceForAllGlobe
{
    public class PyramidGlobeProj() : OnceForAllGlobeProj<PyramidGlobe>(new(197, 174, 79))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.Pyramids;
        public override bool NotFoundCheck() => StructureDatas.PyramidPositions.Count is 0;
        public override Point16[] Positions => [.. StructureDatas.PyramidPositions];
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddRecipeGroup(RecipeGroupID.Sand, 50)
            .AddTile(TileID.WorkBenches)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}

public class FloatingIslandGlobe : OnceForAllGlobe
{
    public class FloatingIslandGlobeProj() : OnceForAllGlobeProj<FloatingIslandGlobe>(new(34, 170, 82))
    {
        public override StructureDatas.UnlockID StructureType => StructureDatas.UnlockID.FloatingIslands;
        public override bool NotFoundCheck() => StructureDatas.SkyHousePositions.Count is 0 && StructureDatas.SkyLakePositions.Count is 0;
        public override Point16[] Positions => [..StructureDatas.SkyHousePositions];
        public override Point16[] PositionsAnother => [..StructureDatas.SkyLakePositions];
    }
    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 18)
            .AddRecipeGroup(RecipeGroupID.Wood, 100)
            .AddIngredient(ItemID.Rope, 100)
            .AddTile(TileID.WorkBenches)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}