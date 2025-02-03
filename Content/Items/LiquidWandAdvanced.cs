using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModHooks;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI;
using ImproveGame.UIFramework;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public class LiquidWandAdvanced : LiquidWand
    {

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
                .AddCondition(ConfigCondition.NotAvailableLiquidWandC)
                .AddCondition(ConfigCondition.AvailableLiquidWandAdvancedC)
                .Register();

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LiquidWand>())
                .AddIngredient(ItemID.UltraAbsorbantSponge)
                .AddIngredient(ItemID.BottomlessLavaBucket)
                .AddIngredient(ItemID.BottomlessHoneyBucket)
                .AddIngredient(ItemID.BottomlessShimmerBucket)
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(ConfigCondition.AvailableLiquidWandAdvancedC)
                .DisableDecraft() // 防止刷物品，因为月后基础液体法杖可以直接微光转化为终极液体法杖
                .Register();
        }
    }
}