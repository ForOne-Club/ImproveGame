using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    public class FileOperator : ILoadable
    {
        internal static Dictionary<string, TagCompound> CachedStructureDatas = new();
        internal static string SavePath => Path.Combine(ModLoader.ModPath, "ImproveGame");
        internal static string Extension => ".qolstruct";

        public static void SaveAsFile(Rectangle rectInWorld)
        {
            TrUtils.TryCreatingDirectory(SavePath);

            string name = $"QoLStructure_v{ImproveGame.Instance.Version}.qolstruct";
            string thisPath = Path.Combine(SavePath, name);
            if (File.Exists(thisPath))
            {
                for (int i = 2; i <= 999; i++)
                {
                    name = $"QoLStructure_v{ImproveGame.Instance.Version} ({i}){Extension}";
                    thisPath = Path.Combine(SavePath, name);
                    if (!File.Exists(thisPath))
                    {
                        break;
                    }
                }
            }

            Main.NewText("Structure saved as " + thisPath, Color.Yellow);
            FileStream stream = File.Create(thisPath);
            stream.Close();

            var tag = SaveStructure(rectInWorld);
            TagIO.ToFile(tag, thisPath);

            CachedStructureDatas.Clear();
            if (StructureGUI.Visible && UISystem.Instance.StructureGUI is not null)
            {
                UISystem.Instance.StructureGUI.CacheSetupStructures = true;
            }
        }

        public static TagCompound SaveStructure(Rectangle rectInWorld)
        {
            // 这里不能合并，合并就坏了
            TagCompound tag = new();
            tag.Add("ModVersion", ImproveGame.Instance.Version.ToString());
            tag.Add("Width", rectInWorld.Width);
            tag.Add("Height", rectInWorld.Height);
            tag.Add("OriginX", 0);
            tag.Add("OriginY", 0);

            List<TileDefinition> data = new();
            for (int x = rectInWorld.X; x <= rectInWorld.X + rectInWorld.Width; x++)
            {
                for (int y = rectInWorld.Y; y <= rectInWorld.Y + rectInWorld.Height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    string tileName = tile.TileType.ToString();
                    string wallName = tile.WallType.ToString();
                    if (tile.TileType >= TileID.Count)
                    {
                        var modTile = ModContent.GetModTile(tile.TileType);
                        tileName = $"{modTile.Mod.Name}/{modTile.Name}";
                    }
                    if (tile.WallType >= WallID.Count)
                    {
                        var modWall = ModContent.GetModWall(tile.WallType);
                        wallName = $"{modWall.Mod.Name}/{modWall.Name}";
                    }
                    if (!tile.HasTile)
                    {
                        tileName = "-1";
                    }

                    byte platformDrawSlopeType = 0;
                    #region 平台斜坡绘制信息 由于懒得在绘制时获取周围信息，就这么搞
                    if (TileID.Sets.Platforms[tile.TileType])
                    {
                        if (tile.Slope == SlopeType.SlopeDownLeft && Main.tile[x + 1, y + 1].HasTile && Main.tileSolid[Main.tile[x + 1, y + 1].TileType] && Main.tile[x + 1, y + 1].Slope is not SlopeType.SlopeDownRight && Main.tile[x + 1, y + 1].BlockType is not BlockType.HalfBlock && (!Main.tile[x, y + 1].HasTile || (Main.tile[x, y + 1].BlockType != BlockType.Solid && Main.tile[x, y + 1].BlockType != BlockType.SlopeUpRight) || (!TileID.Sets.BlocksStairs[Main.tile[x, y + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[x, y + 1].TileType])))
                        {
                            if (TileID.Sets.Platforms[Main.tile[x + 1, y + 1].TileType] && Main.tile[x + 1, y + 1].Slope == SlopeType.Solid)
                                platformDrawSlopeType = 2;
                            else
                                platformDrawSlopeType = 1;
                        }
                        else if (tile.Slope == SlopeType.SlopeDownRight && Main.tile[x - 1, y + 1].HasTile && Main.tileSolid[Main.tile[x - 1, y + 1].TileType] && Main.tile[x - 1, y + 1].Slope is not SlopeType.SlopeDownLeft && Main.tile[x - 1, y + 1].BlockType is not BlockType.HalfBlock && (!Main.tile[x, y + 1].HasTile || (Main.tile[x, y + 1].BlockType != BlockType.Solid && Main.tile[x, y + 1].BlockType != BlockType.SlopeUpLeft) || (!TileID.Sets.BlocksStairs[Main.tile[x, y + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[x, y + 1].TileType])))
                        {
                            if (TileID.Sets.Platforms[Main.tile[x - 1, y + 1].TileType] && Main.tile[x - 1, y + 1].Slope == SlopeType.Solid)
                                platformDrawSlopeType = 4;
                            else
                                platformDrawSlopeType = 3;
                        }
                    }
                    # endregion

                    data.Add(
                        new TileDefinition(
                            tileName,
                            tile.BlockType,
                            wallName,
                            tile.TileFrameX,
                            tile.TileFrameY,
                            tile.WallFrameX,
                            tile.WallFrameY,
                            platformDrawSlopeType
                        ));
                }
            }

            tag.Add("StructureData", data);
            return tag;
        }

        public static bool LoadFile(string path)
        {
            TagCompound tag = TagIO.FromFile(path);

            if (tag is null)
            {
                // 此处应有Logger.Warn
                return false;
            }

            CachedStructureDatas.Add(path, tag);
            return true;
        }

        public static TagCompound GetTagFromFile(string path)
        {
            if (!CachedStructureDatas.TryGetValue(path, out TagCompound tag))
            {
                if (!LoadFile(path))
                    return null;

                tag = CachedStructureDatas[path];
            }

            return tag;
        }

        public static int ParseTileType(string tileTypeSerialized)
        {
            if (!int.TryParse(tileTypeSerialized, out int tileType))
            {
                string[] parts = tileTypeSerialized.Split('/');

                if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind(parts[1], out ModTile modTileType))
                    tileType = modTileType.Type;

                else tileType = 0;
            }
            return tileType;
        }

        public static int ParseWallType(string wallTypeSerialized)
        {
            if (!int.TryParse(wallTypeSerialized, out int wallType))
            {
                string[] parts = wallTypeSerialized.Split('/');
                if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind(parts[1], out ModWall modWallType))
                    wallType = modWallType.Type;
                else wallType = 0;
            }
            return wallType;
        }

        public void Load(Mod mod) { }

        public void Unload() => CachedStructureDatas = null;
    }
}
