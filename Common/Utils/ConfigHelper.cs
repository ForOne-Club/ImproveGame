using ImproveGame.Packets;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace ImproveGame.Common.Utils;

public static class ConfigHelper
{
    public static void SetConfigValue(ModConfig config, FieldInfo fieldInfo, object value, bool broadcast = true)
    {
        if (fieldInfo.FieldType != value.GetType())
            throw new Exception($"Field type mismatch: {fieldInfo.FieldType} != {value.GetType()}");

        var modConfig = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == config.Name);

        // TML的注释：
        // Main Menu: Save, leave reload for later
        // MP with ServerSide: Send request to server
        // SP or MP with ClientSide: Apply immediately if !NeedsReload
        if (Main.gameMenu)
        {
            fieldInfo.SetValue(config, value);
            ConfigManager.Save(config); // 保存配置到文件
            ConfigManager.Load(modConfig); // 重新加载配置
            // modConfig.OnChanged(); delayed until ReloadRequired checked
            // Reload will be forced by Back Button in UIMods if needed
        }
        // 处于游戏内
        else
        {
            // 需要重新加载，不允许保存
            bool reloadRequired = fieldInfo.GetCustomAttribute<ReloadRequiredAttribute>() is not null;
            if (reloadRequired)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Main.NewText(Language.GetTextValue("tModLoader.ModConfigCantSaveBecauseChangesWouldRequireAReload"),
                    Color.Red); //"Can't save because changes would require a reload."
                return;
            }

            // 服务器端配置，处于客户端，需要广播
            if (modConfig.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient && broadcast)
            {
                // 发送更好的体验自己的包
                ConfigOptionPacket.Send(config, fieldInfo, value);
                return;
            }

            // 本地配置，或者处于服务器端，或者处于客户端，但不需要广播
            fieldInfo.SetValue(config, value);
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