using ImproveGame.Common.Configs;
using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class PlayerAbility : Category
{
    public override int ItemIconId => ItemID.GuideVoodooDoll;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddToggle(Config, nameof(Config.SuperVault));
        panel.AddToggle(Config, nameof(Config.SuperVoidVault));
        panel.AddToggle(Config, nameof(Config.SmartVoidVault));
        panel.AddValueSlider(Config, nameof(Config.GrabDistance));
        panel.AddValueSlider(Config, nameof(Config.ExtraToolSpeed));
        panel.AddToggle(Config, nameof(Config.ModifyPlayerPlaceSpeed));
        panel.AddValueSlider(Config, nameof(Config.ModifyPlayerTileRange));
        panel.AddValueSlider(Config, nameof(Config.ExtraPlayerBuffSlots));
        panel.AddToggle(Config, nameof(Config.RespawnWithFullHP));
        panel.AddToggle(Config, nameof(Config.DontDeleteBuff));
        panel.AddValueSlider(Config, nameof(Config.ResurrectionTimeShortened));
        panel.AddValueSlider(Config, nameof(Config.BOSSBattleResurrectionTimeShortened));
        panel.AddToggle(Config, nameof(Config.BanTombstone));
        panel.AddToggle(Config, nameof(Config.LongerExpertDebuff));
        panel.AddToggle(Config, nameof(Config.SimpleVeinMining));
        panel.AddToggle(uiConfig, nameof(uiConfig.AutoSummon));
    }
}