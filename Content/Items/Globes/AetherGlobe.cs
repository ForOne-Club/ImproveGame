using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;
using ImproveGame.Packets.WorldFeatures;

namespace ImproveGame.Content.Items.Globes;

public class AetherGlobe () : Globe(ItemRarityID.Quest, Item.sellPrice(silver: 10))
{
    public class AetherGlobeProj : GlobeProjBase
    {
        public override ModItem GetModItemDummy() => ModContent.GetInstance<AetherGlobe>();
    }

    public override bool RevealOperation(Projectile projectile, bool onlyJudging)
    {
        return RevealAetherPacket.Reveal(projectile, onlyJudging);
    }

    public override Color GetEffectColor() => Main.DiscoColor;

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.StoneBlock, 80)
            .AddRecipeGroup(RecipeSystem.AnyGem, 6)
            .AddTile(TileID.WorkBenches)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}