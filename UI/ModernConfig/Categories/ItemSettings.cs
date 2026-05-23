using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class ItemSettings : Category
{
    public override int ItemIconId => ItemID.Wood;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        //panel.AddValueText(Config, nameof(Config.ItemMaxStack));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ItemMaxStack));
        panel.AddToggle(uiConfig, nameof(uiConfig.ShowModName));
        panel.AddToggle(uiConfig, nameof(uiConfig.ShowShimmerInfo));
        panel.AddToggle(uiConfig, nameof(uiConfig.ShowAmmoInfo));
        panel.AddToggle(uiConfig, nameof(uiConfig.AutoSummon));
        panel.AddToggle(uiConfig, nameof(uiConfig.MagicMirrorInstantTp));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.QuestFishStack));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConsume_SummonItem));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConsume_Ammo));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConsume_Projectile));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConsume_Wire));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ImprovePrefix));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.MiddleEnableBank));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.PortableCraftingStation));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoPlace_BUFFTile));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoPlace_BUFFTile_Banner));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConsume_Potion));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConsume_PotionRequirement));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.InfiniteRedPotion));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.RedPotionEverywhere));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.RedPotionRequirement));
        panel.AddToggle(uiConfig, nameof(uiConfig.ShowMoreData));
    }
}