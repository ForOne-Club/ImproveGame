using ImproveGame.Common.ModSystems;
using ImproveGame.Interface;
using ImproveGame.Interface.GUI;
using System.Collections.Generic;
using System.IO.Compression;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    public class FileOperator : ILoadable
    {
        internal static Dictionary<string, TagCompound> CachedStructureDatas = new();
        internal static string SavePath => Path.Combine(ModLoader.ModPath, "ImproveGame");
        internal static string Extension => ".qotstruct";

        public static void SaveAsFile(Rectangle rectInWorld)
        {
            TrUtils.TryCreatingDirectory(SavePath);

            string name = $"{GetText("ConstructGUI.Structure")}{Extension}";
            string thisPath = Path.Combine(SavePath, name);
            if (File.Exists(thisPath))
            {
                for (int i = 2; i <= 999; i++)
                {
                    name = $"{GetText("ConstructGUI.Structure")} ({i}){Extension}";
                    thisPath = Path.Combine(SavePath, name);
                    if (!File.Exists(thisPath))
                    {
                        break;
                    }
                }
            }

            AddNotification(GetText("ConstructGUI.SavedAs") + thisPath, Color.Yellow);

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
            CachedStructureDatas.Add(path, tag);
            return true;
        }

        public static TagCompound GetTagFromFile(string path)
        {
            if (!CachedStructureDatas.TryGetValue(path, out TagCompound tag))
            {
                try
                {
                    LoadFile(path);
                } 
                catch
                {
                    AddNotification(GetText("ConstructGUI.FileInfo.LoadError"), Color.Red);
                    return null;
                }

                tag = CachedStructureDatas[path];
            }

            return tag;
        }

        public void Load(Mod mod) { }

        public void Unload() => CachedStructureDatas = null;
    }
}
