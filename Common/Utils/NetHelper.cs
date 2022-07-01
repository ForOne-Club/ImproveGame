using ImproveGame.Common.Players;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Utils
{
    internal class NetHelper
    {
        public static void HandlePacket(BinaryReader reader, int sender) {
            MessageType type = (MessageType)reader.ReadByte();
            switch (type) {
                case MessageType.Autofish_ServerReceiveItem:
                    Autofish_ServerReceiveItem(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveItem:
                    Autofish_ClientReceiveItem(reader);
                    break;
                case MessageType.Autofish_ServerSyncItem:
                    Autofish_ServerSyncItem(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveSyncItem:
                    Autofish_ClientReceiveSyncItem(reader);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void Autofish_ClientReceiveSyncItem(BinaryReader reader) {
            byte type = reader.ReadByte();
            Item fishingPole = null;
            Item bait = null;
            Item[] fish = new Item[15];
            // 发送鱼
            if (type <= 14) {
                fish[type] = ItemIO.Receive(reader, true);
            }
            // 发送鱼竿
            if (type == 15) {
                fishingPole = ItemIO.Receive(reader, true);
            }
            // 发送鱼饵
            if (type == 16) {
                bait = ItemIO.Receive(reader, true);
            }
            // 全部发送
            if (type == 17) {
                fishingPole = ItemIO.Receive(reader, true);
                bait = ItemIO.Receive(reader, true);
                for (int i = 0; i < 15; i++)
                    fish[i] = ItemIO.Receive(reader, true);
            }
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                if (fishingPole is not null)
                    autofisher.fishingPole = fishingPole;
                if (bait is not null)
                    autofisher.bait = bait;
                if (type <= 14)
                    autofisher.fish[type] = fish[type];
                if (type == 17)
                    autofisher.fish = fish;
                AutofisherGUI.RequireRefresh = true;
            }
        }

        /// <summary>
        /// 由客户端执行，向服务器发送获取 <seealso cref="TEAutofisher"/> 物品的请求
        /// </summary>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        /// <param name="type">请求类型，小于或等于14为请求相应的fish，15为钓竿，16为鱼饵，17为全部</param>
        public static void Autofish_ClientSendSyncItem(Point16 point, byte type) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerSyncItem);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Write(type);
            packet.Send(-1, -1);
        }

        public static void Autofish_ServerSyncItem(BinaryReader reader, int sender) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            byte type = reader.ReadByte();
            // 发送到请求的客户端
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                var packet = ImproveGame.Instance.GetPacket();
                packet.Write((byte)MessageType.Autofish_ClientReceiveSyncItem);
                packet.Write(type);
                // 发送鱼
                if (type <= 14) {
                    ItemIO.Send(autofisher.fish[type], packet, true);
                }
                // 发送鱼竿
                if (type == 15) {
                    ItemIO.Send(autofisher.fishingPole, packet, true);
                }
                // 发送鱼饵
                if (type == 16) {
                    ItemIO.Send(autofisher.bait, packet, true);
                }
                // 全部发送
                if (type == 17) {
                    ItemIO.Send(autofisher.fishingPole, packet, true);
                    ItemIO.Send(autofisher.bait, packet, true);
                    for (int i = 0; i < 15; i++)
                        ItemIO.Send(autofisher.fish[i], packet, true);
                }
                packet.Write(x);
                packet.Write(y);
                packet.Send(sender, -1);
            }
            // 没有东西，传过去告诉他这TE没了
            else {
                var origin = MyUtils.GetTileOrigin(x, y);
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, 2, 2, TileChangeType.None);
            }
        }

        public static void Autofish_ServerReceiveItem(BinaryReader reader, int sender) {
            byte type = reader.ReadByte();
            var item = ItemIO.Receive(reader, true);
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            // 服务端的设置好
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                if (type <= 14) {
                    autofisher.fish[type] = item;
                }
                if (type == 15) {
                    autofisher.fishingPole = item;
                }
                if (type == 16) {
                    autofisher.bait = item;
                }
            }
            // 发送到客户端
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ClientReceiveItem);
            packet.Write(type);
            ItemIO.Send(item, packet, true);
            packet.Write(x);
            packet.Write(y);
            packet.Send(-1, sender); // 不发回给发送端
        }

        public static void Autofish_ClientReceiveItem(BinaryReader reader) {
            byte type = reader.ReadByte();
            var item = ItemIO.Receive(reader, true);
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            // 客户端的设置好
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                if (type <= 14) {
                    autofisher.fish[type] = item;
                }
                if (type == 15) {
                    autofisher.fishingPole = item;
                }
                if (type == 16) {
                    autofisher.bait = item;
                }
                AutofisherGUI.RequireRefresh = true;
            }
        }

        /// <summary>
        /// 由客户端执行，向服务器同步客户端的 <seealso cref="TEAutofisher"/> 内某物品
        /// </summary>
        /// <param name="type">请求类型，小于或等于14为请求相应的fish，15为钓竿，16为鱼饵</param>
        /// <param name="item">物品的实例</param>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        public static void Autofish_ClientSendItem(byte type, Item item, Point16 point) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerReceiveItem);
            packet.Write(type);
            ItemIO.Send(item, packet, true);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Send();
        }
    }

    internal enum MessageType : byte
    {
        Autofish_ServerReceiveItem,
        Autofish_ClientReceiveItem,
        Autofish_ServerSyncItem,
        Autofish_ClientReceiveSyncItem
    }
}
