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
            // 金锭
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 12)
                .AddIngredient(ItemID.CopperBar, 6)
                .AddIngredient(ItemID.Glass, 20)
                .Register();
            // 铂金锭
            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 12)
                .AddIngredient(ItemID.CopperBar, 6)
                .AddIngredient(ItemID.Glass, 20)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.LeadBar, 12)
                .AddIngredient(ItemID.CopperBar, 6)
                .AddIngredient(ItemID.Glass, 20)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.LeadBar, 12)
                .AddIngredient(ItemID.TinBar, 6)
                .AddIngredient(ItemID.Glass, 20)
                .Register();
        }
    }
}
