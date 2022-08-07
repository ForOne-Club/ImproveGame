using ImproveGame.Common.Systems;
using ImproveGame.Interface.Common;
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

            TagIO.ToFile(new QoLStructure(rectInWorld).Tag, thisPath);

            CachedStructureDatas.Clear();
            if (StructureGUI.Visible && UISystem.Instance.StructureGUI is not null)
            {
                UISystem.Instance.StructureGUI.CacheSetupStructures = true;
                if (WandSystem.ConstructFilePath == thisPath)
                    WandSystem.ConstructFilePath = string.Empty;
            }
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

        public void Load(Mod mod) { }

        public void Unload() => CachedStructureDatas = null;
    }
}
