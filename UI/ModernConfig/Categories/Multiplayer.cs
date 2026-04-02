using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Multiplayer : Category
{
    public override int ItemIconId => ItemID.GolfCupFlagRed;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.OnlyHost));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.OnlyHostByPassword));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ShareCraftingStation));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ShareInfBuffs));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ShareRange));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TeamAutoJoin));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BedOnlyOne));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoConditionTP));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.DummyCustomAIStyleAllowed));
    }
}