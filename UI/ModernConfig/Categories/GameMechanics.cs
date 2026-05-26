using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class GameMechanics : Category
{
    public override int ItemIconId => ItemID.Cog;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BedEverywhere));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoSleepRestrictions));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BedTimeRate));
        panel.AddEnum(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BombsNotDamage));
        panel.AddEnum(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.DisableNonPlayerBombsExplosions));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.FasterExtractinator));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.MiddleEnableBank));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoBiomeSpread));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.JourneyResearch));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BanDamageVar));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.NoLakeSizePenalty));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.LightNotBlocked));
        panel.AddToggle(uiConfig, nameof(uiConfig.KeepFocus));
    }
}