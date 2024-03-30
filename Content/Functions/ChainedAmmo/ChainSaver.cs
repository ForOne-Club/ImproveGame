using ImproveGame.Core;
using ImproveGame.UI.AmmoChainPanel;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.ChainedAmmo;

public class ChainSaver
{
    internal static Dictionary<string, AmmoChain> AmmoChains = new();

    internal static string SavePath => Paths.SavePath;
    internal static string Extension => ".ammochain";

    public static void SaveAsFile(AmmoChain chain, string chainName)
    {
        TrUtils.TryCreatingDirectory(SavePath);

        string name = $"{chainName}{Extension}";
        string thisPath = Path.Combine(SavePath, name);
        if (File.Exists(thisPath))
        {
            for (int i = 2; i <= 999; i++)
            {
                name = $"{chainName} ({i}){Extension}";
                thisPath = Path.Combine(SavePath, name);
                if (!File.Exists(thisPath))
                {
                    break;
                }
            }
        }

        TagIO.ToFile(chain.SerializeData(), thisPath);

        AmmoChains.Clear();
    }

    public static bool LoadFile(string path)
    {
        TagCompound tag = TagIO.FromFile(path);
        AmmoChains.Add(path, AmmoChain.DESERIALIZER.Invoke(tag));
        return true;
    }

    public static AmmoChain GetChainFromFile(string path)
    {
        if (!AmmoChains.TryGetValue(path, out AmmoChain chain))
        {
            try
            {
                LoadFile(path);
            }
            catch
            {
                AddNotification(GetText("UI.AmmoChain.FileLoadError"), Color.Red);
                return null;
            }

            chain = AmmoChains[path];
        }

        return chain;
    }

    /// <summary>
    /// 读取所有弹药链文件，并将其存入AmmoChains。由于文件小，不需要异步（除非你搞几百个来专门压力测试）
    /// </summary>
    public static void ReadAllAmmoChains()
    {
        AmmoChains.Clear();
        foreach (string file in Directory.GetFiles(SavePath, $"*{Extension}"))
        {
            LoadFile(file);
        }
    }
}