using ImproveGame.Common.Configs;
using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class ItemSettings : Category
{
    public override int ItemIconId => ItemID.Wood;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddValueText(Config, nameof(Config.ItemMaxStack));
        panel.AddToggle(uiConfig, nameof(uiConfig.ShowShimmerInfo));
        panel.AddToggle(uiConfig, nameof(uiConfig.AutoSummon));
        panel.AddToggle(Config, nameof(Config.QuestFishStack));
        panel.AddToggle(Config, nameof(Config.NoConsume_SummonItem));
        panel.AddToggle(Config, nameof(Config.NoConsume_Ammo));
        panel.AddToggle(Config, nameof(Config.NoConsume_Projectile));
        panel.AddToggle(Config, nameof(Config.ImprovePrefix));
        panel.AddToggle(Config, nameof(Config.MiddleEnableBank));
        panel.AddToggle(Config, nameof(Config.PortableCraftingStation));
        panel.AddToggle(Config, nameof(Config.NoPlace_BUFFTile));
        panel.AddToggle(Config, nameof(Config.NoPlace_BUFFTile_Banner));
        panel.AddToggle(Config, nameof(Config.NoConsume_Potion));
        panel.AddValueSlider(Config, nameof(Config.NoConsume_PotionRequirement));
        panel.AddToggle(Config, nameof(Config.InfiniteRedPotion));
        panel.AddToggle(Config, nameof(Config.RedPotionEverywhere));
        panel.AddValueSlider(Config, nameof(Config.RedPotionRequirement));
        panel.AddToggle(uiConfig, nameof(uiConfig.ShowMoreData));
    }
}