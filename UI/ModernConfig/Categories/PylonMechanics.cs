using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class PylonMechanics : Category
{
    public override int ItemIconId => ItemID.TeleportationPylonVictory;
    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.PylonPlaceNoRestriction));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.PylonTeleNoBiome));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.PylonTeleNoDanger));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.PylonTeleNoNear));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.PylonTeleNoNPC));
    }
}
