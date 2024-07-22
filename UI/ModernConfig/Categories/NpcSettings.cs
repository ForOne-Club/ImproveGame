using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class NpcSettings : Category
{
    public override int ItemIconId => ItemID.CombatBook;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.TownNPCHome));
        panel.AddToggle(Config, nameof(Config.TownNPCGetTFIntoHouse));
        panel.AddToggle(Config, nameof(Config.NPCLiveInEvil));
        panel.AddValueSlider(Config, nameof(Config.TownNPCSpawnSpeed));
        panel.AddEnum(Config, nameof(Config.NoCD_FishermanQuest));
        panel.AddToggle(Config, nameof(Config.ModifyNPCHappiness));
        panel.AddValueSlider(Config, nameof(Config.NPCHappiness));
        panel.AddToggle(Config, nameof(Config.TravellingMerchantStay));
        panel.AddToggle(Config, nameof(Config.TravellingMerchantRefresh));
        panel.AddToggle(Config, nameof(Config.QuickNurse));
    }
}