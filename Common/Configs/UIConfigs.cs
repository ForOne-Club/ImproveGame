using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    [Label($"${path}.UIConfigs.Label")]
    public class UIConfigs : ModConfig
    {
        const string path = "Mods.ImproveGame.Config";
        public static UIConfigs Instance { get; set; }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override void OnLoaded() => Instance = this;

        [Label($"${path}.TextDrawOffsetY.Label")]
        [DefaultValue(5f)]
        [Range(0, 5f)]
        [Slider]
        [Increment(0.5f)]
        public float TextDrawOffsetY;
    }
}
