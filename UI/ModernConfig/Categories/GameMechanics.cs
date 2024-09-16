namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class GameMechanics : Category
{
    public override int ItemIconId => ItemID.Cog;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.BedEverywhere));
        panel.AddToggle(Config, nameof(Config.NoSleepRestrictions));
        panel.AddValueSlider(Config, nameof(Config.BedTimeRate));
        panel.AddEnum(Config, nameof(Config.BombsNotDamage));
        panel.AddToggle(Config, nameof(Config.NoPylonRestrictions));
        panel.AddToggle(Config, nameof(Config.FasterExtractinator));
        panel.AddToggle(Config, nameof(Config.MiddleEnableBank));
        panel.AddToggle(Config, nameof(Config.NoBiomeSpread));
        panel.AddToggle(Config, nameof(Config.JourneyResearch));
        panel.AddToggle(Config, nameof(Config.BanDamageVar));
        panel.AddToggle(Config, nameof(Config.NoLakeSizePenalty));
        panel.AddToggle(Config, nameof(Config.LightNotBlocked));
    }
}