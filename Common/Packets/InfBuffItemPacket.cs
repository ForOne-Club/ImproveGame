using ImproveGame.Common.Players;

namespace ImproveGame.Common.Packets
{
    public class InfBuffItemPacket : NetModule
    {
        private byte whoAmI;
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
            p.Write(whoAmI);
            p.Write(items.Count);
            for (var i = 0; i < items.Count; i++) {
                // 为了减少传输包大小，只传输这俩
                p.Write(items[i].stack);
                p.Write(items[i].type);
            }
        }

        public override void Read(BinaryReader r)
        {
            whoAmI = r.ReadByte();
            int listCount = r.ReadInt32();
            items = new();
            for (var i = 0; i < listCount; i++)
            {
                int stack = r.ReadInt32();
                int type = r.ReadInt32();
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
