using System;
using Terraria.ModLoader.Core;

namespace ImproveGame.Core;

public class LocDebugTemporaryFix : ModSystem
{
    public static ModKeybind LocalizationDebuggingKeybind { get; private set; }

    public override void Load()
    {
        LocalizationDebuggingKeybind = KeybindLoader.RegisterKeybind(Mod, "[DEBUG]LocalizationDebugging", "OemTilde");
    }

    public override void PostUpdateInput()
    {
        if (!LocalizationDebuggingKeybind.JustPressed)
            return;

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
