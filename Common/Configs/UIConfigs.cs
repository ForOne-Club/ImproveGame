using ImproveGame.Common.Configs.Elements;
using ImproveGame.Interface.Common;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs;

public class UIConfigs : ModConfig
{
    public static UIConfigs Instance { get; set; }
    public override ConfigScope Mode => ConfigScope.ClientSide;
    public override void OnLoaded() => Instance = this;

    [CustomModConfigItem(typeof(OpenUIConfigElement))]
    public object OpenConfig;

    [CustomModConfigItem(typeof(ResetUIPositionsButton))]
    public object ResetUIPositionsButton;

    [CustomModConfigItem(typeof(SuicideButtonElement))]
    public object SuicideButton;

    [CustomModConfigItem(typeof(ThemeColorElement))]
    public ThemeType ThemeType;

    [ReloadRequired]
    public bool ResetNativeUI;

    [DefaultValue(7f)]
    [Range(0, 10f)]
    [Slider]
    [Increment(0.5f)]
    [CustomModConfigItem(typeof(FontOffsetPreview))]
    public float GeneralFontOffsetY;

    [DefaultValue(16f)]
    [Range(0, 20f)]
    [Increment(0.5f)]
    [CustomModConfigItem(typeof(BigFontOffsetPreview))]
    [Slider]
    public float BigFontOffsetY;

    [DefaultValue(false)]
    public bool ShowMoreData;

    [DefaultValue(true)]
    public bool ShowShimmerInfo;

    public enum PAPDisplayMode
    {
        AlwaysDisplayed, WhenOpeningBackpack, NotDisplayed
    }

    [DefaultValue(PAPDisplayMode.AlwaysDisplayed)]
    [DrawTicks]
    [Slider]
    public PAPDisplayMode PlyInfo;

    [DefaultValue(true)]
    public bool QoLAutoTrash;

    [DefaultValue(false)]
    public bool KeepFocus;

    [DefaultValue(true)]
    public bool ExplosionEffect;

    [DefaultValue(false)]
    public bool HideNoConsumeBuffs;

    [DefaultValue(0.3f)]
    [Range(0f, 1f)]
    [Increment(0.05f)]
    [Slider]
    public float InfernoTransparency;

    [DefaultValue(0.3f)]
    [Range(0f, 1f)]
    [Increment(0.05f)]
    [Slider]
    public float InvisibleTransparency;

    [DefaultValue(true)]
    public bool MagicMirrorInstantTp;

    [Header("MinimapHeader")]
    [DefaultValue(true)]
    public bool MarkDungeon;

    [DefaultValue(true)]
    public bool MarkTemple;

    [DefaultValue(true)]
    public bool MarkAether;

    [DefaultValue(true)]
    public bool MarkFloatingIsland;

    [DefaultValue(true)]
    public bool MarkPyramid;

    [DefaultValue(true)]
    public bool MarkPlantera;
}
