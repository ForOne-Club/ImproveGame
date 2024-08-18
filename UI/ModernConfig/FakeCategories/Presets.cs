using ImproveGame.UI.ModernConfig.OfficialPresets;
using ImproveGame.UI.ModernConfig.OptionElements.PresetElements;
using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.ModernConfig.FakeCategories;

public sealed class Presets : Category
{
    private static int _directoryCount;

    public override Texture2D GetIcon()
    {
        return ModAsset.PaintWand.Value;
    }

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        panel.AddOfficialPreset<ExplorerPreset>();
        panel.AddOfficialPreset<WarriorPreset>();
        panel.AddOfficialPreset<VanillaPreset>();
        panel.AddToOptionsDirect(new AddPresetElement());
        panel.AddToOptionsDirect(new OpenFolderElement());

        string folderPath = PresetHandler.ConfigPresetsPath; // 替换为你的文件夹路径
        var di = new DirectoryInfo(folderPath);
        var directories = di.GetDirectories();
        foreach (DirectoryInfo directory in directories)
            panel.AddToOptionsDirect(new PlayerPresetElement(directory.Name));
        _directoryCount = directories.Length;

        // 用来监听文件夹变化，并不是实际的UI
        var watcherView = new View
        {
            IgnoresMouseInteraction = true
        };
        watcherView.OnUpdate += _ =>
        {
            var info = new DirectoryInfo(folderPath);
            if (info.GetDirectories().Length != _directoryCount)
                ConfigOptionsPanel.Instance.DelayRefreshCurrentPage = true;
        };
        panel.AddToOptionsDirect(watcherView);
    }
}