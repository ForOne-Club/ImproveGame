using System.Reflection;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Utils;

public static class ConfigHelper
{
    public static void SetConfigValue(ModConfig config, FieldInfo fieldInfo, object value, bool broadcast = true)
    {
        if (fieldInfo.FieldType != value.GetType())
            throw new Exception($"Field type mismatch: {fieldInfo.FieldType} != {value.GetType()}");
        
        fieldInfo.SetValue(config, value);
        
        var modConfig = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == config.Name);
        // Main Menu: Save, leave reload for later
        // MP with ServerSide: Send request to server
        // SP or MP with ClientSide: Apply immediately if !NeedsReload
        if (Main.gameMenu) {
            ConfigManager.Save(config); // 保存配置到文件
            ConfigManager.Load(modConfig); // 重新加载配置
            // modConfig.OnChanged(); delayed until ReloadRequired checked
            // Reload will be forced by Back Button in UIMods if needed
        }
        else {
            ConfigManager.Save(config);
            ConfigManager.Load(modConfig);
            modConfig.OnChanged();
        }
    }
    
    public static string GetLocalizationKey(ModConfig config, string optionName)
        => $"Configs.{config.Name}.{optionName}";
    
    public static string GetTooltip(ModConfig config, string optionName)
        => GetText($"{GetLocalizationKey(config, optionName)}.Tooltip");
    
    public static string GetLabel(ModConfig config, string optionName)
        => GetText($"{GetLocalizationKey(config, optionName)}.Label");
}