using ImproveGame.Common.ModSystems;
using ImproveGame.Packets;
using rail;
using System.IO;
using System;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using System.Collections.Generic;
using Terraria.ModLoader.UI;
using System.Linq;
using System.Collections;
using ImproveGame.UI.ModernConfig.OptionElements;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ImproveGame.Common.Utils;

public static class ConfigHelper
{
    static void InternalSetValue(ModConfig modConfig, PropertyFieldWrapper variableInfo, object value, object item, List<string> path, IList List, int Index)
    {
        if (!item.GetType().IsValueType)
            if (List != null)
                List[Index] = value;
            else
                variableInfo.SetValue(item, value);
        else
        {
            //给struct套struct做的神必适配
            //按说应该没人会把struct高强度套娃塞进ModConfig吧不会吧
            if (path == null)
                throw new Exception("You must give a path when setvalue to a struct");
            List<PropertyFieldWrapper> fldInfo = [];
            object obj = modConfig;
            int start = 0;
            IList objList = null;
            int listIndex = -1;
            for (int i = path.Count - 1; i >= 0; i--)
            {
                if (int.TryParse(path[i], out var index))
                {
                    start = i + 1;
                    for (int k = 0; k < i; k++)
                    {
                        if (int.TryParse(path[k], out var idx))
                            obj = ((IList)obj)[idx];
                        else
                            obj = ModernConfigOption.GetWrapper(obj.GetType(), path[k]).GetValue(obj);
                    }
                    if (obj is IList list)
                    {
                        objList = list;
                        obj = objList[index];
                    }
                    else if (obj is IDictionary dict)
                    {
                        string type = path[i + 1];
                        objList = new List<object>();
                        foreach (var v in (type == "Value" ? dict.Values : dict.Keys))
                        {
                            objList.Add(v);
                        }
                        obj = objList[index];

                    }
                    listIndex = index;

                    break;
                }
            }
            List<object> objs = [obj];
            Type curType = obj.GetType();
            object curObj = obj;
            for (int i = start; i < path.Count; i++)
            {
                var wrapper = ModernConfigOption.GetWrapper(curType, path[i]);
                fldInfo.Add(wrapper);
                curObj = wrapper.GetValue(curObj);
                objs.Add(curObj);
                curType = wrapper.Type;
            }
            fldInfo.Add(variableInfo);
            variableInfo.SetValue(objs[^1], value);
            variableInfo.SetValue(item, value);//两个是不同的引用，一个在ModConfig里，一个在Option里，都得改

            int count = objs.Count;
            for (int k = 2; k <= count; k++)
            {
                fldInfo[^k].SetValue(objs[^k], objs[^(k - 1)]);
                if (!objs[^k].GetType().IsValueType && k != count)
                    break;
            }
            if (objList != null)
            {
                objList[listIndex] = objs[^1];
            }
        }
    }
    public static object GetItemViaPathForSetDefault(object target, IEnumerable<string> path, out bool failed, bool privateAllowed = false)
    {
        failed = false;
        object item = target;
        object lastItem = item;
        PropertyFieldWrapper prevWrapper = null;
        var bindFlag = BindingFlags.Public | BindingFlags.Instance;
        if (privateAllowed)
            bindFlag |= BindingFlags.NonPublic;
        if (path != null)
            foreach (var p in path)
            {
                if (item == null)
                {
                    failed = true;
                    return null;
                }
                var curType = item.GetType();
                var fld = curType.GetField(p, bindFlag);
                var prop = curType.GetProperty(p, bindFlag);
                if (fld != null)
                {
                    item = fld.GetValue(item);
                    prevWrapper = new PropertyFieldWrapper(fld);
                }
                else if (prop != null)
                {
                    item = prop.GetValue(item);
                    prevWrapper = new PropertyFieldWrapper(prop);
                }
                else if (item is IEnumerable collection && int.TryParse(p, out int index))
                {
                    int counter = 0;
                    bool useElementDefault = true;
                    foreach (var i in collection)
                    {
                        if (counter == index)
                        {
                            item = i;
                            useElementDefault = false;
                            break;
                        }
                        counter++;
                    }
                    if (useElementDefault)
                    {
                        object toAdd;
                        var DefaultListValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<DefaultListValueAttribute>(prevWrapper, lastItem, null);
                        var JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<JsonDefaultListValueAttribute>(prevWrapper, lastItem, null);
                        bool isList = curType.GetGenericTypeDefinition() == typeof(List<>);
                        bool isSet = curType.GetGenericTypeDefinition() == typeof(HashSet<>);
                        bool isDictionary = curType.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                        if (!isList && !isDictionary && !isSet)
                            throw new Exception("Collection Not Support");
                        if (DefaultListValueAttribute != null)
                        {
                            toAdd = DefaultListValueAttribute.Value;
                        }
                        else
                        {
                            var type = curType.GetGenericArguments()[isDictionary ? 1 : 0];
                            toAdd = ConfigManager.AlternateCreateInstance(type);
                            if (!type.IsValueType && type != typeof(string))
                            {
                                string json = JsonDefaultListValueAttribute?.Json ?? "{}";

                                JsonConvert.PopulateObject(json, toAdd, ConfigManager.serializerSettings);
                            }
                        }
                        if (isList)
                        {
                            item = toAdd;
                        }
                        else if (isSet)
                        {
                            var genericType = typeof(SetElementWrapper<>).MakeGenericType(curType.GetGenericArguments()[0]);
                            item = Activator.CreateInstance(genericType, [toAdd, item]);
                        }
                        else
                        {
                            object keyValue;
                            var keyType = curType.GetGenericArguments()[0];

                            var defaultDictionaryKeyValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<DefaultDictionaryKeyValueAttribute>(prevWrapper, lastItem, null);
                            if (defaultDictionaryKeyValueAttribute != null)
                            {
                                keyValue = defaultDictionaryKeyValueAttribute.Value;
                            }
                            else
                            {
                                keyValue = ConfigManager.AlternateCreateInstance(keyType);
                                var jsonDefaultDictionaryKeyValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<JsonDefaultDictionaryKeyValueAttribute>(prevWrapper, lastItem, null);
                                if (!keyType.IsValueType && keyType != typeof(string))
                                {
                                    string json = jsonDefaultDictionaryKeyValueAttribute?.Json ?? "{}";

                                    JsonConvert.PopulateObject(json, keyValue, ConfigManager.serializerSettings);
                                }
                            }
                            Type genericType = typeof(DictionaryElementWrapper<,>).MakeGenericType(keyType, toAdd.GetType());

                            item = Activator.CreateInstance(genericType, [keyValue, toAdd, item]);

                        }
                    }
                }
                else
                    throw new Exception("Property or field doesn't exist in " + curType.Name);
                lastItem = item;
            }
        if (item == null) return null;
        var lastType = item.GetType();
        if (lastType.IsGenericType && lastType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
        {
            var wrapperType = typeof(DictionaryElementWrapper<,>).MakeGenericType(lastType.GenericTypeArguments[0], lastType.GenericTypeArguments[1]);
            var Key = lastType.GetProperty("Key").GetValue(item);
            var Value = lastType.GetProperty("Value").GetValue(item);
            item = Activator.CreateInstance(wrapperType, [Key, Value, null]);
        }
        return item;
    }
    public static object GetItemViaPath(object target, IEnumerable<string> path, bool privateAllowed = false)
    {
        object item = target;
        object lastItem = item;
        var bindFlag = BindingFlags.Public | BindingFlags.Instance;
        if (privateAllowed)
            bindFlag |= BindingFlags.NonPublic;
        if (path != null)
            foreach (var p in path)
            {
                var curType = item!.GetType();
                var fld = curType.GetField(p, bindFlag);
                var prop = curType.GetProperty(p, bindFlag);
                if (fld != null)
                    item = fld!.GetValue(item)!;
                else if (prop != null)
                    item = prop!.GetValue(item)!;
                else if (item is IEnumerable collection && int.TryParse(p, out int index))
                {
                    int counter = 0;
                    bool flag = true;
                    foreach (var i in collection)
                    {
                        if (counter == index)
                        {
                            item = i;
                            flag = false;
                            break;
                        }
                        counter++;
                    }
                    if (flag)
                        throw new IndexOutOfRangeException();
                }
                else
                    throw new Exception("Property or field doesn't exist in " + curType.Name);

            }
        return item;
    }
    public static void SetConfigValue(ModConfig config, PropertyFieldWrapper variableInfo, object value, object item, bool broadcast = true, List<string> path = null, IList List = null, int Index = -1)
    {
        Type type = List != null ? List[Index].GetType() : variableInfo.Type;
        if (value != null && type != value.GetType())
            throw new Exception($"Field type mismatch: {variableInfo.Type} != {value.GetType()}");

        var modConfig = ConfigManager.Configs[config.Mod].Find(i => i.Name == config.Name);
        bool valueType = item.GetType().IsValueType;
        // TML的注释：
        // Main Menu: Save, leave reload for later
        // MP with ServerSide: Send request to server
        // SP or MP with ClientSide: Apply immediately if !NeedsReload
        if (Main.gameMenu)
        {
            InternalSetValue(config, variableInfo, value, item, path, List, Index);
            //fieldInfo.SetValue(config, value);
            ConfigManager.Save(config); // 保存配置到文件
            //ConfigManager.Load(modConfig); // 重新加载配置

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
            //variableInfo.SetValue(item, value);
            InternalSetValue(config, variableInfo, value, item, path, List, Index);

            ConfigManager.Save(config);
            modConfig.OnChanged();

        }
    }
    //public static void SetConfigValue(ModConfig config, PropertyFieldWrapper variableInfo, object value, bool broadcast = true) => SetConfigValue(config, variableInfo, value, config, broadcast);

    public static void SetItemViaPath(object target, IEnumerable<string> path, object value)
    {
        object item = target;
        object lastItem = item; ;
        var bindFlag = BindingFlags.Public | BindingFlags.Instance;
        int max = path.Count();
        int count = 0;
        if (path != null)
            foreach (var p in path)
            {
                var curType = item!.GetType();
                var fld = curType.GetField(p, bindFlag);
                var prop = curType.GetProperty(p, bindFlag);
                if (count != max - 1)
                {
                    lastItem = item;
                    if (fld != null)
                        item = fld!.GetValue(item)!;
                    else if (prop != null)
                        item = prop!.GetValue(item)!;
                    else if (item is IEnumerable collection && int.TryParse(p, out int index))
                    {
                        int counter = 0;
                        bool flag = true;
                        foreach (var i in collection)
                        {
                            if (counter == index)
                            {
                                item = i;
                                flag = false;
                                break;
                            }
                            counter++;
                        }
                        if (flag)
                            throw new IndexOutOfRangeException();
                    }
                    else
                        throw new Exception("Property or field doesn't exist in " + curType.Name);
                }
                else
                {
                    if (lastItem is IDictionary dict)
                    {
                        object Key = ((dynamic)item).Key;
                        object Value = ((dynamic)item).Value;
                        if (p == "Key")
                        {
                            dict.Remove(Key);
                            dict.Add(value, Value);
                        }
                        else
                            dict[Key] = value;
                        return;
                    }
                    fld?.SetValue(item, value);
                    prop?.SetValue(item, value);
                    if (item is IEnumerable collection && int.TryParse(p, out int index))
                    {
                        if (item is Array array)
                            array.SetValue(value, index);
                        else if (item is IList list)
                            list[index] = value;
                        else if (item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
                        {
                            var addMethod = item.GetType().GetMethod("Add", bindFlag);
                            var removeMethod = item.GetType().GetMethod("Remove", bindFlag);
                            List<object> cache = [.. collection];
                            cache.Reverse();
                            int targetCount = cache.Count - index;
                            foreach (var i in cache[0..targetCount])
                            {
                                removeMethod?.Invoke(item, [i]);
                            }
                            addMethod?.Invoke(item, [value]);
                            for (int i = targetCount - 2; i >= 0; i--)
                                addMethod?.Invoke(item, [cache[i]]);
                        }
                    }
                }
                count++;
            }
    }

    public static string GetModText(string modName, string str, out bool hasValue, params object[] arg)
    {
        string key = $"Mods.{modName}.{str}";
        string text = Language.GetTextValue(key, arg);
        hasValue = Language.Exists(key);
        return ConvertLeftRight(text);
    }
    public static string GetLocalizationKey(ModConfig config, string optionName)
        => $"Configs.{config.Name}.{optionName}";

    public static string GetTooltip(ModConfig config, string optionName)
    {
        string result = GetModText(config.Mod.Name, $"{GetLocalizationKey(config, optionName)}.Tooltip", out bool hasValue);
        return hasValue ? result : "";
    }

    public static string GetLabel(ModConfig config, string optionName)
    {
        string result = GetModText(config.Mod.Name, $"{GetLocalizationKey(config, optionName)}.Label", out bool hasValue);
        return hasValue ? result : optionName;
    }
}