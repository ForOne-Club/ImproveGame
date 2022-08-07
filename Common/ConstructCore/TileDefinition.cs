using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    public class TileDefinition : TagSerializable
    {
        public short TileIndex;
        public short WallIndex;
        public short TileFrameX;
        public short TileFrameY;
        public short WallFrameX;
        public short WallFrameY;

        /// <summary>
        /// ExtraDatas:
        /// <br>0, 1分别为tileVanilla, wallVanilla</br>
        /// <br>2,3均false: platformType为0;</br>
        /// <br>2为true: platformType为1;</br>
        /// <br>3为true: platformType为2;</br>
        /// <br>2,3均为true: platformType为3</br>
        /// <br>4为true: 为Solid（则后面的不执行）</br>
        /// <br>5为true: 为HalfBlock（则后面的不执行）</br>
        /// <br>6为true: Slope为上, 为false: Slope为下</br>
        /// <br>7为true: Slope为左, 为false: Slope为右</br>
        /// </summary>
        public BitsByte ExtraDatas;

        public bool VanillaTile => ExtraDatas[0];
        public bool VanillaWall => ExtraDatas[1];
        public BlockType BlockType
        {
            get
            {
                if (ExtraDatas[4])
                    return BlockType.Solid;
                if (ExtraDatas[5])
                    return BlockType.HalfBlock;
                bool up = ExtraDatas[6];
                bool left = ExtraDatas[7];
                if (!left && !up) // 右下
                    return BlockType.SlopeDownRight;
                if (!left && up) // 右上
                    return BlockType.SlopeUpRight;
                if (left && !up) // 左下
                    return BlockType.SlopeDownLeft;
                if (left && up) // 左上
                    return BlockType.SlopeUpLeft;
                return BlockType.Solid; // 这个是不可能达到的
            }
        }

        public TileDefinition(short tileIndex, short wallIndex, short frameX, short frameY, short wallFrameX, short wallFrameY, BitsByte extraDatas)
        {
            TileIndex = tileIndex;
            WallIndex = wallIndex;
            TileFrameX = frameX;
            TileFrameY = frameY;
            WallFrameX = wallFrameX;
            WallFrameY = wallFrameY;
            ExtraDatas = extraDatas;
        }

        public static BitsByte GetExtraData(bool tileVanilla, bool wallVanilla, byte platformType, BlockType blockType)
        {
            BitsByte data = new();
            data[0] = tileVanilla;
            data[1] = wallVanilla;
            switch (platformType)
            {
                case 1:
                    data[2] = true;
                    break;
                case 2:
                    data[3] = true;
                    break;
                case 3:
                    data[2] = true;
                    data[3] = true;
                    break;
            }
            switch (blockType)
            {
                case BlockType.Solid:
                    data[4] = true;
                    break;
                case BlockType.HalfBlock:
                    data[5] = true;
                    break;
            }
            if (blockType is BlockType.SlopeUpLeft or BlockType.SlopeUpRight)
                data[6] = true;
            if (blockType is BlockType.SlopeUpLeft or BlockType.SlopeDownLeft)
                data[7] = true;
            return data;
        }

        public int GetPlatformDrawType()
        {
            if (!ExtraDatas[2] && !ExtraDatas[3])
                return 0;
            if (ExtraDatas[2] && !ExtraDatas[3])
                return 1;
            if (!ExtraDatas[2] && ExtraDatas[3])
                return 2;
            if (ExtraDatas[2] && ExtraDatas[3])
                return 3;
            return 0; // 这个是不可能达到的
        }

        // tML反射获取叫这个的Field，获取不到就报错，不能删啊
        public static Func<TagCompound, TileDefinition> DESERIALIZER = s => DeserializeData(s);

        public static TileDefinition DeserializeData(TagCompound tag)
        {
            var output = new TileDefinition(
                tag.GetShort("TileIndex"),
                tag.GetShort("WallIndex"),
                tag.GetShort("TileFrameX"),
                tag.GetShort("TileFrameY"),
                tag.GetShort("WallFrameX"),
                tag.GetShort("WallFrameY"),
                (BitsByte)tag.GetByte("ExtraDatas")
            );
            return output;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound()
            {
                ["TileIndex"] = TileIndex,
                ["WallIndex"] = WallIndex,
                ["TileFrameX"] = TileFrameX,
                ["TileFrameY"] = TileFrameY,
                ["WallFrameX"] = WallFrameX,
                ["WallFrameY"] = WallFrameY,
                ["ExtraDatas"] = (byte)ExtraDatas
            };

            return tag;
        }
    }
}
