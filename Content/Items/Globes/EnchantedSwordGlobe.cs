using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;
using ImproveGame.Packets.WorldFeatures;

namespace ImproveGame.Content.Items.Globes;

public class EnchantedSwordGlobe () : GlobePlentyTooltip(ItemRarityID.Green, Item.sellPrice(silver: 50))
{
    public class EnchantedSwordGlobeProj : GlobeProjBase
    {
        public override ModItem GetModItemDummy() => ModContent.GetInstance<EnchantedSwordGlobe>();
    }

    public override Color GetEffectColor() => new (57, 87, 244);

    public override bool RevealOperation(Projectile projectile, bool onlyJudging)
    {
        return RevealEnchantedSwordPacket.Reveal(projectile, onlyJudging);
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddRecipeGroup(RecipeSystem.AnyGem, 5)
            .AddIngredient(ItemID.StoneBlock, 150)
            .AddIngredient(ItemID.FallenStar, 3)
            .AddTile(TileID.Anvils)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}