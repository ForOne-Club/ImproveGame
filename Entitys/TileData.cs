namespace ImproveGame.Entitys
{
    public class TileData
    {
        public enum TileSort { None, Solid, Platform, Torch, Chair, Table, WorkBenche, Bed, Wall, NoWall }
        public TileSort tileSort;
        public int x;
        public int y;

        public TileData(TileSort tileSort, int x, int y)
        {
            this.tileSort = tileSort;
            this.x = x;
            this.y = y;
        }
    }
}
