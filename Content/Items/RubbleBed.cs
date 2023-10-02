using ImproveGame.Content.Tiles;

namespace ImproveGame.Content.Items;

public class RubbleBed : ModItem
{
    // 没啥用，先隐藏起来
    public override bool IsLoadingEnabled(Mod mod) => false;

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<RubbleBedTile>());
        Item.width = 28;
        Item.height = 20;
        Item.value = 2000;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .Register();
    }
}