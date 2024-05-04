using System.Diagnostics;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.Construction
{
    public class QoLStructure
    {
        public TagCompound Tag;

        public Dictionary<string, ushort> entries = new();
        public Dictionary<ushort, ushort> typeMaping = new();

        public string BuildTime;
        public string ModVersion;
        public short Width;
        public short Height;
        public short OriginX;
        public short OriginY;
        public List<TileDefinition> StructureDatas;
        public List<string> SignTexts;

        public int GetOrAddEntry(string fullName)
        {
            if (entries.TryGetValue(fullName, out ushort entry))
            {
                return entry;
            }
            entries.Add(fullName, (ushort)entries.Count);
            return entries.Count - 1;
        }

        internal QoLStructure(string pathName)
        {
            Tag = FileOperator.GetTagFromFile(pathName);
            if (Tag is null)
                return;
            Setup();
        }

        internal QoLStructure(TagCompound tag)
        {
            Tag = tag;
            Setup();
        }

        internal QoLStructure(Rectangle rectInWorld)
        {
            Tag = new();
            Tag.Add(nameof(BuildTime), DateTime.Now.ToString("s"));
            Tag.Add(nameof(ModVersion), ImproveGame.Instance.Version.ToString());
            Tag.Add(nameof(Width), (short)rectInWorld.Width);
            Tag.Add(nameof(Height), (short)rectInWorld.Height);
            Tag.Add(nameof(OriginX), (short)0);
            Tag.Add(nameof(OriginY), (short)0);

            List<TileDefinition> data = new();
            List<string> signTexts = new();
            for (int x = rectInWorld.X; x <= rectInWorld.X + rectInWorld.Width; x++)
            {
                for (int y = rectInWorld.Y; y <= rectInWorld.Y + rectInWorld.Height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    short tileIndex = (short)tile.TileType;
                    short wallIndex = (short)tile.WallType;
                    if (tile.TileType >= TileID.Count)
                    {
                        var modTile = ModContent.GetModTile(tile.TileType);
                        tileIndex = (short)GetOrAddEntry($"{modTile.FullName}t");
                    }
                    if (tile.WallType >= WallID.Count)
                    {
                        var modWall = ModContent.GetModWall(tile.WallType);
                        wallIndex = (short)GetOrAddEntry($"{modWall.FullName}w");
                    }
                    if (!tile.HasTile)
                    {
                        tileIndex = -1;
                    }
                    if (tile.WallType is 0)
                    {
                        wallIndex = -1; // 统一一点
                    }

                    if (Main.tileSign[tile.TileType] && (tile.TileFrameX / 18) % 2 is 0 && tile.TileFrameY / 18 is 0)
                    {
                        int sign = Sign.ReadSign(x, y);
                        if (sign != -1)
                            signTexts.Add(Main.sign[sign].text);
                    }

                    byte platformDrawSlopeType = 4;
                    #region 平台斜坡绘制信息 由于懒得在绘制时获取周围信息，就这么搞
                    if (TileID.Sets.Platforms[tile.TileType])
                    {
                        if (tile.Slope == SlopeType.SlopeDownLeft && Main.tile[x + 1, y + 1].HasTile && Main.tileSolid[Main.tile[x + 1, y + 1].TileType] && Main.tile[x + 1, y + 1].Slope is not SlopeType.SlopeDownRight && Main.tile[x + 1, y + 1].BlockType is not BlockType.HalfBlock && (!Main.tile[x, y + 1].HasTile || (Main.tile[x, y + 1].BlockType != BlockType.Solid && Main.tile[x, y + 1].BlockType != BlockType.SlopeUpRight) || (!TileID.Sets.BlocksStairs[Main.tile[x, y + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[x, y + 1].TileType])))
                        {
                            if (TileID.Sets.Platforms[Main.tile[x + 1, y + 1].TileType] && Main.tile[x + 1, y + 1].Slope == SlopeType.Solid)
                                platformDrawSlopeType = 1;
                            else
                                platformDrawSlopeType = 0;
                        }
                        else if (tile.Slope == SlopeType.SlopeDownRight && Main.tile[x - 1, y + 1].HasTile && Main.tileSolid[Main.tile[x - 1, y + 1].TileType] && Main.tile[x - 1, y + 1].Slope is not SlopeType.SlopeDownLeft && Main.tile[x - 1, y + 1].BlockType is not BlockType.HalfBlock && (!Main.tile[x, y + 1].HasTile || (Main.tile[x, y + 1].BlockType != BlockType.Solid && Main.tile[x, y + 1].BlockType != BlockType.SlopeUpLeft) || (!TileID.Sets.BlocksStairs[Main.tile[x, y + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[x, y + 1].TileType])))
                        {
                            if (TileID.Sets.Platforms[Main.tile[x - 1, y + 1].TileType] && Main.tile[x - 1, y + 1].Slope == SlopeType.Solid)
                                platformDrawSlopeType = 3;
                            else
                                platformDrawSlopeType = 2;
                        }
                    }
                    # endregion
                    var extraDatas = TileDefinition.GetExtraData(tile);
                    var extraDatas2 = TileDefinition.GetExtraData2(tile, platformDrawSlopeType);

                    data.Add(
                        new TileDefinition(
                            tileIndex,
                            wallIndex,
                            tile,
                            extraDatas,
                            extraDatas2
                        ));
                }
            }

            Tag.Add("SignTexts", signTexts);
            Tag.Add("StructureData", data);

            var stringList = new List<string>();
            var indexList = new List<ushort>();
            foreach (var (name, type) in entries)
            {
                stringList.Add(name);
                indexList.Add(type);
            }
            Tag.Add("EntriesName", stringList);
            Tag.Add("EntriesType", indexList);
        }

        private void Setup()
        {
            BuildTime = Tag.GetString(nameof(BuildTime));
            ModVersion = Tag.GetString(nameof(ModVersion));
            Width = Tag.GetShort(nameof(Width));
            Height = Tag.GetShort(nameof(Height));
            OriginX = Tag.GetShort(nameof(OriginX));
            OriginY = Tag.GetShort(nameof(OriginY));
            SignTexts = (List<string>)Tag.GetList<string>("SignTexts") ?? new List<string>();
            StructureDatas = (List<TileDefinition>)Tag.GetList<TileDefinition>("StructureData");
            SetupEntry();
        }

        private void SetupEntry()
        {
            var names = Tag.GetList<string>("EntriesName");
            var types = Tag.GetList<ushort>("EntriesType");
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                ushort type = types[i];
                entries.Add(name, type);

                if (name.EndsWith("t")) // Tile
                {
                    if (ModContent.TryFind<ModTile>(name[..^1], out var tile))
                    {
                        typeMaping.Add(type, tile.Type);
                    }
                }
                else if (name.EndsWith("w")) // Wall
                {
                    if (ModContent.TryFind<ModWall>(name[..^1], out var wall))
                    {
                        typeMaping.Add(type, wall.Type);
                    }
                }
                else
                {
                    Debug.Fail("Failed to find a modblock");
                }
            }
        }

        public static void SetValue(TagCompound tag, string filePath)
        {
            TagIO.ToFile(tag, filePath);
            FileOperator.CachedStructureDatas.Remove(filePath);
        }

        public int ParseTileType(TileDefinition tileData)
        {
            if (tileData.VanillaTile)
                return tileData.TileIndex;
            return typeMaping.TryGetValue((ushort)tileData.TileIndex, out var value) ? value : -1;
        }

        public int ParseWallType(TileDefinition tileData)
        {
            if (tileData.VanillaWall)
                return tileData.WallIndex;
            return typeMaping.TryGetValue((ushort)tileData.WallIndex, out var value) ? value : -1;
        }
    }
}
