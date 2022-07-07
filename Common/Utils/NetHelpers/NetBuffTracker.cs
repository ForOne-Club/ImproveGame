using ImproveGame.Common.Players;
using System.IO;

namespace ImproveGame.Common.Utils.NetHelpers
{
    internal class NetBuffTracker
    {
        public static void HandlePacket(BinaryReader reader, int sender, MessageType type) {
            switch (type) {
                case MessageType.BuffTracker_ServerReceiveSpawnRateSlider:
                    ServerReceiveSpawnRateSlider(reader, sender);
                    break;
                case MessageType.BuffTracker_ClientReceiveSpawnRateSlider:
                    ClientReceiveSpawnRateSlider(reader);
                    break;
            }
        }

        public static void ServerReceiveSpawnRateSlider(BinaryReader reader, int sender) {
            float sliderValue = reader.ReadSingle();
            if (!Main.player[sender].TryGetModPlayer<BattlerPlayer>(out var modPlayer))
                return;
            modPlayer.SpawnRateSliderValue = sliderValue;

            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.BuffTracker_ClientReceiveSpawnRateSlider);
            packet.Write(sliderValue);
            packet.Write((byte)sender);
            packet.Send(-1, sender);
        }

        public static void ClientReceiveSpawnRateSlider(BinaryReader reader) {
            float sliderValue = reader.ReadSingle();
            byte playerIndex = reader.ReadByte();
            if (!Main.player[playerIndex].TryGetModPlayer<BattlerPlayer>(out var modPlayer))
                return;
            modPlayer.SpawnRateSliderValue = sliderValue;
        }

        public static void ClientSendSpawnRateSlider(float sliderValue) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.BuffTracker_ServerReceiveSpawnRateSlider);
            packet.Write(sliderValue);
            packet.Send(-1, -1);
        }
    }
}
