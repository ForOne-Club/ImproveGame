using System.Diagnostics;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ImproveGame.Common.Configs.Elements;

internal class OpenUIConfigElement : OpenFileElement
{
    protected override string FilePath => Path.Combine(ConfigManager.ModConfigPath, "ImproveGame_UIConfigs.json");
}

internal class OpenConfigElement : OpenFileElement
{
    protected override string FilePath => Path.Combine(ConfigManager.ModConfigPath, "ImproveGame_ImproveConfigs.json");
}

internal abstract class OpenFileElement : LargerPanelElement
{
    protected abstract string FilePath { get; }

    public override void LeftClick(UIMouseEvent evt) {
        base.LeftClick(evt);

        if (!File.Exists(FilePath)) return;
        Process.Start(new ProcessStartInfo(FilePath)
        {
            UseShellExecute = true
        });
    }
}