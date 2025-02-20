using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class PlantSettings : Category
{
    public override int ItemIconId => ItemID.Blinkroot;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.AlchemyGrassGrowsFaster));
        panel.AddToggle(Config, nameof(Config.AlchemyGrassAlwaysBlooms));
        panel.AddToggle(Config, nameof(Config.PumpkinGrowsFaster));
        panel.AddToggle(Config, nameof(Config.StaffOfRegenerationAutomaticPlanting));
        panel.AddToggle(Config, nameof(Config.TreeGrowFaster));
        panel.AddToggle(Config, nameof(Config.ShakeTreeFruit));
        panel.AddToggle(Config, nameof(Config.GemTreeAlwaysDropGem));
        panel.AddValueSlider(Config, nameof(Config.MostTreeMin));
        panel.AddValueSlider(Config, nameof(Config.MostTreeMax));
        panel.AddValueSlider(Config, nameof(Config.PalmTreeMin));
        panel.AddValueSlider(Config, nameof(Config.PalmTreeMax));
        panel.AddValueSlider(Config, nameof(Config.GemTreeMin));
        panel.AddValueSlider(Config, nameof(Config.GemTreeMax));
    }
}