using ImproveGame.Content.Tiles;
using Terraria.DataStructures;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    [AutoSync]
    public class LocatePointPacket : NetModule
    {
        private Point16 position;
        private Point16 locatePoint;

        public static LocatePointPacket Get(Point16 position, Point16 locatePoint)
        {
            var module = NetModuleLoader.Get<LocatePointPacket>();
            module.position = position;
            module.locatePoint = locatePoint;
            return module;
        }

        public override void Receive()
        {
            if (TryGetTileEntityAs<TEAutofisher>(position.X, position.Y, out var autofisher))
            {
                autofisher.locatePoint = new(locatePoint.X, locatePoint.Y);
            }
            // 没有东西，传过去告诉他这TE没了
            else if (Main.netMode is NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, position.X, position.Y, 2, 2, TileChangeType.None);
            }
        }
    }
}