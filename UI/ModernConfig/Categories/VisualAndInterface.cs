using ImproveGame.Common.Configs;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class VisualAndInterface : Category
{
    public override int ItemIconId => ItemID.EchoMonolith;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        var uiConfig = UIConfigs.Instance;
        panel.AddEnum(uiConfig, nameof(uiConfig.ThemeType));

        #region 模糊相关配置
        // 是否开启模糊
        panel.AddToggle(uiConfig, nameof(uiConfig.EnableBlur));
        // 单窗口模糊
        panel.AddToggle(uiConfig, nameof(uiConfig.SingleBlur));
        // 模糊缩放偏移分母
        panel.AddValueSlider(uiConfig, nameof(uiConfig.BlurZoomMultiplierDenominator));
        // 模糊迭代次数
        panel.AddValueSlider(uiConfig, nameof(uiConfig.BlurIterationCount));
        // 模糊迭代偏移乘数
        panel.AddValueSlider(uiConfig, nameof(uiConfig.BlurIterationOffsetMultiplier));
        // 混合颜色数
        panel.AddEnum(uiConfig, nameof(uiConfig.BlurMixingNumber));
        #endregion

        panel.AddToggle(uiConfig, nameof(uiConfig.GlobeEffect));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.GeneralFontOffsetY));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.BigFontOffsetY));
        panel.AddToggle(uiConfig, nameof(uiConfig.HideNoConsumeBuffs));
        panel.AddToggle(uiConfig, nameof(uiConfig.ExplosionEffect));
        panel.AddToggle(uiConfig, nameof(uiConfig.RemoveGraveyardVisual));
        panel.AddToggle(uiConfig, nameof(uiConfig.RemoveGraveyardMist));
        panel.AddToggle(uiConfig, nameof(uiConfig.RemoveGraveyardMusic));
        panel.AddColor(uiConfig, nameof(uiConfig.SpelunkerColor));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.InfernoTransparency));
        panel.AddValueSlider(uiConfig, nameof(uiConfig.InvisibleTransparency));
    }
}