using ImproveGame.Common.ModSystems;
using ImproveGame.Packets.WorldFeatures;

namespace ImproveGame.Content.Items.Globes;

public class EnchantedSwordGlobe : ModItem
{
    private LocalizedText GetLocalizedText(string suffix) =>
        Language.GetText($"Mods.ImproveGame.Items.GlobeBase.{suffix}")
            .WithFormatArgs(Language.GetTextValue("ItemName.EnchantedSword"));

    public override LocalizedText DisplayName => GetLocalizedText(nameof(DisplayName));

    public override LocalizedText Tooltip => GetLocalizedText(nameof(Tooltip));

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 50);
    }

    public override void AddRecipes() =>
        CreateRecipe()
            .AddRecipeGroup(RecipeSystem.AnyGem, 5)
            .AddIngredient(ItemID.StoneBlock, 150)
            .AddIngredient(ItemID.FallenStar, 3)
            .AddTile(TileID.Anvils)
            .Register();
    
    public override bool CanUseItem(Player player)
    {
        return RevealEnchantedSwordPacket.Reveal(player);
    }

    public override bool? UseItem(Player player)
    {
        return true;
    }
}