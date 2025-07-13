using ImproveGame.Common.Conditions;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModSystems;

namespace ImproveGame.Content.Items
{
    public class LiquidWandAdvanced : LiquidWand, IConditionItem
    {
        public Condition UseCondition => ConfigCondition.AvailableLiquidWandAdvancedC;

        public override bool AltFunctionUse(Player player) => true;

        public override void SetItemDefaults()
        {
            base.SetItemDefaults();
            SelectRange = new(50, 50);
            MaxTilesPerFrame = 9999;
            IsAdvancedWand = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddRecipeGroup(RecipeSystem.AnyShadowScale, 8)
                .AddRecipeGroup(RecipeSystem.AnyGoldBar, 6)
                .AddIngredient(ItemID.UltraAbsorbantSponge)
                .AddIngredient(ItemID.BottomlessLavaBucket)
                .AddIngredient(ItemID.BottomlessHoneyBucket)
                .AddIngredient(ItemID.BottomlessShimmerBucket)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LiquidWand>())
                .AddIngredient(ItemID.UltraAbsorbantSponge)
                .AddIngredient(ItemID.BottomlessLavaBucket)
                .AddIngredient(ItemID.BottomlessHoneyBucket)
                .AddIngredient(ItemID.BottomlessShimmerBucket)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft() // 防止刷物品，因为月后基础液体法杖可以直接微光转化为终极液体法杖
                .Register();
        }
    }
}