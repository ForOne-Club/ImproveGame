using ImproveGame.Common.Players;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Utils.NetHelpers
{
    internal class NetBigBag
    {
        public static void HandlePacket(BinaryReader reader, int sender, MessageType type) {
            switch (type) {
                case MessageType.BigBag_ReceiveSlot:
                    ReceiveSlotSync(reader);
                    break;
                case MessageType.BigBag_ReceiveAllSlot:
                    ReceiveAllSlotSync(reader);
                    break;
            }
        }

        public static void ReceiveSlotSync(BinaryReader reader) {
            byte slot = reader.ReadByte();
            var item = ItemIO.Receive(reader, true, true);
            int whichPlr = reader.ReadByte();

            if (!DataPlayer.TryGet(Main.player[whichPlr], out var modPlayer))
                return;

            if (modPlayer.SuperVault is not null)
                modPlayer.SuperVault[slot] = item;

            // 转发
            if (Main.netMode is NetmodeID.Server)
            {
                SendSlot(slot, item, (byte)whichPlr, -1, whichPlr);
            }
        }

        public static void SendSlot(int slot, Item item, int whichPlayer, int toWho, int ignoreWho)
        {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.BigBag_ReceiveSlot);
            packet.Write((byte)slot);
            ItemIO.Send(item, packet, true, true);
            packet.Write((byte)whichPlayer);
            packet.Send(toWho, ignoreWho);
        }

        public static void ReceiveAllSlotSync(BinaryReader reader)
        {
            byte whoAmI = reader.ReadByte();

            if (!DataPlayer.TryGet(Main.player[whoAmI], out var modPlayer))
                return;

            for (int i = 0; i < 100; i++)
            {
                modPlayer.SuperVault[i] = ItemIO.Receive(reader, true, true);
            }
        }

        /// <summary>
        /// 仅用于SyncPlayer，一次性发送所有槽位
        /// </summary>
        public static void SendAllSlot(DataPlayer dataPlayer, int toWho, int ignoreWho)
        {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.BigBag_ReceiveAllSlot);
            packet.Write((byte)dataPlayer.Player.whoAmI);
            for (int i = 0; i < 100; i++)
            {
                if (dataPlayer.SuperVault[i] is null)
                {
                    dataPlayer.SuperVault[i] = new();
                }
                ItemIO.Send(dataPlayer.SuperVault[i], packet, true, true);
            }
            packet.Send(toWho, ignoreWho);
        }
    }
}
