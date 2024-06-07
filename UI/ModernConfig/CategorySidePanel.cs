using ImproveGame.UIFramework.SUIElements;
using System.Reflection;

namespace ImproveGame.UI.ModernConfig;

public sealed class CategorySidePanel : SUIPanel
{
    public CategorySidePanel() : base(Color.Black * 0.4f, Color.Black * 0.4f)
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type type in types)
        {
            if (!type.IsSubclassOf(typeof(Category)) || type.IsSubclassOf(typeof(DoNotAutoload)))
                continue;

            var category = (Category)Activator.CreateInstance(type);
            AddCard(new CategoryCard(category));
        }
    }

    private void AddCard<T>() where T : Category
    {
        var category = Activator.CreateInstance<T>();
        AddCard(new CategoryCard(category));
    }

    private void AddCard(CategoryCard card)
    {
        card.JoinParent(this);
    }
}