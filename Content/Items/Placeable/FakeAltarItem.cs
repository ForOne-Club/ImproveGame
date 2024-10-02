using ImproveGame.Content.Tiles;

namespace ImproveGame.Content.Items.Placeable
{
    public abstract class FakeAltarItem : ModItem
    {
        public override string Texture => base.Texture[..^4];
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.autoReuse = true;
            Item.useAnimation = 14;
            Item.createTile = ModContent.TileType<FakeAltar>();
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.consumable = true;
            Item.rare = ItemRarityID.Purple;
        }
    }
    public class DemonAltarItem : FakeAltarItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.placeStyle = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 5)
                .Register();
        }
    }
    public class CrimsonAltarItem : FakeAltarItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.placeStyle = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 5)
                .Register();
        }
    }
}
