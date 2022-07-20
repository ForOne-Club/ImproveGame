using ImproveGame.Common.Systems;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace ImproveGame.Content.Items.Placeable
{
    public class Autofisher : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

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
                .AddRecipeGroup(ModRecipeGroup.IronGroup, 12)
                .AddRecipeGroup(ModRecipeGroup.CopperGroup, 6)
                .AddIngredient(ItemID.Glass, 20)
                .Register();
        }
    }
}
