using ImproveGame.Content.Tiles;
using NetSimplified.Syncing;
using Terraria.DataStructures;

namespace ImproveGame.Common.Packets.NetAutofisher
{
    [AutoSync]
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
    [AutoSync]
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
