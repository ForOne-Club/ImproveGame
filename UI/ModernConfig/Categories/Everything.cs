namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Everything : Category
{
    public override Texture2D GetIcon() => ModAsset.Infinite.Value;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var allCards = CategorySidePanel.CategoriesArray;
        foreach (var categoryCard in allCards)
            categoryCard.AddOptions(panel);
    }
}