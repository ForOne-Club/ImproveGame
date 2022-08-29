using ImproveGame.Common.Players;

namespace ImproveGame.Common.Packets
{
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

        public override void Send(ModPacket p)
        {
            p.Write(whoAmI);
            p.Write(sliderValue);
        }

        public override void Read(BinaryReader r)
        {
            whoAmI = r.ReadByte();
            sliderValue = r.ReadSingle();
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
