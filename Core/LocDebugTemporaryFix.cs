using System;
using Terraria.ModLoader.Core;

namespace ImproveGame.Core;

public class LocDebugTemporaryFix : ModSystem
{
    public static ModKeybind LocalizationDebuggingKeybind { get; private set; }

    public override void Load()
    {
        LocalizationDebuggingKeybind = KeybindLoader.RegisterKeybind(Mod, "LocalizationDebugging", "OemTilde");
    }

    public override void Unload()
    {
        LocalizationDebuggingKeybind = null;
    }

    public override void PostUpdateInput()
    {
        // 上面在Load里注册只是在KeybindLoader里有注册到
        // 要正式能使用需要等到所有模组加载完，这个时候会把模组按键也添加到PlayerInput里面
        // 最简单粗暴的方式就是这样直接try了
        // 顺序大概是 Load -> PostSetUpContents -> 正式加载进模组的按键输入 -> 加载图鉴 -> 加载合成表
        // 在加载图鉴或者加载合成表的位置塞一个标记，没标记的时候直接return，这样也能解决问题
        // 但是看着很意义不明就是了(
        // 所以我就用直接try的方式了
        try
        {
            if (LocalizationDebuggingKeybind == null || !LocalizationDebuggingKeybind.JustPressed)
                return;
        }
        catch { }
        string path = Path.Combine(ModCompile.ModSourcePath, Mod.Name);
        // 使用Directory.GetFiles搜索所有子目录中的.hjson文件
        var locFiles = Directory.GetFiles(path, "*.hjson", SearchOption.AllDirectories);
        HashSet<(string Mod, string fileName)> values = [];
        foreach (var locFile in locFiles)
        {
            string fileName = Path.GetRelativePath(path, locFile); // 生成相对路径
            values.Add((Mod.Name, fileName));
        }

        LocalizationLoader.changedMods.Add(Mod.Name);
        LocalizationLoader.changedFiles.UnionWith(values);

        string originalSourceFolder = Mod.SourceFolder;
        Mod.SourceFolder = path;

        LanguageManager.Instance.ReloadLanguage();

        Mod.SourceFolder = originalSourceFolder;

        Main.NewText("Localization files reloaded");
        }
    }
