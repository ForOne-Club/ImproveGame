using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;
using ImproveGame.Packets.WorldFeatures;
using Terraria;

namespace ImproveGame.Content.Items.Globes;

public class EnchantedSwordGlobe() : GlobePlentyTooltip(ItemRarityID.Green, Item.sellPrice(silver: 50))
{
    public class EnchantedSwordGlobeProj() : GlobeProjBase<EnchantedSwordGlobe>(new(57, 87, 244))
    {
        public override bool RevealOperation(bool onlyJudging) => GlobeRevealer.RevealEnchantedSword(Projectile, GetModItemDummy(), onlyJudging);
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddRecipeGroup(RecipeSystem.AnyGem, 5)
            .AddIngredient(ItemID.StoneBlock, 150)
            .AddIngredient(ItemID.FallenStar, 3)
            .AddTile(TileID.Anvils)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}