using ImproveGame.Common.ModPlayers;

namespace ImproveGame.Packets
{
    [AutoSync]
    public class SpawnRateSlider : NetModule
    {
        private byte whoAmI;
        private float sliderValue;

        public static SpawnRateSlider Get(int whoAmI, float sliderValue)
        {
            var module = NetModuleLoader.Get<SpawnRateSlider>();
            module.sliderValue = sliderValue;
            module.whoAmI = (byte)whoAmI;
            return module;
        }

        public override void Receive()
        {
            if (!Main.player[whoAmI].TryGetModPlayer<BattlerPlayer>(out var modPlayer))
                return;

            modPlayer.SpawnRateSliderValue = sliderValue;

            if (Main.netMode is NetmodeID.Server)
            {
                Send(-1, whoAmI, false);
            }
        }
    }
}
