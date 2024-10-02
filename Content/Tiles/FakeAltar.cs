using Terraria.ObjectData;

namespace ImproveGame.Content.Tiles
{
    public class FakeAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            AdjTiles = [TileID.DemonAltar];
            AddMapEntry(Color.Purple, CreateMapEntryName());
            TileObjectData.addTile(Type);
        }
        public override bool Slope(int i, int j) => false;
    }
}
