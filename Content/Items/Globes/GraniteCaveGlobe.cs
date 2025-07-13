using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;

namespace ImproveGame.Content.Items.Globes;

public class GraniteCaveGlobe() : GlobePlentyTooltip(ItemRarityID.Quest, Item.sellPrice(silver: 30))
{
    public class GraniteCaveGlobeProj() : GlobeProjBase<GraniteCaveGlobe>(new(77, 80, 136))
    {
        public override bool RevealOperation(bool onlyJudging) => GlobeRevealer.RevealGranite(Projectile, GetModItemDummy(), onlyJudging);
    }


    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 8)
            .AddRecipeGroup(RecipeGroupID.IronBar, 4)
            .AddIngredient(ItemID.Ruby)
            .AddTile(TileID.WorkBenches);
}