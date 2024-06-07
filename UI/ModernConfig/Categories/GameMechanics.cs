using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class GameMechanics : Category
{
    public override int ItemIconId => ItemID.Cog;

    public override string LocalizationKey => "GameMechanics";

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.AlchemyGrassGrowsFaster));
        panel.AddToggle(Config, nameof(Config.AlchemyGrassAlwaysBlooms));
        panel.AddToggle(Config, nameof(Config.StaffOfRegenerationAutomaticPlanting));
    }
}