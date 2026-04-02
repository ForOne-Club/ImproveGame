using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class PlayerAbility : Category
{
    public override int ItemIconId => ItemID.GuideVoodooDoll;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SuperVault));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SuperVoidVault));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SmartVoidVault));
        panel.AddEnum(uiConfig, nameof(uiConfig.PlyInfo));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.GrabDistance));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ExtraToolSpeed));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ModifyPlayerPlaceSpeed));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ModifyPlayerTileRange));
        panel.AddList(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.TileSpeed_Blacklist));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ExtraPlayerBuffSlots));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.RespawnWithFullHP));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.DontDeleteBuff));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.ResurrectionTimeShortened));
        panel.AddValueSlider(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BOSSBattleResurrectionTimeShortened));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.BanTombstone));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.LongerExpertDebuff));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.SimpleVeinMining));
        panel.AddToggle(ImproveConfigs.Instance, nameof(ImproveConfigs.Instance.DisableVeinMiningPopup));
        panel.AddToggle(uiConfig, nameof(uiConfig.AutoSummon));
    }
}