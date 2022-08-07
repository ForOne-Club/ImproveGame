using ImproveGame.Common.Systems;

namespace ImproveGame.Content.Items.Placeable
{
    public class Autofisher : ModItem
    {
        public override void SetStaticDefaults() => SacrificeTotal = 1;

        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 6);
            Item.createTile = ModContent.TileType<Tiles.Autofisher>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeSystem.IronGroup, 8)
                .AddRecipeGroup(RecipeSystem.CopperGroup, 4)
                .AddIngredient(ItemID.Glass, 20)
                .Register();
        }
    }
}
