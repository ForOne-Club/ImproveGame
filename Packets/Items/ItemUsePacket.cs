namespace ImproveGame.Packets.Items
{
    [AutoSync]
    public class ItemUsePacket : NetModule
    {
        private byte whoAmI;
        private float itemRotation;
        private bool directionBool;
        private short itemAnimation;

        public static ItemUsePacket Get(Player player) => Get(player.whoAmI, player.itemRotation, player.direction,
            (short)player.itemAnimation);

        public static ItemUsePacket Get(int whoAmI, float itemRotation, int direction, short itemAnimation)
        {
            var module = NetModuleLoader.Get<ItemUsePacket>();
            module.whoAmI = (byte)whoAmI;
            module.itemRotation = itemRotation;
            module.directionBool = direction is 1;
            module.itemAnimation = itemAnimation;
            return module;
        }

        public override void Receive()
        {
            Main.player[whoAmI].itemRotation = itemRotation;
            Main.player[whoAmI].direction = directionBool ? 1 : -1;
            Main.player[whoAmI].itemAnimation = itemAnimation;

            if (Main.netMode is NetmodeID.Server)
            {
                Send(-1, whoAmI, runLocally: false);
            }
        }
    }
}