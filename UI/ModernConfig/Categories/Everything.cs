namespace ImproveGame.UI.ModernConfig.Categories;

public class Everything : Category
{
    public override Texture2D GetIcon()
    {
        return ModAsset.Infinite.Value;
    }

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var allCards = CategorySidePanel.CategoriesArray;
        foreach (var categoryCard in allCards) {
            categoryCard.AddOptions(panel);
        }
    }
}