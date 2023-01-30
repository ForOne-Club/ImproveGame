using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label($"${path}.UIConfigs.Label")]
    public class UIConfigs : ModConfig
    {
        private const string path = "Mods.ImproveGame.Config";
        public static UIConfigs Instance { get; set; }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override void OnLoaded() => Instance = this;

        [Label($"${path}.ResetNativeUI.Label")]
        [ReloadRequired]
        public bool ResetNativeUI;

        [Label($"${path}.GeneralFontOffsetY.Label")]
        [DefaultValue(7f)]
        [Range(0, 10f)]
        [Slider]
        [Increment(0.5f)]
        public float GeneralFontOffsetY;

        [Label($"${path}.BigFontOffsetY.Label")]
        [DefaultValue(16f)]
        [Range(0, 20f)]
        [Slider]
        [Increment(0.5f)]
        public float BigFontOffsetY;

        [Label($"${path}.ShowMoreData.Label")]
        [DefaultValue(false)]
        public bool ShowMoreData;

        public enum PlyInfoDisplayMode
        {
            [Label($"${path}.PlyInfo.AlwaysDisplayed")]
            AlwaysDisplayed,
            [Label($"${path}.PlyInfo.WhenOpeningInventory")]
            WhenOpeningBackpack,
            [Label($"${path}.PlyInfo.NotDisplayed")]
            NotDisplayed
        }

        [Label($"${path}.PlyInfo.Label")]
        [DefaultValue(PlyInfoDisplayMode.AlwaysDisplayed)]
        [Slider]
        [DrawTicks]
        public PlyInfoDisplayMode PlyInfo;
    }
}
