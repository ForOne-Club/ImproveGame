using ImproveGame.Common.Configs;
using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class VisualAndInterface : Category
{
    public override int ItemIconId => ItemID.EchoMonolith;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddToggle(uiConfig, nameof(uiConfig.GlassVfxOn));
        panel.AddEnum(uiConfig, nameof(uiConfig.ThemeType));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.GeneralFontOffsetY));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.BigFontOffsetY));
        panel.AddToggle(uiConfig, nameof(uiConfig.HideNoConsumeBuffs));
        panel.AddToggle(uiConfig, nameof(uiConfig.ExplosionEffect));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.InfernoTransparency));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.InvisibleTransparency));
    }
}