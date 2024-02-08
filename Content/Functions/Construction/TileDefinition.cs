using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.Construction
{
    public class TileDefinition : TagSerializable
    {
        public short TileIndex;
        public short WallIndex;
        public short TileFrameX;
        public short TileFrameY;
        public byte TileColor;
        public short WallFrameX;
        public short WallFrameY;
        public byte WallColor;
        public TileWallBrightnessInvisibilityData CoatingData;

        /// <summary>
        /// ExtraDatas:
        /// <br>0, 1分别为!tileVanilla, !wallVanilla，即true就不是原版, false就是</br>
        /// <br>2为actuated，是否被虚化</br>
        /// <br>3被Extra2取走了，暂无用处</br>
        /// <br>4为true: 为Solid（则后面的不执行）</br>
        /// <br>5为true: 为HalfBlock（则后面的不执行）</br>
        /// <br>6为true: Slope为上, 为false: Slope为下</br>
        /// <br>7为true: Slope为左, 为false: Slope为右</br>
        /// </summary>
        public BitsByte ExtraDatas;

        /// <summary>
        /// ExtraDatas2 (不同于Extra1，这个是必存的):
        /// <br>0, 为是否有特殊platformType, 为false的话后面的就不管了</br>
        /// <br>1,2均false: platformType为0;</br>
        /// <br>1为true: platformType为1;</br>
        /// <br>2为true: platformType为2;</br>
        /// <br>1,2均为true: platformType为3</br>
        /// <br>3,4,5,6分别为红绿蓝黄线</br>
        /// <br>7为是否有促动器</br>
        /// </summary>
        public BitsByte ExtraDatas2;

        public bool RedWire => ExtraDatas2[3];
        public bool GreenWire => ExtraDatas2[4];
        public bool BlueWire => ExtraDatas2[5];
        public bool YellowWire => ExtraDatas2[6];
        public bool HasActuator => ExtraDatas2[7];

        public bool VanillaTile => !ExtraDatas[0];
        public bool VanillaWall => !ExtraDatas[1];
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

        // 存的时候另外做的
        public TileDefinition() { }

        public TileDefinition(short tileIndex, short wallIndex, Tile tile, BitsByte extraDatas, BitsByte extraDatas2)
        {
            TileIndex = tileIndex;
            WallIndex = wallIndex;
            TileFrameX = tile.TileFrameX;
            TileFrameY = tile.TileFrameY;
            TileColor = tile.TileColor;
            WallFrameX = (short)tile.WallFrameX;
            WallFrameY = (short)tile.WallFrameY;
            WallColor = tile.WallColor;
            CoatingData = tile.Get<TileWallBrightnessInvisibilityData>();
            ExtraDatas = extraDatas;
            ExtraDatas2 = extraDatas2;
        }

        public static BitsByte GetExtraData(Tile tile)
        {
            BitsByte data = new();
            data[0] = tile.TileType >= TileID.Count;
            data[1] = tile.WallType >= WallID.Count;
            data[2] = tile.IsActuated;
            var blockType = tile.BlockType;
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

        public static BitsByte GetExtraData2(Tile tile, byte platformType)
        {
            BitsByte data = new();

            switch (platformType)
            {
                case 0:
                    data[0] = true;
                    break;
                case 1:
                    data[0] = true;
                    data[1] = true;
                    break;
                case 2:
                    data[0] = true;
                    data[2] = true;
                    break;
                case 3:
                    data[0] = true;
                    data[1] = true;
                    data[2] = true;
                    break;
            }
            data[3] = tile.RedWire;
            data[4] = tile.GreenWire;
            data[5] = tile.BlueWire;
            data[6] = tile.YellowWire;
            data[7] = tile.HasActuator;
            return data;
        }

        public int GetPlatformDrawType()
        {
            if (!ExtraDatas2[0])
                return -1;
            if (!ExtraDatas2[1] && !ExtraDatas2[2])
                return 0;
            if (ExtraDatas2[1] && !ExtraDatas2[2])
                return 1;
            if (!ExtraDatas2[1] && ExtraDatas2[2])
                return 2;
            if (ExtraDatas2[1] && ExtraDatas2[2])
                return 3;
            return 0; // 这个是不可能达到的
        }

        // tML反射获取叫这个的Field，获取不到就报错，不能删啊
        public static Func<TagCompound, TileDefinition> DESERIALIZER = s => DeserializeData(s);

        public static TileDefinition DeserializeData(TagCompound tag)
        {
            var output = new TileDefinition
            {
                TileIndex = -1,
                WallIndex = -1
            };
            if (tag.TryGet("TileIndex", out short tileIndex))
            {
                output.TileIndex = tileIndex;
                output.TileFrameX = tag.GetShort("TileFrameX");
                output.TileFrameY = tag.GetShort("TileFrameY");
                tag.TryGet("TileColor", out output.TileColor);
            }
            if (tag.TryGet("WallIndex", out short wallIndex))
            {
                output.WallIndex = wallIndex;
                output.WallFrameX = tag.GetShort("WallFrameX");
                output.WallFrameY = tag.GetShort("WallFrameY");
                tag.TryGet("WallColor", out output.WallColor);
            }
            if (tag.TryGet("CoatingData", out byte coatingData))
            {
                output.CoatingData = new TileWallBrightnessInvisibilityData
                {
                    bitpack = coatingData
                };
            }
            if (output.TileIndex is not -1 || output.WallIndex is not -1)
            {
                output.ExtraDatas = tag.GetByte("ExtraDatas");
            }
            output.ExtraDatas2 = tag.GetByte("ExtraDatas2");
            return output;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound();
            if (TileIndex is not -1)
            {
                tag["TileIndex"] = TileIndex;
                tag["TileFrameX"] = TileFrameX;
                tag["TileFrameY"] = TileFrameY;
                if (TileColor is not 0)
                    tag["TileColor"] = TileColor;
            }
            if (WallIndex is not -1)
            {
                tag["WallIndex"] = WallIndex;
                tag["WallFrameX"] = WallFrameX;
                tag["WallFrameY"] = WallFrameY;
                if (WallColor is not 0)
                    tag["WallColor"] = WallColor;
            }
            tag["CoatingData"] = CoatingData.Data;
            if (TileIndex is not -1 || WallIndex is not -1)
            {
                tag["ExtraDatas"] = (byte)ExtraDatas;
            }
            tag["ExtraDatas2"] = (byte)ExtraDatas2;

            return tag;
        }
    }
}
