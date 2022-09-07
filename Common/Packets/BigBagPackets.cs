using ImproveGame.Common.Players;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Packets
{
    public class BigBagSlotPacket : NetModule
    {
        private byte whoAmI;
        private byte slot;
        private Item item;

        public static BigBagSlotPacket Get(Item item, int whoAmI, int slot)
        {
            var module = NetModuleLoader.Get<BigBagSlotPacket>();
            module.item = item;
            module.whoAmI = (byte)whoAmI;
            module.slot = (byte)slot;
            return module;
        }

        public static BigBagSlotPacket Get(DataPlayer dataPlayer, int slot)
        {
            var module = NetModuleLoader.Get<BigBagSlotPacket>();
            module.item = dataPlayer.SuperVault[slot];
            module.whoAmI = (byte)dataPlayer.Player.whoAmI;
            module.slot = (byte)slot;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(whoAmI);
            p.Write(slot);
            ItemIO.Send(item, p, true, true);
        }

        public override void Read(BinaryReader r)
        {
            whoAmI = r.ReadByte();
            slot = r.ReadByte();
            item = ItemIO.Receive(r, true, true);
        }

        public override void Receive()
        {
            if (!DataPlayer.TryGet(Main.player[whoAmI], out var modPlayer))
                return;

            if (modPlayer.SuperVault is not null)
                modPlayer.SuperVault[slot] = item;

            // 转发
            if (Main.netMode is NetmodeID.Server)
            {
                var packet = Get(modPlayer, slot);
                packet.Send(-1, whoAmI, false);
            }
        }
    }

    public class BigBagAllSlotsPacket : NetModule
    {
        private DataPlayer dataPlayer;
        private byte whoAmI;
        private Item[] items = new Item[100];

        public static BigBagAllSlotsPacket Get(DataPlayer dataPlayer)
        {
            var module = NetModuleLoader.Get<BigBagAllSlotsPacket>();
            module.dataPlayer = dataPlayer;
            module.whoAmI = (byte)dataPlayer.Player.whoAmI;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(whoAmI);
            for (int i = 0; i < 100; i++)
            {
                if (dataPlayer.SuperVault[i] is null)
                {
                    dataPlayer.SuperVault[i] = new();
                }
                ItemIO.Send(dataPlayer.SuperVault[i], p, true, true);
            }
        }

        public override void Read(BinaryReader r)
        {
            whoAmI = r.ReadByte();
            for (int i = 0; i < 100; i++)
            {
                items[i] = ItemIO.Receive(r, true, true);
            }
        }

        public override void Receive()
        {
            if (!DataPlayer.TryGet(Main.player[whoAmI], out var modPlayer))
                return;

            for (int i = 0; i < 100; i++)
            {
                modPlayer.SuperVault[i] = items[i];
            }
        }
    }
}
