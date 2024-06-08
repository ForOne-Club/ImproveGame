using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Multiplayer : Category
{
    public override int ItemIconId => ItemID.GolfCupFlagRed;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.ShareCraftingStation));
        panel.AddToggle(Config, nameof(Config.ShareInfBuffs));
        panel.AddValueSlider(Config, nameof(Config.ShareRange));
        panel.AddToggle(Config, nameof(Config.TeamAutoJoin));
        panel.AddToggle(Config, nameof(Config.BedOnlyOne));
        panel.AddToggle(Config, nameof(Config.NoConditionTP));
    }
}