using ImproveGame.Common.Configs.FavoritedSystem;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Favorites : Category
{
    public override Texture2D GetIcon() => ModAsset.FallenStarStatic.Value;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var allCards = CategorySidePanel.CategoriesArray;
        foreach (var categoryCard in allCards)
            categoryCard.AddOptions(panel);
    }

    public override Func<ModConfig, string, bool> CanOptionBeAdded => (config, optionName) =>
        FavoritedOptionDatabase.FavoritedOptions.Contains($"{config.Name}.{optionName}");
}