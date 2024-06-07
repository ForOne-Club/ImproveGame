using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig;

public sealed class CategorySidePanel : SUIPanel
{
    public CategorySidePanel() : base(Color.Black * 0.4f, Color.Black * 0.4f, 12, 2, false) {
        AddCard<GameMechanics>();
        AddCard<NpcSettings>();
    }

    private void AddCard<T>() where T : Category
    {
        var category = Activator.CreateInstance<T>();
        var card = new CategoryCard(category);
        card.JoinParent(this);
    }
}