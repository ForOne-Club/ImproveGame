using ImproveGame.Common.Conditions;
using ImproveGame.Common.ModSystems;

namespace ImproveGame.Content.Items.Placeable;

public class BaitSupplier : ModItem
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
        Item.value = Item.sellPrice(gold: 2);
        Item.createTile = ModContent.TileType<Tiles.BaitSupplier>();
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Wire, 20)
            .AddIngredient(ItemID.StoneBlock, 50)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 10)
            .AddTile(TileID.Anvils)
            .AddCondition(ConfigCondition.AvailableBaitSupplierC)
            .Register();
    }
}