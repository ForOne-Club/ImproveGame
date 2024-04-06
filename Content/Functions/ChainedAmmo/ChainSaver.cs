using ImproveGame.Core;
using ImproveGame.UI.AmmoChainPanel;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace ImproveGame.Content.Functions.ChainedAmmo;

public class ChainSaver
{
    internal static Dictionary<string, AmmoChain> AmmoChains = new();

    internal static string SavePath => Paths.SavePath;
    internal static string Extension => ".ammochain";

    /// <summary>
    /// 将弹药链保存成文件
    /// </summary>
    /// <returns>弹药链最终以什么名字保存</returns>
    public static string SaveAsFile(AmmoChain chain, string chainName)
    {
        if (Main.netMode is NetmodeID.Server)
            return "";

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

        return name;
    }

    public static void ModifyExistingFile(AmmoChain chain, string chainName)
    {
        if (Main.netMode is NetmodeID.Server)
            return;

        string name = $"{chainName}{Extension}";
        string thisPath = Path.Combine(SavePath, name);
        if (!File.Exists(thisPath))
        {
            SaveAsFile(chain, chainName);
            return;
        }

        using var stream = File.OpenWrite(thisPath);
        // 清空文件
        stream.Seek(0, SeekOrigin.Begin);
        stream.SetLength(0);
        // 写入
        TagIO.ToStream(chain.SerializeData(), stream);
    }

    public static void TryDeleteFile(string chainName, bool force = false)
    {
        if (Main.netMode is NetmodeID.Server)
            return;

        string name = $"{chainName}{Extension}";
        string thisPath = Path.Combine(SavePath, name);
        if (!File.Exists(thisPath))
            return;

        FileUtilities.Delete(thisPath, false, forceDeleteFile: force);
    }

    public static bool LoadFile(string path)
    {
        if (Main.netMode is NetmodeID.Server)
            return false;

        TagCompound tag = TagIO.FromFile(path);
        if (AmmoChain.IsTagInvalid(tag))
            return false;

        AmmoChain ammoChain;
        try
        {
            ammoChain = AmmoChain.DESERIALIZER.Invoke(tag);
        }
        catch
        {
            return false;
        }

        AmmoChains.Add(path, ammoChain);
        return true;
    }

    /// <summary>
    /// 读取所有弹药链文件，并将其存入AmmoChains。由于文件小，不需要异步（除非你搞几百个来专门压力测试）
    /// </summary>
    public static void ReadAllAmmoChains()
    {
        AmmoChains.Clear();

        if (Main.netMode is NetmodeID.Server)
            return;

        if (!TrUtils.TryCreatingDirectory(SavePath))
            return;

        foreach (string file in Directory.GetFiles(SavePath, $"*{Extension}"))
        {
            LoadFile(file);
        }
    }
}