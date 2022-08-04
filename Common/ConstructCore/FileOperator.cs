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
            // 这里不能合并，合并就坏了
            TagCompound tag = new();
            tag.Add("ModVersion", ImproveGame.Instance.Version.ToString());
            tag.Add("Width", rectInWorld.Width);
            tag.Add("Height", rectInWorld.Height);

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
