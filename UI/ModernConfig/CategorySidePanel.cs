using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UI.ModernConfig.FakeCategories;
using ImproveGame.UIFramework.SUIElements;
using System.Collections;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using tModPorter;

namespace ImproveGame.UI.ModernConfig;

public sealed class CategorySidePanel : SUIPanel
{
    internal static readonly Category[] CategoriesArray =
    [
        new PlayerAbility(),
        new ItemSettings(),
        new PlantSettings(),
        new NpcSettings(),
        new EnemySettings(),
        new GameMechanics(),
        new PylonMechanics(),
        new Multiplayer(),
        new ModFeatures(),
        new ModItemSettings(),
        new VisualAndInterface(),
        new Minimap(),
    ];
    //static Dictionary<Mod, Action<Mod, Dictionary<string, CategoryCard>>> cardAddingProcesses = [];
    internal static readonly Dictionary<string, CategoryCard> Cards = [];

    public static Dictionary<Mod, List<Category>> ModdedCards = [];

    public static Dictionary<Mod, Category> ModdedAboutPage = [];

    public static Dictionary<Mod, LocalizedText> ModdedTitle = [];


    public static Dictionary<PropertyFieldWrapper, PreviewDrawing> ModdedPreviews = [];

    public static void RemoveCategory(Mod mod)
    {
        if (ModdedCards.TryGetValue(mod, out var dict))
            dict.Clear();
    }

    public static void RegisterCategory(Mod mod, Category category)
    {
        if (!ModdedCards.TryGetValue(mod, out var list))
        {
            list = [];
            ModdedCards.Add(mod, list);
        }
        list.Add(category);
    }

    public static void RegisterCategory(Mod mod, List<KeyValuePair<PropertyFieldWrapper, ModConfig>> variables, int itemIconID = 0, Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
    {
        CrossModCategoryCard categoryCard = new CrossModCategoryCard(variables, itemIconID, getIconTexture, getLabel, getTooltip);

        RegisterCategory(mod, categoryCard);
    }

    public static void RegisterCategory(Mod mod, List<KeyValuePair<string, ModConfig>> variables, int itemIconID = 0, Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
    {
        List<KeyValuePair<PropertyFieldWrapper, ModConfig>> variables_member = [];
        foreach (var pair in variables)
        {
            var configType = pair.Value.GetType();
            var fieldInfo = configType.GetField(pair.Key);
            var propertyInfo = configType.GetProperty(pair.Key);

            if (fieldInfo != null)
                variables_member.Add(new(new(fieldInfo), pair.Value));
            else if (propertyInfo != null)
                variables_member.Add(new(new(propertyInfo), pair.Value));
            else
                ImproveGame.Instance.Logger.Error($"Property or Field Named \"{pair.Key}\" Not Found in Config \"{pair.Value}\".");
        }
        CrossModCategoryCard categoryCard = new CrossModCategoryCard(variables_member, itemIconID, getIconTexture, getLabel, getTooltip);

        RegisterCategory(mod, categoryCard);
    }
    public static void SetAboutPage(Mod mod, Category category) => ModdedAboutPage[mod] = category;

    public static void SetAboutPage(Mod mod, Func<string> getAboutText, int itemIconID = 0, Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
        => ModdedAboutPage[mod] = new AboutPage_CrossMod(getAboutText, itemIconID, getIconTexture, getLabel, getTooltip);


    public static void RemoveAboutPage(Mod mod) => ModdedAboutPage.Remove(mod);

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
        EnableBlur = false;
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
            if (!ModdedAboutPage.TryGetValue(mod, out var page))
                new CategoryCard(AboutPage_ModConfig).JoinParent(Categories.ListView);
            else
                new CategoryCard(page).JoinParent(Categories.ListView);

            new CategoryCard(new Favorites()).JoinParent(Categories.ListView);
            new CategoryCard(new Everything()).JoinParent(Categories.ListView);

            if (!ModdedCards.TryGetValue(mod, out var list))
                DefaultCardAddingProcess(mod);
            else
                foreach (Category card in list)
                    new CategoryCard(card).JoinParent(Categories.ListView);

            // 控件页，检测到此Mod确实有控件才添加
            if (KeybindLoader.Keybinds.Any(x => x.Mod.Name == mod.Name))
                new CategoryCard(new Keybinds(mod.Name)).JoinParent(Categories.ListView);
        }
    }
    public void DefaultCardAddingProcess(Mod mod)
    {
        if (mod == null || !ConfigManager.Configs.TryGetValue(mod, out var configs))
            return;
        var sortedConfigs = configs.OrderBy(x => Utils.CleanChatTags(x.DisplayName.Value)).ToList();
        foreach (var config in sortedConfigs)
            new CategoryCard(new SingleConfigCategory(config)).JoinParent(Categories.ListView);
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

    //public static void RegisterCardAddingProcess(Mod mod, Action<Mod, Dictionary<string, CategoryCard>> process)
    //{
    //    cardAddingProcesses[mod] = process;
    //}
}