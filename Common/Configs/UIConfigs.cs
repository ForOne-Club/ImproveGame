using ImproveGame.Common.Configs.Elements;
using ImproveGame.Interface.Common;
using Newtonsoft.Json;
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

    [Header("UIHeader")]
    [DefaultValue(GlassType.Disabled)]
    [DrawTicks]
    public GlassType GlassVfx;

    [CustomModConfigItem(typeof(ThemeColorElement))]
    public ThemeType ThemeType;

    [DefaultValue(true)]
    [JsonIgnore]
    public bool BigBackpackButton;

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

    [Header("GameHeader")]
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
    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkDungeon;

    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkTemple;

    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkAether;

    [DefaultValue(0.9f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkFloatingIsland;

    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkPyramid;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkPlantera;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkEnchantedSword;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    public float MarkEmptyAutofisher;
}
