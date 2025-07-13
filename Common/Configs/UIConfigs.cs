using ImproveGame.Common.Configs.Elements;
using ImproveGame.UI.ModernConfig;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.Common;
using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs;

public class UIConfigs : ModConfig
{
    public static UIConfigs Instance { get; set; }
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [CustomModConfigItem(typeof(OpenUIConfigElement))]
    public object OpenConfig;

    [CustomModConfigItem(typeof(ResetUIPositionsButton))]
    public object ResetUIPositionsButton;

    [CustomModConfigItem(typeof(SuicideButtonElement))]
    public object SuicideButton;

    [Header("UIHeader")]

    [CustomModConfigItem(typeof(ThemeColorElement))]
    public ThemeType ThemeType;

    [DefaultValue(true)]
    public bool GlobeEffect;

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

    [DefaultValue(false)]
    public bool EnableBlur;

    [DefaultValue(false)]
    [DisplayCondition(nameof(UIConfigs), nameof(EnableBlur))]
    public bool SingleBlur;

    [Slider]
    [DefaultValue(2f)]
    [Range(1f, 8f)]
    [Increment(0.5f)]
    [DisplayCondition(nameof(UIConfigs), nameof(EnableBlur))]
    public float BlurZoomMultiplierDenominator;

    [Slider]
    [DefaultValue(2)]
    [Range(0, 10)]
    [Increment(1)]
    [DisplayCondition(nameof(UIConfigs), nameof(EnableBlur))]
    public int BlurIterationCount;

    [Slider]
    [DefaultValue(2f)]
    [Range(1f, 10f)]
    [Increment(1f)]
    [DisplayCondition(nameof(UIConfigs), nameof(EnableBlur))]
    public float BlurIterationOffsetMultiplier;

    [Slider]
    [DefaultValue(BlurMixingNumber.Three)]
    [DisplayCondition(nameof(UIConfigs), nameof(EnableBlur))]
    public BlurMixingNumber BlurMixingNumber;

    /// <summary>
    /// 显示调试数据
    /// </summary>
    [Header("GameHeader")]
    [DefaultValue(false)]
    public bool ShowMoreData;

    [DefaultValue(true)]
    public bool ShowShimmerInfo;

    [DefaultValue(true)]
    public bool ShowModName;

    public enum PAPDisplayMode
    {
        AlwaysDisplayed, WhenOpeningBackpack, NotDisplayed
    }

    [DefaultValue(PAPDisplayMode.AlwaysDisplayed)]
    [DrawTicks]
    [Slider]
    public PAPDisplayMode PlyInfo;

    /// <summary>
    /// 自动垃圾桶
    /// </summary>
    [DefaultValue(true)]
    public bool QoLAutoTrash;

    [DefaultValue(true)]
    public bool RecipeSearch;

    [DefaultValue(false)]
    public bool KeepFocus;

    [DefaultValue(true)]
    public bool ExplosionEffect;

    /// <summary>
    /// 隐藏无限续杯增益显示
    /// </summary>
    [DefaultValue(false)]
    public bool HideNoConsumeBuffs;

    /// <summary>
    /// 狱火圈不透明度
    /// </summary>
    [DefaultValue(0.3f)]
    [Range(0f, 1f)]
    [Increment(0.05f)]
    [Slider]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float InfernoTransparency;

    /// <summary>
    /// 隐身不透明度
    /// </summary>
    [DefaultValue(0.3f)]
    [Range(0f, 1f)]
    [Increment(0.05f)]
    [Slider]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float InvisibleTransparency;

    [DefaultValue(true)]
    public bool MagicMirrorInstantTp;

    [DefaultValue(true)]
    public bool AutoSummon;

    [DefaultValue(false)]
    public bool RemoveGraveyardVisual;

    [DefaultValue(false)]
    [DisplayCondition(nameof(UIConfigs), nameof(RemoveGraveyardVisual))]
    public bool RemoveGraveyardMist;

    [DefaultValue(false)]
    [DisplayCondition(nameof(UIConfigs), nameof(RemoveGraveyardVisual))]
    public bool RemoveGraveyardMusic;

    [DefaultValue(false)]
    public bool FckKeybindPopup;

    [Header("MinimapHeader")]
    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkDungeon;

    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkTemple;

    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkAether;

    [DefaultValue(0.9f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkFloatingIsland;

    [DefaultValue(1f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkPyramid;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkPlantera;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkEnchantedSword;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkMarbleCave;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkGraniteCave;

    [DefaultValue(0.8f)]
    [Increment(0.1f)]
    [Range(0f, 1.5f)]
    [Slider]
    [DisplayCondition(nameof(ImproveConfigs), nameof(ImproveConfigs.MinimapMark))]
    [CustomModConfigItem(typeof(RoundFloatElement))]
    public float MarkEmptyAutofisher;

    public override void OnLoaded()
    {
        Instance = this;
    }

    public override void OnChanged()
    {
        BlurMakeSystem.EnableBlur = EnableBlur;
        BlurMakeSystem.SingleBlur = SingleBlur;
        BlurMakeSystem.BlurZoomMultiplierDenominator = BlurZoomMultiplierDenominator;
        BlurMakeSystem.BlurIterationCount = BlurIterationCount;
        BlurMakeSystem.BlurIterationOffsetMultiplier = BlurIterationOffsetMultiplier;
        BlurMakeSystem.BlurMixingNumber = BlurMixingNumber;
    }
}
