using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ImproveGame.Common.ConstructCore
{
    public class FileOperator
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
        }

        public static TagCompound SaveStructure(Rectangle rectInWorld)
        {
            TagCompound tag = new()
            {
                { "ModVersion", ImproveGame.Instance.Version.ToString() },
                { "Width", rectInWorld.Width },
                { "Height", rectInWorld.Height }
            };

            List<TileDefinition> data = new();
            for (int x = rectInWorld.X; x <= rectInWorld.X + rectInWorld.Width; x++)
            {
                for (int y = rectInWorld.Y; y <= rectInWorld.Y + rectInWorld.Height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    // 暂时先不保存大于16的
                    TileObjectData tileData = TileObjectData.GetTileData(tile.TileType, 0, 0);
                    if (tile.HasTile && tileData is not null && (tileData.CoordinateFullHeight > 16 || tileData.CoordinateFullWidth > 16))
                        continue;

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

                    data.Add(
                        new TileDefinition(
                            tileName,
                            wallName,
                            tile.TileFrameX,
                            tile.TileFrameY,
                            tile.WallFrameX,
                            tile.WallFrameY
                            ));
                }
            }

            tag.Add("StructureData", data);
            return tag;
        }

        public static bool DrawPreviewFromTag(SpriteBatch sb, TagCompound tag, Point leftTop, float scale = 1f)
        {
            List<TileDefinition> data = (List<TileDefinition>)tag.GetList<TileDefinition>("StructureData");

            if (data is null)
            {
                // 此处应有Logger.Warn
                return false;
            }

            Color color = Color.White;
            int width = tag.GetInt("Width");
            int height = tag.GetInt("Height");

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);

                    TileDefinition tileData = data[index];

                    if (!int.TryParse(tileData.Tile, out int tileType))
                    {
                        string[] parts = tileData.Tile.Split('/');

                        if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind(parts[1], out ModTile modTileType))
                            tileType = modTileType.Type;

                        else tileType = 0;
                    }

                    if (!int.TryParse(tileData.Wall, out int wallType))
                    {
                        string[] parts = tileData.Wall.Split('/');
                        if (parts.Length > 1 && ModLoader.GetMod(parts[0]) != null && ModLoader.GetMod(parts[0]).TryFind(parts[1], out ModWall modWallType))
                            wallType = modWallType.Type;

                        else wallType = 0;
                    }

                    var position = (leftTop + new Point(x, y)).ToWorldCoordinates(Vector2.Zero);

                    if (wallType > 0) // Wall
                    {
                        Main.instance.LoadWall(wallType);
                        Texture2D textureWall = TextureAssets.Wall[wallType].Value;

                        int wallFrame = Main.wallFrame[wallType] * 180;
                        Rectangle value = new(tileData.WallFrameX, tileData.WallFrameY + wallFrame, 32, 32);
                        Vector2 pos = position + new Vector2(x * 16 - 8, y * 16 - 8);
                        sb.Draw(textureWall, pos * scale, value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                    if (tileType > 0) // Tile
                    {
                        Main.instance.LoadTiles(tileType);
                        Texture2D texture = TextureAssets.Tile[tileType].Value;
                        Rectangle? value = new Rectangle(tileData.TileFrameX, tileData.TileFrameY, 16, 16);
                        Vector2 pos = position + new Vector2(x * 16, y * 16);
                        sb.Draw(texture, pos * scale, value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            return true;
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
    }
}
