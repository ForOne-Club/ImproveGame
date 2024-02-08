using ImproveGame.Common.ModPlayers;

namespace ImproveGame.Packets
{
    [AutoSync]
    public class BigBagSlotPacket : NetModule
    {
        private byte whoAmI;
        private byte slot;
        [ItemSync(syncFavorite: true)] private Item item;

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

    [AutoSync]
    public class BigBagAllSlotsPacket : NetModule
    {
        private byte whoAmI;
        [ItemSync(syncFavorite: true)] private Item[] items;

        public static BigBagAllSlotsPacket Get(DataPlayer dataPlayer)
        {
            var module = NetModuleLoader.Get<BigBagAllSlotsPacket>();
            module.whoAmI = (byte)dataPlayer.Player.whoAmI;
            module.items = dataPlayer.SuperVault;
            return module;
        }

        public override void Receive()
        {
            if (items is null || items.Length != 100 || !DataPlayer.TryGet(Main.player[whoAmI], out var modPlayer))
                return;

            for (int i = 0; i < modPlayer.SuperVault.Length && i < items.Length; i++)
            {
                modPlayer.SuperVault[i] = items[i];
            }
        }
    }
}