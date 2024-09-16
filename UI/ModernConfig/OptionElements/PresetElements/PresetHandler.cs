﻿using ImproveGame.Common.Configs;
using ImproveGame.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements.PresetElements;

public class PresetHandler
{
    public static string ConfigPresetsPath => Path.Combine(Paths.SavePath, "ConfigPresets");
    private const string FileNameModConfig = "ImproveConfigs.json";
    private const string FileNameUIConfig = "UIConfigs.json";

    public static readonly JsonSerializerSettings SerializerSettings = new ()
    {
        Formatting = Formatting.Indented, // 缩进
        DefaultValueHandling = DefaultValueHandling.Include, // 包括所有默认值
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver()
    };

    public static void RenamePreset(string oldName, string newName)
    {
        if (oldName == newName)
            return;

        TrUtils.TryCreatingDirectory(ConfigPresetsPath);
        string path = Path.Combine(ConfigPresetsPath, oldName);
        if (!Directory.Exists(path))
            return;

        string newPath = Path.Combine(ConfigPresetsPath, newName);
        Directory.Move(path, newPath);
    }

    public static void DeletePreset(string name)
    {
        TrUtils.TryCreatingDirectory(ConfigPresetsPath);
        string path = Path.Combine(ConfigPresetsPath, name);
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    /// <summary>
    /// 将当前配置保存到指定路径
    /// </summary>
    public static void SaveAsPreset(string name)
    {
        if (Main.netMode is NetmodeID.Server)
            return;

        TrUtils.TryCreatingDirectory(ConfigPresetsPath);

        // 避免重名
        string adjustedName = name;
        string path = Path.Combine(ConfigPresetsPath, adjustedName);
        if (Directory.Exists(path))
        {
            for (int i = 2; i <= 999; i++)
            {
                adjustedName = $"{name} ({i})";
                path = Path.Combine(ConfigPresetsPath, adjustedName);
                if (!Directory.Exists(path))
                {
                    break;
                }
            }
        }

        TrUtils.TryCreatingDirectory(path);

        // Save current configs to the path
        string jsonModConfig = JsonConvert.SerializeObject(Config, SerializerSettings);
        File.WriteAllText(Path.Combine(path, FileNameModConfig), jsonModConfig);

        string jsonUIConfig = JsonConvert.SerializeObject(UIConfigs.Instance, SerializerSettings);
        File.WriteAllText(Path.Combine(path, FileNameUIConfig), jsonUIConfig);
    }

    /// <summary>
    /// 从指定路径加载配置并应用
    /// </summary>
    public static void LoadAndApplyPreset(string name)
    {
        TrUtils.TryCreatingDirectory(ConfigPresetsPath);
        string path = Path.Combine(ConfigPresetsPath, name);
        Directory.CreateDirectory(path);

        var modConfigPath = Path.Combine(path, FileNameModConfig);
        var uiConfigPath = Path.Combine(path, FileNameUIConfig);

        if (File.Exists(modConfigPath))
        {
            var jsonModConfig = File.ReadAllText(modConfigPath);
            LoadAndApplyConfig<ImproveConfigs>(jsonModConfig);
        }

        if (File.Exists(uiConfigPath))
        {
            var jsonUIConfig = File.ReadAllText(uiConfigPath);
            LoadAndApplyConfig<UIConfigs>(jsonUIConfig);
        }
    }

    public static void LoadAndApplyConfig<T>(string jsonText) where T : ModConfig
    {
        var loadedConfig = JsonConvert.DeserializeObject<T>(jsonText, SerializerSettings);
        var modConfig = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == loadedConfig.GetType().Name);
        loadedConfig.Name = modConfig.Name;
        loadedConfig.Mod = modConfig.Mod;

        if (Main.gameMenu)
        {
            ConfigManager.Save(loadedConfig);
            ConfigManager.Load(modConfig);
        }
        else
        {
            // 纠正ReloadRequired的字段
            CorrectReloadRequiredFields(loadedConfig, modConfig);

            // 游戏内发包
            if (loadedConfig.Mode == ConfigScope.ServerSide && Main.netMode == NetmodeID.MultiplayerClient)
            {
                var requestChanges = new ModPacket(MessageID.InGameChangeConfig);
                requestChanges.Write(loadedConfig.Mod.Name);
                requestChanges.Write(loadedConfig.Name);
                string json = JsonConvert.SerializeObject(loadedConfig, ConfigManager.serializerSettingsCompact);
                requestChanges.Write(json);
                requestChanges.Send();
                return;
            }

            ConfigManager.Save(loadedConfig);
            ConfigManager.Load(modConfig);
            modConfig.OnChanged();
        }
    }

    /// <summary>
    /// 将加载到的配置的ReloadRequired的字段纠正到和当前配置一致
    /// </summary>
    /// <param name="loadedConfig">加载到的配置</param>
    /// <param name="currentConfig">当前配置</param>
    private static void CorrectReloadRequiredFields(ModConfig loadedConfig, ModConfig currentConfig)
    {
        foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(loadedConfig))
        {
            var reloadRequired =
                ConfigManager.GetCustomAttributeFromMemberThenMemberType<ReloadRequiredAttribute>(variable,
                    loadedConfig, null);

            if (reloadRequired == null)
            {
                continue;
            }

            var loadedValue = variable.GetValue(loadedConfig);
            var currentValue = variable.GetValue(currentConfig);
            if (!ConfigManager.ObjectEquals(loadedValue, currentValue))
            {
                variable.SetValue(loadedConfig, currentValue);
            }
        }
    }
}