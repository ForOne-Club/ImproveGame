using ImproveGame.Common.Configs;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.OfficialPresets;

public abstract class OfficialPreset
{
    public virtual string LocalizationKey => GetType().Name;

    public string Label => GetText($"ModernConfig.Presets.{LocalizationKey}.Label");
    public string Tooltip => GetText($"ModernConfig.Presets.{LocalizationKey}.Tooltip");
    public string Link => GetText($"ModernConfig.Presets.{LocalizationKey}.Link");

    public abstract void ApplyPreset(ImproveConfigs modConfig, UIConfigs uiConfig,
        AvailableModItemConfigs modItemConfig);

    public void OnApply()
    {
        var modConfig = Config;
        var uiConfig = UIConfigs.Instance;
        var modItemConfig = AvailableConfig;

        ApplyPreset(modConfig, uiConfig, modItemConfig);

        var modConfigToLoad = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == modConfig.Name);
        var uiConfigToLoad = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == uiConfig.Name);
        var modItemConfigToLoad = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == modItemConfig.Name);

        ConfigManager.Save(modConfig); // 保存配置到文件
        ConfigManager.Save(uiConfig); // 保存配置到文件
        ConfigManager.Save(modItemConfig); // 保存配置到文件
        ConfigManager.Load(modConfigToLoad); // 重新加载配置
        ConfigManager.Load(uiConfigToLoad); // 重新加载配置
        ConfigManager.Load(modItemConfigToLoad); // 重新加载配置

        // TML的注释：
        // Main Menu: Save, leave reload for later
        // MP with ServerSide: Send request to server
        // SP or MP with ClientSide: Apply immediately if !NeedsReload
        if (Main.gameMenu)
        {
            // modConfig.OnChanged(); delayed until ReloadRequired checked
            // Reload will be forced by Back Button in UIMods if needed
        }
        // 处于游戏内
        else
        {
            modConfig.OnChanged();
            uiConfig.OnChanged();
        }
    }
}