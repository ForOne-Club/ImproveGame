using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class NpcSettings : Category
{
    public override int ItemIconId => ItemID.CombatBook;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TownNPCHome));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TownNPCGetTFIntoHouse));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NPCLiveInEvil));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TownNPCSpawnSpeed));
        panel.AddEnum(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoCD_FishermanQuest));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ModifyNPCHappiness));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NPCHappiness));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TravellingMerchantStay));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TravellingMerchantRefresh));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.QuickNurse));
    }
}