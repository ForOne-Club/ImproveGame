using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Everything : Category
{
    public override Texture2D GetIcon() => ModAsset.Infinite.Value;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var mod = ModernConfigUI.Instance.currentMod;
        if (mod.Name == "ImproveGame")
        {
            var allCards = CategorySidePanel.CategoriesArray;
            foreach (var categoryCard in allCards)
                categoryCard.AddOptions(panel);
            return;
        }
        if (!CategorySidePanel.ModdedCards.TryGetValue(mod, out var list))
        {
            if (mod == null || !ConfigManager.Configs.TryGetValue(mod, out var configs))
                return;
            var sortedConfigs = configs.OrderBy(x => Utils.CleanChatTags(x.DisplayName.Value)).ToList();
            foreach (var config in sortedConfigs)
                new SingleConfigCategory(config).AddOptions(panel);
        }
        else
            foreach (Category card in list)
                card.AddOptions(panel);
    }
}