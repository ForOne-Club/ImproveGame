using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig;

/// <summary>
/// 用于ModernConfig，仅在指定配置项为true时显示此配置项
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DisplayConditionAttribute (string configName, string flagName) : Attribute
{
    public bool IsVisible => (bool)Config.GetType().GetField(FlagName)?.GetValue(Config)!;
    public ModConfig Config => ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == configName);
    public string FlagName { get; } = flagName;
}
