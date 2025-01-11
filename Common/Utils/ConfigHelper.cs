using ImproveGame.Common.ModSystems;
using ImproveGame.Packets;
using rail;
using System.Reflection;
using Terraria.Chat;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Common.Utils;

public static class ConfigHelper
{
    public static void SetConfigValue(ModConfig config, PropertyFieldWrapper variableInfo, object value, object item, bool broadcast = true, List<string> path = null)
    {
        if (variableInfo.Type != value.GetType())
            throw new Exception($"Field type mismatch: {variableInfo.Type} != {value.GetType()}");

        var modConfig = ConfigManager.Configs[config.Mod].Find(i => i.Name == config.Name);
        bool valueType = item.GetType().IsValueType;
        // TML的注释：
        // Main Menu: Save, leave reload for later
        // MP with ServerSide: Send request to server
        // SP or MP with ClientSide: Apply immediately if !NeedsReload
        if (Main.gameMenu)
        {
            if (!valueType)
                variableInfo.SetValue(item, value);
            else
            {
                //给struct套struct做的神必适配
                //按说应该没人会把struct高强度套娃塞进ModConfig吧不会吧
                if (path == null)
                    throw new Exception("You must give a path when setvalue to a struct");
                var flag = BindingFlags.Public | BindingFlags.Instance;
                List<PropertyFieldWrapper> fldInfo = [];
                List<object> objs = [modConfig];
                Type curType = modConfig.GetType();
                object curObj = modConfig;
                for (int i = 0; i < path.Count; i++)
                {
                    var curFld = curType.GetField(path[i], flag);
                    var curProp = curType.GetField(path[i], flag);
                    PropertyFieldWrapper wrapper = null;
                    if (curFld != null)
                        wrapper = new PropertyFieldWrapper(curFld);
                    else if (curProp != null)
                        wrapper = new PropertyFieldWrapper(curProp);
                    else
                        throw new Exception("Property or field doesn't exist in " + curType.Name);

                    fldInfo.Add(wrapper);
                    curObj = curFld.GetValue(curObj);
                    objs.Add(curObj);
                    curType = curFld.FieldType;
                }
                fldInfo.Add(variableInfo);
                for (int k = 1; k <= objs.Count; k++)
                {
                    fldInfo[^k].SetValue(objs[^k], k == 1 ? value : objs[^(k - 1)]);
                    if (!objs[^k].GetType().IsValueType && k != objs.Count)
                    {
                        break;
                    }
                }

            }
            //fieldInfo.SetValue(config, value);
            ConfigManager.Save(config); // 保存配置到文件
            ConfigManager.Load(modConfig); // 重新加载配置
            // modConfig.OnChanged(); delayed until ReloadRequired checked
            // Reload will be forced by Back Button in UIMods if needed
        }
        // 处于游戏内
        else
        {
            // 需要重新加载，不允许保存
            bool reloadRequired = variableInfo.MemberInfo.GetCustomAttribute<ReloadRequiredAttribute>() is not null;
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
                // 没有通过主机IP验证
                if (Config.OnlyHost && !Main.countsAsHostForGameplay[Main.myPlayer])
                {
                    // “无法更改: 你不是服务器主机玩家！”
                    Main.NewText(GetText("Configs.ImproveConfigs.OnlyHost.Unaccepted"), Color.Red);
                    return;
                }

                if (Config.OnlyHostByPassword && !NetPasswordSystem.LocalPlayerRegistered)
                {
                    // “无法更改：你没有通过密码验证！”
                    Main.NewText(GetText("Configs.ImproveConfigs.OnlyHostByPassword.Unaccepted"), Color.Red);
                    return;
                }

                // 发送更好的体验自己的包
                ConfigOptionPacket.Send(config, variableInfo, value, path);
                return;
            }

            // 本地配置，或者处于服务器端，或者处于客户端，但不需要广播
            variableInfo.SetValue(config, value);
            ConfigManager.Save(config);
            ConfigManager.Load(modConfig);
            modConfig.OnChanged();
        }
    }
    //public static void SetConfigValue(ModConfig config, PropertyFieldWrapper variableInfo, object value, bool broadcast = true) => SetConfigValue(config, variableInfo, value, config, broadcast);
    public static string GetModText(string modName, string str, params object[] arg)
    {
        string text = Language.GetTextValue($"Mods.{modName}.{str}", arg);
        return ConvertLeftRight(text);
    }
    public static string GetLocalizationKey(ModConfig config, string optionName)
        => $"Configs.{config.Name}.{optionName}";

    public static string GetTooltip(ModConfig config, string optionName)
        => GetModText(config.Mod.Name, $"{GetLocalizationKey(config, optionName)}.Tooltip");

    public static string GetLabel(ModConfig config, string optionName)
        => GetModText(config.Mod.Name, $"{GetLocalizationKey(config, optionName)}.Label");
}