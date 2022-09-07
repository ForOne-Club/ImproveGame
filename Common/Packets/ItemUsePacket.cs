using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.Common.Packets
{
    public class ItemUsePacket : NetModule
    {
        private byte whoAmI;
        private float itemRotation;
        private int direction;
        private short itemAnimation;

        public static ItemUsePacket Get(Player player) => Get(player.whoAmI, player.itemRotation, player.direction, (short)player.itemAnimation);

        public static ItemUsePacket Get(int whoAmI, float itemRotation, int direction, short itemAnimation)
        {
            var module = NetModuleLoader.Get<ItemUsePacket>();
            module.whoAmI = (byte)whoAmI;
            module.itemRotation = itemRotation;
            module.direction = direction;
            module.itemAnimation = itemAnimation;
            return module;
        }

        public override void Send(ModPacket p)
        {
            bool directionBool = direction == 1;
            p.Write(whoAmI);
            p.Write(itemRotation);
            p.Write(directionBool);
            p.Write(itemAnimation);
        }

        public override void Read(BinaryReader r)
        {
            whoAmI = r.ReadByte();
            itemRotation = r.ReadSingle();
            direction = r.ReadBoolean() ? 1 : -1;
            itemAnimation = r.ReadInt16();
        }

        public override void Receive()
        {
            Main.player[whoAmI].itemRotation = itemRotation;
            Main.player[whoAmI].direction = direction;
            Main.player[whoAmI].itemAnimation = itemAnimation;

            if (Main.netMode is NetmodeID.Server)
            {
                Send(-1, whoAmI, runLocally: false);
            }
        }
    }
}
