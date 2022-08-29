using ImproveGame.Common.Players;
using ImproveGame.Content.Tiles;
using Terraria.DataStructures;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    /// <summary>
    /// 客户端发送，问服务器可不可以开箱
    /// </summary>
    public class OpenRequestPacket : NetModule
    {
        private Point16 position;

        public static OpenRequestPacket Get(Point16 position)
        {
            var module = NetModuleLoader.Get<OpenRequestPacket>();
            module.position = position;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
        }

        public override void Receive()
        {
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                var player = Main.player[k];
                if (player.active && !player.dead && player.TryGetModPlayer<AutofishPlayer>(out var modPlayer) && modPlayer.Autofisher == position)
                {
                    return;
                }
            }

            if (!TryGetTileEntityAs<TEAutofisher>(position, out var autofisher))
                return;

            // 没玩家，发开箱包
            var packets = AggregateModule.Get(
                new() {
                    FisherAllFiltersPacket.Get(
                        position,
                        autofisher.CatchCrates,
                        autofisher.CatchAccessories,
                        autofisher.CatchTools,
                        autofisher.CatchWhiteRarityCatches,
                        autofisher.CatchNormalCatches
                    ),
                    LocatePointPacket.Get(
                        position,
                        autofisher.locatePoint
                    ),
                    ItemsSyncAllPacket.Get(
                        position,
                        true,
                        autofisher.fishingPole,
                        autofisher.bait,
                        autofisher.accessory,
                        autofisher.fish
                    )
                }
            );
            // 服务器设置好
            Main.player[Sender].GetModPlayer<AutofishPlayer>().SetAutofisher(position, false);
            packets.Send(Mod, Sender, -1, false);
        }
    }

    /// <summary>
    /// 服务器发送，让其他玩家知道有个玩家开箱了
    /// </summary>
    public class SyncOpenPacket : NetModule
    {
        private Point16 position;
        private byte whoAmI;

        public static SyncOpenPacket Get(Point16 position, int whoAmI)
        {
            var module = NetModuleLoader.Get<SyncOpenPacket>();
            module.position = position;
            module.whoAmI = (byte)whoAmI;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(whoAmI);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            whoAmI = r.ReadByte();
        }

        public override void Receive()
        {
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                if (AutofishPlayer.TryGet(Main.player[whoAmI], out var modPlayer))
                {
                    modPlayer.SetAutofisher(new(position.X, position.Y), false);
                }
            }
            else
            {
                // 服务端的设置好
                Main.player[Sender].GetModPlayer<AutofishPlayer>().SetAutofisher(new(position.X, position.Y), false);
                // 给其他玩家发开关包
                Send(-1, Sender, false);
            }
        }
    }
}
