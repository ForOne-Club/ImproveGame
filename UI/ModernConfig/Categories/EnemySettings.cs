using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class EnemySettings : Category
{
    public override int ItemIconId => ItemID.BloodMoonStarter;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NPCCoinDropRate));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SlimeExDrop));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.LavalessLavaSlime));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BestiaryQuickUnlock));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BannerRequirement));
    }
}