using ImproveGame.Common.Configs;
using ImproveGame.Common.ModSystems;
using Newtonsoft.Json;
using System.Reflection;
using Terraria.Chat;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Packets;

[AutoSync]
public class ConfigOptionPacket : NetModule
{
    private string _configName;
    private string _fieldName;
    private string _json;
    private string[] path;
    public static void Send(ModConfig modConfig, PropertyFieldWrapper variableInfo, object value,List<string> path = null)
    {
        string json = JsonConvert.SerializeObject(value, ConfigManager.serializerSettings);
        var module = NetModuleLoader.Get<ConfigOptionPacket>();
        module._configName = modConfig.Name;
        module._fieldName = variableInfo.Name;
        module._json = json;
        module.path = path?.ToArray();
        module.Send();
    }
    public override void Receive()
    {
        // 理论上不可能出现的情况，没有验证还是发了包
        if (Main.netMode is NetmodeID.Server && Config.OnlyHostByPassword && !NetPasswordSystem.Registered[Sender])
        {
            ChatHelper.SendChatMessageToClient(
                new NetworkText(GetText("Configs.ImproveConfigs.OnlyHostByPassword.Unaccepted"),
                    NetworkText.Mode.Literal), Color.Red, Sender);
            return;
        }

        var modConfig = ConfigManager.Configs[ImproveGame.Instance].Find(i => i.Name == _configName);


        PropertyFieldWrapper variableInfo = null;
        var fieldInfo = modConfig.GetType().GetField(_fieldName);
        var propertyInfo = modConfig.GetType().GetField(_fieldName);
        if (fieldInfo != null)
            variableInfo = new PropertyFieldWrapper(fieldInfo);
        else if (propertyInfo != null)
            variableInfo = new PropertyFieldWrapper(propertyInfo);
        if (variableInfo == null)
            return;
        var value = JsonConvert.DeserializeObject(_json, fieldInfo.FieldType, ConfigManager.serializerSettings);

        object item = modConfig;
        var bindFlag = BindingFlags.Public | BindingFlags.Instance;
        if (path != null) 
            foreach(var p in path) 
            {
                var curType = item.GetType();
                var fld = curType.GetField(p, bindFlag);
                var prop = curType.GetProperty(p, bindFlag);
                if (fld != null)
                    item = fld.GetValue(item);
                else if (prop != null)
                    item = prop.GetValue(item);
                else
                    throw new Exception("Property or field doesn't exist in " + curType.Name);
            }

        ConfigHelper.SetConfigValue(modConfig, variableInfo, value,item, false, path: [.. path]);

        // 转发到全体
        if (Main.netMode is NetmodeID.Server)
            Send();
    }
}