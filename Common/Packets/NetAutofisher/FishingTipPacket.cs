using ImproveGame.Content.Tiles;
using Terraria.DataStructures;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    /// <summary>
    /// 由服务器执行，向所有玩家同步钓鱼信息提示
    /// </summary>
    public class FishingTipPacket : NetModule
    {
        private Point16 position;
        private string text;

        public static FishingTipPacket Get(Point16 position, string text)
        {
            var module = NetModuleLoader.Get<FishingTipPacket>();
            module.position = position;
            module.text = text;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(text);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            text = r.ReadString();
        }

        public override void Receive()
        {
            if (TryGetTileEntityAs<TEAutofisher>(position.X, position.Y, out var autofisher))
            {
                autofisher.SetFishingTip(text);
            }
        }
    }
}
