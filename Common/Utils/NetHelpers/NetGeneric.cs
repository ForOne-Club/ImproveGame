using System.IO;

namespace ImproveGame.Common.Utils.NetHelpers
{
    internal class NetGeneric
    {
        public static void HandlePacket(BinaryReader reader, int sender, MessageType type) {
            switch (type) {
                case MessageType.ServerReceivePlrItemUsing:
                    ServerReceivePlrItemUsing(reader, sender);
                    break;
                case MessageType.ClientReceivePlrItemUsing:
                    ClientReceivePlrItemUsing(reader);
                    break;
            }
        }

        public static void ClientReceivePlrItemUsing(BinaryReader reader) {
            float itemRotation = reader.ReadSingle();
            bool directionBool = reader.ReadBoolean();
            int direction = directionBool ? 1 : -1;
            short itemAnimation = reader.ReadInt16();

            byte sender = reader.ReadByte();
            Main.player[sender].itemRotation = itemRotation;
            Main.player[sender].direction = direction;
            Main.player[sender].itemAnimation = itemAnimation;
        }

        public static void ServerReceivePlrItemUsing(BinaryReader reader, int sender) {
            float itemRotation = reader.ReadSingle();
            bool directionBool = reader.ReadBoolean();
            int direction = directionBool ? 1 : -1;
            short itemAnimation = reader.ReadInt16();
            Main.player[sender].itemRotation = itemRotation;
            Main.player[sender].direction = direction;
            Main.player[sender].itemAnimation = itemAnimation;

            // 向除了发送者以外的玩家同步信息
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.ClientReceivePlrItemUsing);
            packet.Write(itemRotation);
            packet.Write(directionBool);
            packet.Write(itemAnimation);
            packet.Write((byte)sender);
            packet.Send(-1, sender);
        }

        /// <summary>
        /// 客户端执行，发送 <seealso cref="Player.itemRotation"/>, <seealso cref="Entity.direction"/> 和 <seealso cref="Player.itemAnimation"/> 信息到服务器和其他所有玩家
        /// </summary>
        /// <param name="player"> <seealso cref="Player"/>实例 </param>
        public static void ClientSendPlrItemUsing(Player player) {
            float itemRotation = player.itemRotation;
            bool directionBool = player.direction == 1;

            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.ServerReceivePlrItemUsing);
            packet.Write(itemRotation);
            packet.Write(directionBool);
            packet.Write((short)player.itemAnimation);
            packet.Send(-1, -1);
        }
    }
}
