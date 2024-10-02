namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class PylonMechanics : Category
{
    public override int ItemIconId => ItemID.TeleportationPylonVictory;
    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.AddToggle(Config, nameof(Config.PylonPlaceNoRestriction));
        panel.AddToggle(Config, nameof(Config.PylonTeleNoBiome));
        panel.AddToggle(Config, nameof(Config.PylonTeleNoDanger));
        panel.AddToggle(Config, nameof(Config.PylonTeleNoNear));
        panel.AddToggle(Config, nameof(Config.PylonTeleNoNPC));
    }
}
