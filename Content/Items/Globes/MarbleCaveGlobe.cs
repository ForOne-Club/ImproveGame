using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;

namespace ImproveGame.Content.Items.Globes;

public class MarbleCaveGlobe() : GlobePlentyTooltip(ItemRarityID.Quest, Item.sellPrice(silver: 30))
{
    public class MarbleCaveGlobeProj() : GlobeProjBase<MarbleCaveGlobe>(new(132, 137, 164))
    {
        public override bool RevealOperation(bool onlyJudging) => GlobeRevealer.RevealMarble(Projectile, GetModItemDummy(), onlyJudging);
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 8)
            .AddRecipeGroup(RecipeGroups.IronBar, 4)
            .AddIngredient(ItemID.Ruby)
            .AddTile(TileID.WorkBenches);
}