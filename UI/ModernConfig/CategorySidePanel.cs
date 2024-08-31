using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UI.ModernConfig.FakeCategories;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig;

public sealed class CategorySidePanel : SUIPanel
{
    internal static readonly Category[] CategoriesArray =
    {
        new PlayerAbility(),
        new ItemSettings(),
        new PlantSettings(),
        new NpcSettings(),
        new EnemySettings(),
        new GameMechanics(),
        new Multiplayer(),
        new ModFeatures(),
        new VisualAndInterface(),
        new Minimap(),
    };

    internal static readonly Dictionary<string, CategoryCard> Cards = new();

    private SUIScrollView2 Categories { get;  set; }

    static CategorySidePanel()
    {
        AddCard<AboutPage>();
        AddCard<Presets>();
        AddCard<Favorites>();
        AddCard<Everything>();
        foreach (var card in CategoriesArray)
            AddCard(new CategoryCard(card));
        AddCard<LicensePage>();
    }

    public CategorySidePanel(Color color) : base(color, color)
    {
        // 自动添加没法直观地调节顺序，所以手动添加
        // Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        // foreach (Type type in types)
        // {
        //     if (!type.IsSubclassOf(typeof(Category)) || type.IsSubclassOf(typeof(DoNotAutoload)))
        //         continue;
        //
        //     var category = (Category)Activator.CreateInstance(type);
        //     AddCard(new CategoryCard(category));
        // }

        Categories = new SUIScrollView2(Orientation.Vertical);
        Categories.SetPadding(0f, 0f);
        Categories.SetSize(0f, 0f, 1f, 1f);
        Categories.JoinParent(this);

        foreach ((string _, CategoryCard card) in Cards)
        {
            card.JoinParent(Categories.ListView);
        }
    }

    private static void AddCard<T>() where T : Category
    {
        var category = Activator.CreateInstance<T>();
        AddCard(new CategoryCard(category));
    }

    private static void AddCard(CategoryCard card)
    {
        Cards.Add(card.Category.LocalizationKey, card);
    }
}