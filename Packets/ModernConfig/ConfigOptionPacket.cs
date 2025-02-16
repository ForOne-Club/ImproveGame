using ImproveGame.Common.Configs;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI.ModernConfig;
using ImproveGame.UI.ModernConfig.OptionElements;
using Newtonsoft.Json;
using System.Reflection;
using Terraria.Chat;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Packets;

[AutoSync]
public class ConfigOptionPacket : NetModule
{
    private string _modName;
    private string _configName;
    private string _fieldName;
    private string _json;
    private string[] path;
    private string _popInfo;
    private bool _rejected;
    public static void Send(ModConfig modConfig, PropertyFieldWrapper variableInfo, object value, List<string> path = null)
    {
        string json = JsonConvert.SerializeObject(value, ConfigManager.serializerSettings);
        var module = NetModuleLoader.Get<ConfigOptionPacket>();
        module._modName = modConfig.Mod.Name;
        module._configName = modConfig.Name;
        module._fieldName = variableInfo.Name;
        module._json = json;
        module.path = path?.ToArray();
        module._popInfo = "";
        module._rejected = false;
        module.Send();
    }
    public override void Receive()
    {

        var modConfig = ConfigManager.Configs[ModLoader.GetMod(_modName)].Find(i => i.Name == _configName);
        if (Main.netMode is NetmodeID.Server)
        {
            if (_modName == "ImproveGame" && Config.OnlyHostByPassword && !NetPasswordSystem.Registered[Sender])// 理论上不可能出现的情况，没有验证还是发了包
            {
                //ChatHelper.SendChatMessageToClient(
                //        new NetworkText(GetText("Configs.ImproveConfigs.OnlyHostByPassword.Unaccepted"),
                //        NetworkText.Mode.Literal), Color.Red, Sender);
                SendRejectedConfig(GetText("Configs.ImproveConfigs.OnlyHostByPassword.Unaccepted"), modConfig);
                return;
            }

        }


        PropertyFieldWrapper variableInfo = ModernConfigOption.GetWrapper(modConfig.GetType(), _fieldName);
        var value = JsonConvert.DeserializeObject(_json, variableInfo.Type, ConfigManager.serializerSettings);

        object item = modConfig;
        var bindFlag = BindingFlags.Public | BindingFlags.Instance;
        if (path != null)
            foreach (var p in path)
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
        // 转发到全体
        if (Main.netMode is NetmodeID.Server)
        {
            ModConfig pendingConfig = ConfigManager.GeneratePopulatedClone(modConfig);
            object pendingItem = pendingConfig;
            if (path != null)
                foreach (var p in path)
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
            ConfigHelper.SetConfigValue(pendingConfig, variableInfo, value, pendingItem, false, true, path: path == null ? null : [.. path]);

            var netText = NetworkText.FromKey("tModLoader.ModConfigAccepted");
            bool flag = modConfig.AcceptClientChanges(pendingConfig, Sender, ref netText);
            if (!flag)
            {
                SendRejectedConfig(netText.ToString(), modConfig);
                return;
            }
            var text = "";
#pragma warning disable CS0618 // Type or member is obsolete
            flag = modConfig.AcceptClientChanges(pendingConfig, Sender, ref text);
            if (!flag)
            {
                SendRejectedConfig(text, modConfig);
                return;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            ConfigHelper.SetConfigValue(modConfig, variableInfo, value, item, false,false, path: path == null ? null : [.. path]);
            _popInfo = Language.GetTextValue("tModLoader.ModConfigServerResponse", netText.ToString());
            _rejected = false;
            Send();
        }
        else
        {
            ConfigHelper.SetConfigValue(modConfig, variableInfo, value, item, false,false, path: path == null ? null : [.. path]);
            if (_popInfo != null && _popInfo.Length > 0)
            {
                ModernConfigUI.PopNewInfo(_popInfo, Main.MouseScreen - ModernConfigUI.Instance.MainPanel.GetDimensions().Position() - FontAssets.MouseText.Value.MeasureString(_popInfo) * new Vector2(1f, 0f) - new Vector2(64,32), _rejected ? Color.Red : Color.Green);
            }
        }

    }
    public void SendRejectedConfig(string text, ModConfig modConfig)
    {
        _rejected = true;
        _popInfo = text;
        //var modConfig = ConfigManager.Configs[ModLoader.GetMod(_modName)].Find(i => i.Name == _configName);
        object item = modConfig;
        var bindFlag = BindingFlags.Public | BindingFlags.Instance;
        if (path != null)
            foreach (var p in path)
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
        PropertyFieldWrapper variableInfo = ModernConfigOption.GetWrapper(modConfig.GetType(), _fieldName);
        _json = JsonConvert.SerializeObject(variableInfo.GetValue(item), ConfigManager.serializerSettings);
        Send();
    }
}