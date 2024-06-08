using ImproveGame.Common.Configs;
using Newtonsoft.Json;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace ImproveGame.Packets;

[AutoSync]
public class ConfigOptionPacket : NetModule
{
    private string _configName;
    private string _fieldName;
    private string _json;

    public static void Send(ModConfig modConfig, FieldInfo fieldInfo, object value)
    {
        string json = JsonConvert.SerializeObject(value, ConfigManager.serializerSettings);
        var module = NetModuleLoader.Get<ConfigOptionPacket>();
        module._configName = modConfig.Name;
        module._fieldName = fieldInfo.Name;
        module._json = json;
        module.Send();
    }

    public override void Receive()
    {
        var modConfig = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == _configName);
        var fieldInfo = modConfig.GetType().GetField(_fieldName);
        if (fieldInfo == null)
            return;

        var value = JsonConvert.DeserializeObject(_json, fieldInfo.FieldType, ConfigManager.serializerSettings);
        ConfigHelper.SetConfigValue(modConfig, fieldInfo, value, false);

        // 转发到全体
        if (Main.netMode is NetmodeID.Server)
            Send();
    }
}