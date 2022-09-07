using ImproveGame.Content.Tiles;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    public class ItemsSyncAllPacket : NetModule
    {
        private Point16 position;
        private bool isOpenFisher;
        private Item fishingPole;
        private Item bait;
        private Item accessory;
        private Item[] fish;

        public static ItemsSyncAllPacket Get(Point16 position, bool isOpenFisher, Item fishingPole, Item bait, Item accessory, Item[] fishes)
        {
            var module = NetModuleLoader.Get<ItemsSyncAllPacket>();
            module.position = position;
            module.isOpenFisher = isOpenFisher;
            module.fishingPole = fishingPole;
            module.bait = bait;
            module.accessory = accessory;
            module.fish = fishes;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(isOpenFisher);
            ItemIO.Send(fishingPole, p, true);
            ItemIO.Send(bait, p, true);
            ItemIO.Send(accessory, p, true);
            p.Write(fish);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            isOpenFisher = r.ReadBoolean();
            fishingPole = ItemIO.Receive(r, true);
            bait = ItemIO.Receive(r, true);
            accessory = ItemIO.Receive(r, true);
            fish = r.ReadItemArray();
        }

        public override void Receive()
        {
            if (TileLoader.GetTile(Main.tile[position.ToPoint()].TileType) is not Autofisher fisherTile || !TryGetTileEntityAs<TEAutofisher>(position, out var autofisher))
            {
                return;
            }

            autofisher.fishingPole = fishingPole;
            autofisher.bait = bait;
            autofisher.accessory = accessory;
            autofisher.fish = fish;

            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                AutofisherGUI.RequireRefresh = true;
                if (isOpenFisher)
                {
                    fisherTile.ServerOpenRequest = true;
                    fisherTile.RightClick(position.X, position.Y);
                    return;
                }
                if (AutofisherGUI.Visible)
                    UISystem.Instance.AutofisherGUI.RefreshItems();
            }
            else
            {
                Send(-1, Sender, false);
            }
        }
    }

    public class ItemsStackChangePacket : NetModule
    {
        private Point16 position;
        private byte type;
        private int stackChange;

        public static ItemsStackChangePacket Get(Point16 position, byte type, int stackChange)
        {
            var module = NetModuleLoader.Get<ItemsStackChangePacket>();
            module.position = position;
            module.type = type;
            module.stackChange = stackChange;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(type);
            p.Write(stackChange);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            type = r.ReadByte();
            stackChange = r.ReadInt32();
        }

        public override void Receive()
        {
            if (TryGetTileEntityAs<TEAutofisher>(position, out var autofisher))
            {
                // 发送鱼
                if (type <= 14)
                {
                    autofisher.fish[type].stack += stackChange;
                }
                // 发送鱼饵
                if (type == 16)
                {
                    autofisher.bait.stack += stackChange;
                }
                // 发送到其他的所有客户端
                if (Main.netMode is NetmodeID.Server)
                {
                    ItemSyncPacket.Get(position, type).Send(ignoreClient: Sender, runLocally: false);
                }
                // 客户端刷新框
                else
                {
                    AutofisherGUI.RequireRefresh = true;
                    if (AutofisherGUI.Visible)
                        UISystem.Instance.AutofisherGUI.RefreshItems();
                }
            }
        }
    }

    /// <summary>
    /// 客户端调用: 向服务器发送获取 <seealso cref="TEAutofisher"/> 物品的请求
    /// </summary>
    public class RequestItemPacket : NetModule
    {
        private Point16 position;
        private byte type;

        public static RequestItemPacket Get(Point16 position, byte type)
        {
            var module = NetModuleLoader.Get<RequestItemPacket>();
            module.position = position;
            module.type = type;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(type);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            type = r.ReadByte();
        }

        /// <summary>
        /// 理论上只有服务器调用
        /// </summary>
        public override void Receive()
        {
            ItemSyncPacket.Get(position, type).Send(Sender, -1, false);
        }
    }

    /// <summary>
    /// 发送物品，在服务器上Receive的话会转发到其他客户端
    /// </summary>
    public class ItemSyncPacket : NetModule
    {
        private Point16 position;
        private byte type;
        private Item fishingPole = null;
        private Item bait = null;
        private Item accessory = null;
        private Item[] fish = new Item[15];

        public static ItemSyncPacket Get(Point16 position, byte type)
        {
            var module = NetModuleLoader.Get<ItemSyncPacket>();
            module.position = position;
            module.type = type;
            TryGetTileEntityAs<TEAutofisher>(position, out var autofisher);
            module.fishingPole = autofisher.fishingPole;
            module.bait = autofisher.bait;
            module.accessory = autofisher.accessory;
            module.fish = autofisher.fish;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(type);

            // 发送鱼
            if (type <= 14)
            {
                if (fish[type] is null)
                    ItemIO.Send(new(), p, true);
                else
                    ItemIO.Send(fish[type], p, true);
            }
            // 发送鱼竿
            if (type is 15)
            {
                ItemIO.Send(fishingPole, p, true);
            }
            // 发送鱼饵
            if (type is 16)
            {
                ItemIO.Send(bait, p, true);
            }
            // 发送饰品
            if (type is 17)
            {
                ItemIO.Send(accessory, p, true);
            }
            // 全部发送
            if (type is 18)
            {
                ItemIO.Send(fishingPole, p, true);
                ItemIO.Send(bait, p, true);
                ItemIO.Send(accessory, p, true);
                p.Write(fish);
            }
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            type = r.ReadByte();

            fishingPole = null;
            bait = null;
            accessory = null;
            fish = new Item[15];

            // 发送鱼
            if (type <= 14)
            {
                fish[type] = ItemIO.Receive(r, true);
            }
            // 发送鱼竿
            if (type is 15)
            {
                fishingPole = ItemIO.Receive(r, true);
            }
            // 发送鱼饵
            if (type is 16)
            {
                bait = ItemIO.Receive(r, true);
            }
            // 发送饰品
            if (type is 17)
            {
                accessory = ItemIO.Receive(r, true);
            }
            // 全部发送
            if (type is 18)
            {
                fishingPole = ItemIO.Receive(r, true);
                bait = ItemIO.Receive(r, true);
                accessory = ItemIO.Receive(r, true);
                fish = r.ReadItemArray();
            }
        }

        public override void Receive()
        {
            if (TryGetTileEntityAs<TEAutofisher>(position, out var autofisher))
            {
                if (type <= 14)
                    autofisher.fish[type] = fish[type];
                if (type is 15 or 18)
                    autofisher.fishingPole = fishingPole;
                if (type is 16 or 18)
                    autofisher.bait = bait;
                if (type is 17 or 18)
                    autofisher.accessory = accessory;
                if (type == 18)
                    autofisher.fish = fish;
                if (Main.netMode is NetmodeID.MultiplayerClient)
                {
                    AutofisherGUI.RequireRefresh = true;
                    if (AutofisherGUI.Visible)
                        UISystem.Instance.AutofisherGUI.RefreshItems();
                }
                else if (Main.netMode is NetmodeID.Server)
                {
                    Send(ignoreClient: Sender, runLocally: false);
                }
            }
        }
    }
}
