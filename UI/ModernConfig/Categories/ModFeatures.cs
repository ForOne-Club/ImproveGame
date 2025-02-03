using ImproveGame.Common.Configs;
using ImproveGame.UI.ModernConfig.OptionElements;

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
        panel.AddToggle(Config, nameof(Config.WorldFeaturePanel));
        panel.AddToggle(Config, nameof(Config.WeatherControl));
        panel.AddToggle(Config, nameof(Config.QuickShimmer));
        panel.AddToggle(Config, nameof(Config.MinimapMark));
        panel.AddToggle(Config, nameof(Config.SuperVault));
        panel.AddToggle(Config, nameof(Config.ImprovePrefix));
        panel.AddToggle(Config, nameof(Config.AmmoChain));
        panel.AddValueText(Config, nameof(Config.SpawnRateMaxValue));
        panel.AddToggle(Config, nameof(Config.EmptyAutofisher));
        panel.AddValueSlider(Config, nameof(Config.ExStorageSearchDistance));
        panel.AddToggle(Config, nameof(Config.ICanSeeForeverAllBag));
        panel.AddToggle(Config, nameof(Config.WandMaterialNoConsume));
    }
}