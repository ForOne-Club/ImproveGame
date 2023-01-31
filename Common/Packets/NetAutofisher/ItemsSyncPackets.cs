using ImproveGame.Content.Tiles;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    [AutoSync]
    public class ItemsSyncAllPacket : NetModule
    {
        private int tileEntityID;
        private bool isOpenFisher;
        private Item fishingPole;
        private Item bait;
        private Item accessory;
        private Item[] fish;

        public static ItemsSyncAllPacket Get(int tileEntityID, bool isOpenFisher, Item fishingPole, Item bait,
            Item accessory, Item[] fishes)
        {
            var module = NetModuleLoader.Get<ItemsSyncAllPacket>();
            module.tileEntityID = tileEntityID;
            module.isOpenFisher = isOpenFisher;
            module.fishingPole = fishingPole;
            module.bait = bait;
            module.accessory = accessory;
            module.fish = fishes;
            return module;
        }

        public override void Receive()
        {
            if (!TryGetTileEntityAs<TEAutofisher>(tileEntityID, out var autofisher) ||
                TileLoader.GetTile(Main.tile[autofisher.Position.ToPoint()].TileType) is not Autofisher fisherTile)
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
                    fisherTile.RightClick(autofisher.Position.X, autofisher.Position.Y);
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

    [AutoSync]
    public class ItemsStackChangePacket : NetModule
    {
        private int tileEntityID;
        private byte type;
        private int stackChange;

        public static ItemsStackChangePacket Get(int tileEntityID, byte type, int stackChange)
        {
            var module = NetModuleLoader.Get<ItemsStackChangePacket>();
            module.tileEntityID = tileEntityID;
            module.type = type;
            module.stackChange = stackChange;
            return module;
        }

        public override void Receive()
        {
            if (!TryGetTileEntityAs<TEAutofisher>(tileEntityID, out var autofisher))
            {
                return;
            }

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
                ItemSyncPacket.Get(tileEntityID, type).Send(ignoreClient: Sender, runLocally: false);
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

    /// <summary>
    /// 客户端调用: 向服务器发送获取 <seealso cref="TEAutofisher"/> 物品的请求
    /// </summary>
    [AutoSync]
    public class RequestItemPacket : NetModule
    {
        private int tileEntityID;
        private byte type;

        public static RequestItemPacket Get(int tileEntityID, byte type)
        {
            var module = NetModuleLoader.Get<RequestItemPacket>();
            module.tileEntityID = tileEntityID;
            module.type = type;
            return module;
        }

        /// <summary>
        /// 理论上只有服务器调用
        /// </summary>
        public override void Receive()
        {
            ItemSyncPacket.Get(tileEntityID, type).Send(Sender, -1, false);
        }
    }

    /// <summary>
    /// 发送物品，在服务器上Receive的话会转发到其他客户端
    /// </summary>
    public class ItemSyncPacket : NetModule
    {
        [AutoSync] private int tileEntityID;
        [AutoSync] private byte type;
        private Item fishingPole = null;
        private Item bait = null;
        private Item accessory = null;
        private Item[] fish = new Item[15];

        public static ItemSyncPacket Get(int tileEntityID, byte type)
        {
            var module = NetModuleLoader.Get<ItemSyncPacket>();
            module.tileEntityID = tileEntityID;
            module.type = type;
            TryGetTileEntityAs<TEAutofisher>(tileEntityID, out var autofisher);
            module.fishingPole = autofisher.fishingPole;
            module.bait = autofisher.bait;
            module.accessory = autofisher.accessory;
            module.fish = autofisher.fish;
            return module;
        }

        public override void Send(ModPacket p)
        {
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
            if (TryGetTileEntityAs<TEAutofisher>(tileEntityID, out var autofisher))
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