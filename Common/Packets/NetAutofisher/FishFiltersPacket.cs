using ImproveGame.Content.Tiles;
using Terraria.DataStructures;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    public class FishFiltersPacket : NetModule
    {
        private Point16 position;
        private bool toggle;
        private byte filterType;

        public static FishFiltersPacket Get(Point16 position, bool toggle, byte filterType)
        {
            var module = NetModuleLoader.Get<FishFiltersPacket>();
            module.position = position;
            module.filterType = filterType;
            module.toggle = toggle;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(toggle);
            p.Write(filterType);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            toggle = r.ReadBoolean();
            filterType = r.ReadByte();
        }

        public override void Receive()
        {
            if (!TryGetTileEntityAs<TEAutofisher>(position.X, position.Y, out var autofisher))
                return;

            switch (filterType)
            {
                case 0: autofisher.CatchCrates = toggle; break;
                case 1: autofisher.CatchAccessories = toggle; break;
                case 2: autofisher.CatchTools = toggle; break;
                case 3: autofisher.CatchWhiteRarityCatches = toggle; break;
                case 4: autofisher.CatchNormalCatches = toggle; break;
            }
        }
    }

    /// <summary>
    /// 全部过滤器包
    /// </summary>
    public class FisherAllFiltersPacket : NetModule
    {
        private Point16 position;
        private bool catchCrates;
        private bool catchAccessories;
        private bool catchTools;
        private bool catchWhiteRarityCatches;
        private bool catchNormalCatches;

        public static FisherAllFiltersPacket Get(Point16 position, bool catchCrates, bool catchAccessories, bool catchTools, bool catchWhiteRarityCatches, bool catchNormalCatches)
        {
            var module = NetModuleLoader.Get<FisherAllFiltersPacket>();
            module.position = position;
            module.catchCrates = catchCrates;
            module.catchAccessories = catchAccessories;
            module.catchTools = catchTools;
            module.catchWhiteRarityCatches = catchWhiteRarityCatches;
            module.catchNormalCatches = catchNormalCatches;
            return module;
        }

        public override void Send(ModPacket p)
        {
            p.Write(position);
            p.Write(catchCrates);
            p.Write(catchAccessories);
            p.Write(catchTools);
            p.Write(catchWhiteRarityCatches);
            p.Write(catchNormalCatches);
        }

        public override void Read(BinaryReader r)
        {
            position = r.ReadPoint16();
            catchCrates = r.ReadBoolean();
            catchAccessories = r.ReadBoolean();
            catchTools = r.ReadBoolean();
            catchWhiteRarityCatches = r.ReadBoolean();
            catchNormalCatches = r.ReadBoolean();
        }

        public override void Receive()
        {
            if (TileLoader.GetTile(Main.tile[position.ToPoint()].TileType) is Autofisher && TryGetTileEntityAs<TEAutofisher>(position.X, position.Y, out var autofisher))
            {
                autofisher.CatchCrates = catchCrates;
                autofisher.CatchAccessories = catchAccessories;
                autofisher.CatchTools = catchTools;
                autofisher.CatchWhiteRarityCatches = catchWhiteRarityCatches;
                autofisher.CatchNormalCatches = catchNormalCatches;
            }
        }
    }
}
