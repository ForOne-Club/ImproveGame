using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class EnemySettings : Category
{
    public override int ItemIconId => ItemID.BloodMoonStarter;

    public override string LocalizationKey => "EnemySettings";

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddValueSlider(Config, nameof(Config.NPCCoinDropRate));
        panel.AddToggle(Config, nameof(Config.SlimeExDrop));
        panel.AddToggle(Config, nameof(Config.LavalessLavaSlime));
        panel.AddToggle(Config, nameof(Config.BestiaryQuickUnlock));
        panel.AddValueSlider(Config, nameof(Config.BannerRequirement));
    }
}