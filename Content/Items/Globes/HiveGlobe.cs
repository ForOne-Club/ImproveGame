using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;

namespace ImproveGame.Content.Items.Globes;

public class HiveGlobe() : GlobePlentyTooltip(ItemRarityID.Quest, Item.sellPrice(silver: 30))
{
    public class HiveGlobeProj() : GlobeProjBase<HiveGlobe>(new(254, 246, 37))
    {
        public override bool RevealOperation(bool onlyJudging) => GlobeRevealer.RevealHive(Projectile, GetModItemDummy(), onlyJudging);
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.MudBlock, 20)
            .AddIngredient(ItemID.RichMahogany, 20)
            .AddTile(TileID.WorkBenches);
}