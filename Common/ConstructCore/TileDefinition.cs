using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    internal class TileDefinition : TagSerializable
    {
        public string Tile;
        public BlockType BlockType;
        public string Wall;
        public short TileFrameX;
        public short TileFrameY;
        public int WallFrameX;
        public int WallFrameY;

        public byte PlatformSlopeDrawType;

        public TileDefinition(string tileName, BlockType blockType, string wallName, short frameX, short frameY, int wallFrameX, int wallFrameY, byte platformSlopeType = 0)
        {
            Tile = tileName;
            BlockType = blockType;
            Wall = wallName;
            TileFrameX = frameX;
            TileFrameY = frameY;
            WallFrameX = wallFrameX;
            WallFrameY = wallFrameY;
            PlatformSlopeDrawType = platformSlopeType;
        }

        // tML反射获取叫这个的Field，获取不到就报错，不能删啊
        public static Func<TagCompound, TileDefinition> DESERIALIZER = s => DeserializeData(s);

        public static TileDefinition DeserializeData(TagCompound tag)
        {
            var output = new TileDefinition(
                tag.GetString("Tile"),
                (BlockType)tag.GetByte("BlockType"),
                tag.GetString("Wall"),
                tag.GetShort("TileFrameX"),
                tag.GetShort("TileFrameY"),
                tag.GetInt("WallFrameX"),
                tag.GetInt("WallFrameY")
            );
            if (tag.ContainsKey("PlatformSlopeDrawType"))
            {
                output.PlatformSlopeDrawType = tag.GetByte("PlatformSlopeDrawType");
            }
            return output;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound()
            {
                ["Tile"] = Tile,
                ["BlockType"] = (byte)BlockType,
                ["Wall"] = Wall,
                ["TileFrameX"] = TileFrameX,
                ["TileFrameY"] = TileFrameY,
                ["WallFrameX"] = WallFrameX,
                ["WallFrameY"] = WallFrameY
            };

            if (PlatformSlopeDrawType is not 0)
            {
                tag.Add("PlatformSlopeDrawType", PlatformSlopeDrawType);
            }

            return tag;
        }
    }
}
