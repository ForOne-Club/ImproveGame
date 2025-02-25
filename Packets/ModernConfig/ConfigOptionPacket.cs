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

//[AutoSync]//不知道为什么path会同步失败，就自己发包了
public class ConfigOptionPacket : NetModule
{
    private string _modName;
    private string _configName;
    private string _json;
    private string _valueTypeFullName;
    private string[] path;
    private string _popInfo;
    private bool _rejected;
    public static void Send(ModConfig modConfig, PropertyFieldWrapper variableInfo, object value, List<string> path = null)
    {
        string json = JsonConvert.SerializeObject(value, ConfigManager.serializerSettings);
        var module = NetModuleLoader.Get<ConfigOptionPacket>();
        module._modName = modConfig.Mod.Name;
        module._configName = modConfig.Name;
        module._json = json;
        module._valueTypeFullName = value.GetType().FullName;
        List<string> cachedPath = [];
        if (path != null)
            cachedPath.AddRange(path);
        cachedPath.Add(variableInfo.Name);
        module.path = [.. cachedPath];
        path?.ToArray();
        module._popInfo = "";
        module._rejected = false;
        module.Send();
    }
    public override void Send(ModPacket p)
    {
        p.Write(_modName);
        p.Write(_configName);
        p.Write(_json);
        p.Write(_valueTypeFullName);
        p.Write(path.Length);
        foreach (var str in path)
            p.Write(str);
        p.Write(_popInfo);
        p.Write(_rejected);
        base.Send(p);
    }
    public override void Read(BinaryReader r)
    {
        _modName = r.ReadString();
        _configName = r.ReadString();
        _json = r.ReadString();
        _valueTypeFullName = r.ReadString();
        int length = r.ReadInt32();
        path = new string[length];
        for (int n = 0; n < length; n++)
            path[n] = r.ReadString();
        _popInfo = r.ReadString();
        _rejected = r.ReadBoolean();
        base.Read(r);
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

        var valueTypeFullName = System.Type.GetType(_valueTypeFullName);
        object value = JsonConvert.DeserializeObject(_json, valueTypeFullName, ConfigManager.serializerSettings);

        // 转发到全体
        if (Main.netMode is NetmodeID.Server)
        {
            ModConfig pendingConfig = ConfigManager.GeneratePopulatedClone(modConfig);
            ConfigHelper.SetItemViaPath(pendingConfig, path, value);
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

            ConfigHelper.SetItemViaPath(modConfig, path, value);
            ConfigManager.Save(modConfig);
            if (!Main.gameMenu)
                modConfig.OnChanged();
            _popInfo = Language.GetTextValue("tModLoader.ModConfigServerResponse", netText.ToString());
            _rejected = false;
            Send();
        }
        else
        {
            ConfigHelper.SetItemViaPath(modConfig, path, value);
            ConfigManager.Save(modConfig);
            if (!Main.gameMenu)
                modConfig.OnChanged();
            if (_popInfo != null && _popInfo.Length > 0)
            {
                ModernConfigUI.PopNewInfo(_popInfo, Main.MouseScreen - FontAssets.MouseText.Value.MeasureString(_popInfo) * new Vector2(1f, 0f) - new Vector2(64, 32), _rejected ? Color.Red : Color.Green);
            }
        }

    }
    public void SendRejectedConfig(string text, ModConfig modConfig)
    {
        _rejected = true;
        _popInfo = text;
        //var modConfig = ConfigManager.Configs[ModLoader.GetMod(_modName)].Find(i => i.Name == _configName);
        object item = ConfigHelper.GetItemViaPath(modConfig, path);
        _json = JsonConvert.SerializeObject(item, ConfigManager.serializerSettings);
        Send();
    }
}