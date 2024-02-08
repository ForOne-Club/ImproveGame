using ImproveGame.Common.ModPlayers;

namespace ImproveGame.Packets.Items
{
    public class InfBuffItemPacket : NetModule
    {
        [AutoSync] private byte whoAmI;
        private List<Item> items = new();

        public static InfBuffItemPacket Get(InfBuffPlayer modPlayer)
        {
            var module = NetModuleLoader.Get<InfBuffItemPacket>();
            module.whoAmI = (byte)modPlayer.Player.whoAmI;
            module.items = modPlayer.AvailableItems;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write((ushort) items.Count);
            for (var i = 0; i < items.Count; i++)
            {
                // 为了减少传输包大小，只传输这俩
                p.Write((ushort) items[i].stack);
                p.Write((ushort) items[i].type);
            }
        }

        public override void Read(BinaryReader r)
        {
            int listCount = r.ReadUInt16();
            items = new List<Item>();
            for (var i = 0; i < listCount; i++)
            {
                int stack = r.ReadUInt16();
                int type = r.ReadUInt16();
                items.Add(new Item(type, stack));
            }
        }

        public override void Receive()
        {
            if (!InfBuffPlayer.TryGet(Main.player[whoAmI], out var modPlayer))
                return;

            modPlayer.AvailableItems = items;

            // 转发
            if (Main.netMode is NetmodeID.Server)
            {
                var packet = Get(modPlayer);
                packet.Send(-1, whoAmI, false);
            }
        }
    }
}