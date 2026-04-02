using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class ModFeatures : Category
{
    public override Texture2D GetIcon()
    {
        return ModAsset.SpaceWand.Value;
    }

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddToggle(uiConfig, nameof(uiConfig.QoLAutoTrash));
        panel.AddToggle(uiConfig, nameof(uiConfig.RecipeSearch));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.WorldFeaturePanel));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.WeatherControl));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.QuickShimmer));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.MinimapMark));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SuperVault));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ImprovePrefix));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.AmmoChain));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SpawnRateMaxValue));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SpawnRateMinValue));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.EmptyAutofisher));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ExStorageSearchDistance));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ICanSeeForeverAllBag));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.WandMaterialNoConsume));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.DummyCustomAIStyleAllowed));
    }
}