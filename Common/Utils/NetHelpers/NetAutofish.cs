using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.GUI;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Utils.NetHelpers
{
    internal class NetAutofish
    {
        public static void HandlePacket(BinaryReader reader, int sender, MessageType type) {
            switch (type) {
                case MessageType.Autofish_ServerReceiveItem:
                    ServerReceiveItem(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveItem:
                    ClientReceiveItem(reader);
                    break;
                case MessageType.Autofish_ServerSyncItem:
                    ServerSyncItem(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveSyncItem:
                    ClientReceiveSyncItem(reader);
                    break;
                case MessageType.Autofish_ServerReceiveLocatePoint:
                    ServerReceiveLocatePoint(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveLocatePoint:
                    ClientReceiveLocatePoint(reader);
                    break;
                case MessageType.Autofish_ServerReceiveStackChange:
                    ServerReceiveStackChange(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveTipChange:
                    ClientReceiveTipChange(reader);
                    break;
                case MessageType.Autofish_ServerReceiveAutofisherPosition:
                    ServerReceiveAutofisherPosition(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceiveOpenRequest:
                    ClientReceiveOpenRequest(reader);
                    break;
                case MessageType.Autofish_ServerReceiveOpenRequest:
                    ServerReceiveOpenRequest(reader, sender);
                    break;
                case MessageType.Autofish_ClientReceivePlayersToggle:
                    ClientReceivePlayersToggle(reader);
                    break;
            }
        }

        /// <summary>
        /// 由服务器执行，向所有玩家同步钓鱼信息提示
        /// </summary>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        public static void ServerSendTipChange(Point16 point, string tip) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ClientReceiveTipChange);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Write(tip);
            packet.Send(-1, -1);
        }

        public static void ClientReceiveTipChange(BinaryReader reader) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            string tip = reader.ReadString();
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                autofisher.SetFishingTip(tip);
            }
        }

        public static void ServerReceiveStackChange(BinaryReader reader, int sender) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            byte type = reader.ReadByte();
            int stackChange = reader.ReadInt32();
            // 同步个堆叠
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                // 发送鱼
                if (type <= 14) {
                    autofisher.fish[type].stack += stackChange;
                }
                // 发送鱼饵
                if (type == 16) {
                    autofisher.bait.stack += stackChange;
                }
                // 发送到其他的所有客户端
                ServerSendSyncItem(new Point16(x, y), type, sender);
            }
        }

        /// <summary>
        /// 由客户端执行，向服务器发送更改堆叠而不是发送物品（避免延迟导致两端不同步而刷物品）
        /// </summary>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        /// <param name="type">请求类型，小于或等于14为请求相应的fish，16为鱼饵，其他的没必要同步</param>
        /// <param name="stackChange">更改的堆叠量</param>
        public static void ClientSendStackChange(Point16 point, byte type, int stackChange) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerReceiveStackChange);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Write(type);
            packet.Write(stackChange);
            packet.Send(-1, -1);
        }

        public static void ServerReceiveLocatePoint(BinaryReader reader, int sender) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            short locateX = reader.ReadInt16();
            short locateY = reader.ReadInt16();
            // 发送到其他的所有客户端
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                autofisher.locatePoint = new(locateX, locateY);
                var packet = ImproveGame.Instance.GetPacket();
                packet.Write((byte)MessageType.Autofish_ClientReceiveLocatePoint);
                packet.Write(x);
                packet.Write(y);
                packet.Write(locateX);
                packet.Write(locateY);
                packet.Send(-1, sender);
            }
            // 没有东西，传过去告诉他这TE没了
            else {
                var origin = MyUtils.GetTileOrigin(x, y);
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, 2, 2, TileChangeType.None);
            }
        }

        public static void ClientReceiveLocatePoint(BinaryReader reader) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            short locateX = reader.ReadInt16();
            short locateY = reader.ReadInt16();
            // 发送到请求的客户端
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                autofisher.locatePoint = new(locateX, locateY);
            }
        }

        /// <summary>
        /// 由客户端执行，向服务器同步定位点
        /// </summary>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        /// <param name="locatePoint">定位点坐标</param>
        public static void ClientSendLocatePoint(Point16 point, Point16 locatePoint) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerReceiveLocatePoint);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Write(locatePoint.X);
            packet.Write(locatePoint.Y);
            packet.Send(-1, -1);
        }

        public static void ClientReceiveSyncItem(BinaryReader reader) {
            byte type = reader.ReadByte();
            Item fishingPole = null;
            Item bait = null;
            Item accessory = null;
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
            // 发送饰品
            if (type == 17) {
                accessory = ItemIO.Receive(reader, true);
            }
            // 全部发送
            if (type == 18) {
                fishingPole = ItemIO.Receive(reader, true);
                bait = ItemIO.Receive(reader, true);
                accessory = ItemIO.Receive(reader, true);
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
                if (accessory is not null)
                    autofisher.accessory = accessory;
                if (type <= 14)
                    autofisher.fish[type] = fish[type];
                if (type == 18)
                    autofisher.fish = fish;
                AutofisherGUI.RequireRefresh = true;
                if (AutofisherGUI.Visible)
                    UISystem.Instance.AutofisherGUI.RefreshItems();
            }
        }

        /// <summary>
        /// 由客户端执行，向服务器发送获取 <seealso cref="TEAutofisher"/> 物品的请求
        /// </summary>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        /// <param name="type">请求类型，小于或等于14为请求相应的fish，15为钓竿，16为鱼饵，17为饰品，18为全部</param>
        public static void ClientSendSyncItem(Point16 point, byte type) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerSyncItem);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Write(type);
            packet.Send(-1, -1);
        }

        public static void ServerSyncItem(BinaryReader reader, int sender) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            byte type = reader.ReadByte();
            ServerSendSyncItem(new Point16(x, y), type, sender);
        }

        public static void ServerSendSyncItem(Point16 point, byte type, int sender = -1) {
            // 发送到请求的客户端
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(point.X, point.Y, out var autofisher)) {
                var packet = ImproveGame.Instance.GetPacket();
                packet.Write((byte)MessageType.Autofish_ClientReceiveSyncItem);
                packet.Write(type);
                // 发送鱼
                if (type <= 14) {
                    if (autofisher.fish[type] is null)
                        ItemIO.Send(new(), packet, true);
                    else
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
                // 发送饰品
                if (type == 17) {
                    ItemIO.Send(autofisher.accessory, packet, true);
                }
                // 全部发送
                if (type == 18) {
                    ItemIO.Send(autofisher.fishingPole, packet, true);
                    ItemIO.Send(autofisher.bait, packet, true);
                    ItemIO.Send(autofisher.accessory, packet, true);
                    for (int i = 0; i < 15; i++) {
                        if (autofisher.fish[i] is null)
                            ItemIO.Send(new(), packet, true);
                        else
                            ItemIO.Send(autofisher.fish[i], packet, true);
                    }
                }
                packet.Write(point.X);
                packet.Write(point.Y);
                packet.Send(sender, -1);
            }
            // 没有东西，传过去告诉他这TE没了
            else {
                var origin = MyUtils.GetTileOrigin(point.X, point.Y);
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, 2, 2, TileChangeType.None);
            }
        }

        public static void ServerReceiveItem(BinaryReader reader, int sender) {
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
                if (type == 17) {
                    autofisher.accessory = item;
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

        public static void ClientReceiveItem(BinaryReader reader) {
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
                if (type == 17) {
                    autofisher.accessory = item;
                }
                AutofisherGUI.RequireRefresh = true;
                if (AutofisherGUI.Visible)
                    UISystem.Instance.AutofisherGUI.RefreshItems();
            }
        }

        /// <summary>
        /// 由客户端执行，向服务器同步客户端的 <seealso cref="TEAutofisher"/> 内某物品
        /// </summary>
        /// <param name="type">请求类型，小于或等于14为请求相应的fish，15为钓竿，16为鱼饵，17为饰品</param>
        /// <param name="item">物品的实例</param>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        public static void ClientSendItem(byte type, Item item, Point16 point) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerReceiveItem);
            packet.Write(type);
            ItemIO.Send(item, packet, true);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Send();
        }

        public static void ClientSendAutofisherPosition(short x, short y) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerReceiveAutofisherPosition);
            packet.Write(x);
            packet.Write(y);
            packet.Send(-1, -1); // 不发回给发送端
        }

        public static void ServerReceiveAutofisherPosition(BinaryReader reader, int sender) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            // 服务端的设置好
            Main.player[sender].GetModPlayer<AutofishPlayer>().SetAutofisher(new(x, y), false);
            // 给其他玩家发开关包
            ServerSendPlayerToggle(new(x, y), (byte)sender, -1, sender);
        }

        // 这个只会在服务器允许打开才发生，要是不允许服务器没有任何回应
        public static void ClientReceiveOpenRequest(BinaryReader reader) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            var fishingPole = ItemIO.Receive(reader, true);
            var bait = ItemIO.Receive(reader, true);
            var accessory = ItemIO.Receive(reader, true);
            Item[] fish = new Item[15];
            for (int i = 0; i < 15; i++)
                fish[i] = ItemIO.Receive(reader, true);
            if (TileLoader.GetTile(Main.tile[x, y].TileType) is Autofisher && MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher)) {
                var fisherTile = TileLoader.GetTile(Main.tile[x, y].TileType) as Autofisher;
                fisherTile.ServerOpenRequest = true;
                // 为了放置开箱一瞬间拿上次开的东西，在这里顺便把机内容同步了
                AutofisherGUI.RequireRefresh = true;
                autofisher.fishingPole = fishingPole;
                autofisher.bait = bait;
                autofisher.accessory = accessory;
                autofisher.fish = fish;
                fisherTile.RightClick(x, y);
            }
        }

        public static void ServerReceiveOpenRequest(BinaryReader reader, int sender) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            var origin = new Point16(x, y);
            for (int k = 0; k < Main.maxPlayers; k++) {
                var player = Main.player[k];
                if (player.active && !player.dead && player.TryGetModPlayer<AutofishPlayer>(out var modPlayer) && modPlayer.Autofisher == origin) {
                    return;
                }
            }

            if (!MyUtils.TryGetTileEntityAs<TEAutofisher>(x, y, out var autofisher))
                return;

            // 没玩家，发允许包
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ClientReceiveOpenRequest);
            packet.Write(x);
            packet.Write(y);
            // 为了放置开箱一瞬间拿上次开的东西，在这里顺便把机器内容同步了
            ItemIO.Send(autofisher.fishingPole, packet, true);
            ItemIO.Send(autofisher.bait, packet, true);
            ItemIO.Send(autofisher.accessory, packet, true);
            for (int i = 0; i < 15; i++) {
                if (autofisher.fish[i] is null)
                    ItemIO.Send(new(), packet, true);
                else
                    ItemIO.Send(autofisher.fish[i], packet, true);
            }
            packet.Send(sender, -1); // 只发回给发送端

            // 服务器设置好
            Main.player[sender].GetModPlayer<AutofishPlayer>().SetAutofisher(origin, false);
        }

        /// <summary>
        /// 客户端执行，问服务器可不可以开箱
        /// </summary>
        /// <param name="point">位置</param>
        public static void ClientSendOpenRequest(Point16 point) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ServerReceiveOpenRequest);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Send(-1, -1);
        }

        public static void ClientReceivePlayersToggle(BinaryReader reader) {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            byte plr = reader.ReadByte();
            if (AutofishPlayer.TryGet(Main.player[plr], out var modPlayer)) {
                modPlayer.SetAutofisher(new(x, y), false);
            }
        }

        /// <summary>
        /// 给除了打开玩家以外的其他玩家发送状态同步包，告诉它哪个玩家执行了打开
        /// </summary>
        /// <param name="point"> <seealso cref="TEAutofisher"/> 坐标</param>
        /// <param name="plr">  <seealso cref="Entity.whoAmI"/>  </param>
        public static void ServerSendPlayerToggle(Point16 point, byte plr, int toWho, int ignoreWho) {
            var packet = ImproveGame.Instance.GetPacket();
            packet.Write((byte)MessageType.Autofish_ClientReceivePlayersToggle);
            packet.Write(point.X);
            packet.Write(point.Y);
            packet.Write(plr);
            packet.Send(toWho, ignoreWho);
        }
    }
}
