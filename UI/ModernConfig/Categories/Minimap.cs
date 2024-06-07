using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Minimap : Category
{
    public override int ItemIconId => ItemID.TrifoldMap;

    public override string LocalizationKey => "Minimap";

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddToggle(Config, nameof(Config.MinimapMark));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkDungeon));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkTemple));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkAether));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkFloatingIsland));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkPyramid));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkPlantera));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkEnchantedSword));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.MarkEmptyAutofisher));
    }
}