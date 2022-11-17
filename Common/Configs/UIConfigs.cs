using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Configs
{
    public class UIConfigs : ModConfig
    {
        public static UIConfigs Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override void OnLoaded() => Instance = this;

        [DefaultValue(5f)]
        [Range(0, 5f)]
        [Slider]
        [Increment(0.5f)]
        public float UIYAxisOffset;
    }
}
