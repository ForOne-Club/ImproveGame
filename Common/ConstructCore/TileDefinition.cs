using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    internal class TileDefinition : TagSerializable
    {
        public string Tile;
        public string Wall;
        public short TileFrameX;
        public short TileFrameY;
        public int WallFrameX;
        public int WallFrameY;

        public TileDefinition(string tileName, string wallName, short frameX, short frameY, int wallFrameX, int wallFrameY)
        {
            Tile = tileName;
            Wall = wallName;
            TileFrameX = frameX;
            TileFrameY = frameY;
            WallFrameX = wallFrameX;
            WallFrameY = wallFrameY;
        }

        // tML反射获取叫这个的Field，获取不到就报错，不能删啊
        public static Func<TagCompound, TileDefinition> DESERIALIZER = s => DeserializeData(s);

        public static TileDefinition DeserializeData(TagCompound tag)
        {
            var output = new TileDefinition(
                tag.GetString("Tile"),
                tag.GetString("Wall"),
                tag.GetShort("TileFrameX"),
                tag.GetShort("TileFrameY"),
                tag.GetInt("WallFrameX"),
                tag.GetInt("WallFrameY")
            );
            return output;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound()
            {
                ["Tile"] = Tile,
                ["Wall"] = Wall,
                ["TileFrameX"] = TileFrameX,
                ["TileFrameY"] = TileFrameY,
                ["WallFrameX"] = WallFrameX,
                ["WallFrameY"] = WallFrameY
            };

            return tag;
        }
    }
}
