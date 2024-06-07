using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class NpcSettings : Category
{
    public override int ItemIconId => ItemID.CombatBook;

    public override string LocalizationKey => "NpcSettings";

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.TownNPCHome));
        panel.AddToggle(Config, nameof(Config.TownNPCGetTFIntoHouse));
        panel.AddToggle(Config, nameof(Config.NPCLiveInEvil));
    }
}