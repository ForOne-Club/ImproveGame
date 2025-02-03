using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModSystems;
using Terraria.ID;

namespace ImproveGame.Content.Items.Placeable
{
    public class ExtremeStorage : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 5);
            Item.createTile = ModContent.TileType<Tiles.ExtremeStorage>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire, 5)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddRecipeGroup(RecipeSystem.AnyGoldBar, 12)
                .AddTile(TileID.Anvils)
                .AddCondition(ConfigCondition.AvailableExtremeStorageC)
                .Register();
        }
    }
}
