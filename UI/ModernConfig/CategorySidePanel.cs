using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UI.ModernConfig.FakeCategories;
using ImproveGame.UIFramework.SUIElements;
using Terraria.ModLoader.Config;

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
        new PylonMechanics(),
        new Multiplayer(),
        new ModFeatures(),
        new VisualAndInterface(),
        new Minimap(),
    };
    static Dictionary<Mod, Action<Mod, Dictionary<string, CategoryCard>>> cardAddingProcesses = [];
    internal static readonly Dictionary<string, CategoryCard> Cards = [];

    static Dictionary<Mod, Dictionary<string, CategoryCard>> ModedCards = [];

    private SUIScrollView2 Categories { get; set; }

    internal static Category AboutPage_ModConfig = new AboutPage_ModConfig();

    static CategorySidePanel()
    {
        AddCard<AboutPage>();
        AddCard<Presets>();
        AddCard<Favorites>();
        AddCard<Everything>();
        foreach (var card in CategoriesArray)
            AddCard(new CategoryCard(card));
        AddCard<Keybinds>();
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


    }
    public void ChangeMod(Mod mod, bool reprocess = false)
    {
        //Categories.RemoveAllChildren();
        Categories.ListView.RemoveAllChildren();
        if (mod.Name == "ImproveGame")
            foreach ((string _, CategoryCard card) in Cards)
                card.JoinParent(Categories.ListView);
        else
        {
            if (!ModedCards.TryGetValue(mod, out var dict)) 
            {
                dict = [];
                ModedCards.Add(mod, dict);
                reprocess = true;
            }
            if (reprocess) 
            {
                dict.Clear();
                if (cardAddingProcesses.TryGetValue(mod, out var process))//我们莫得有自己的分类方式！！
                    process.Invoke(mod, dict);
                else
                    DefaultCardAddingProcess(mod, dict);
            }
            foreach ((string _, CategoryCard card) in dict)
                card.JoinParent(Categories.ListView);
        }
    }
    public static void DefaultCardAddingProcess(Mod mod, Dictionary<string, CategoryCard> dict) 
    {
        if (mod == null || !ConfigManager.Configs.TryGetValue(mod, out var configs))
            return;
		var sortedConfigs = configs.OrderBy(x => Utils.CleanChatTags(x.DisplayName.Value)).ToList();
        dict.Add("AboutPage_ModConfig",new CategoryCard(AboutPage_ModConfig));
        foreach (var config in sortedConfigs) 
        {
            dict.Add(config.Name, new CategoryCard(new SingleConfigCategory(config)));
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

    public static void RegisterCardAddingProcess(Mod mod, Action<Mod, Dictionary<string, CategoryCard>> process)
    {
        cardAddingProcesses[mod] = process;
    }
}