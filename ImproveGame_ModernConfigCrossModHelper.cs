using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
namespace ImproveGame;
// 复制到你的项目中之后记得右键解决方案资源管理器中的项目然后同步命名空间
// Copy it to your project and sync the namespace

//该文件给其它模组开发者提供配置中心的跨模组支持，具体参数信息参阅README.md的跨模组支持部分
//This file provides cross-mod assistance for other modders, read README.md for more details
public static class ImproveGame_ModernConfigCrossModHelper
{
    #region 注册分类表系列

    /// <summary>
    /// 这个函数提供了最基本的注册支持 | Basic support for registration
    /// <br>鉴于通常会注册多个分类表，qot请在外部自行调用<see cref="ModLoader.TryGetMod(string, out Mod)"/> | Use the method to get ImproveGame outside</br>
    /// <br>更具体一点 | In detail: <code>if(Main.netMode == NetmodeID.Server || !ModLoader.TryGetMod("ImproveGame",out var qot)) <br>    return;</br></code></br>
    /// </summary>
    /// <returns>是否注册成功 | Success or not</returns>
    public static bool RegisterCategory(Mod qot, Mod target, List<KeyValuePair<string, ModConfig>> variables, int itemIconID = 0,
Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
=> (bool)qot.Call(nameof(RegisterCategory), target, variables, itemIconID, getIconTexture, getLabel, getTooltip);

    /// <summary>
    /// 这个函数用于给单个设置实例批量添加设置选项进一个分类卡 | Register plenty of options from a single configuration
    /// </summary>
    public static void RegisterCategory(Mod qot, Mod target, ModConfig modConfig, List<string> variables, int itemIconID = 0,
        Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
        => RegisterCategory(qot, target, (from name in variables select new KeyValuePair<string, ModConfig>(name, modConfig)).ToList(),
            itemIconID, getIconTexture, getLabel, getTooltip);

    /// <summary>
    /// 这个函数用于给多个设置实例批量添加设置选项进一个分类卡 | Register plenty of options from multi-configuration
    /// </summary>
    public static void RegisterCategory(Mod qot, Mod target, List<(ModConfig, List<string>)> variables, int itemIconID = 0, Func<Texture2D> getIconTexture = null,
        Func<string> getLabel = null, Func<string> getTooltip = null)
    {
        List<KeyValuePair<string, ModConfig>> list = [];
        foreach (var pair in variables)
            list.AddRange((from name in pair.Item2 select new KeyValuePair<string, ModConfig>(name, pair.Item1)).ToList());
        RegisterCategory(qot, target, list, itemIconID, getIconTexture, getLabel, getTooltip);
    }

    #endregion

    /// <summary>
    /// 设置 “关于” 页面 | Set “About” page
    /// </summary>
    public static bool SetAboutPage(Mod qot, Mod target, Func<string> getAboutText, int itemIconID = 0,
Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
=> (bool)qot.Call(nameof(SetAboutPage), target, getAboutText, itemIconID, getIconTexture, getLabel, getTooltip);

    public static bool AddModernConfigTitle(Mod qot, Mod target, LocalizedText titleText)
        => (bool)qot.Call(nameof(AddModernConfigTitle), target, titleText);

    /// <summary>
    /// 注册某选项的预览绘制 | Register a preview drawing for an option
    /// </summary>
    public static bool RegisterPreview(Mod qot, PropertyFieldWrapper variableInfo, Action<UIElement, ModConfig, PropertyFieldWrapper, object, IList, int> previewDrawingMethod)
        => (bool)qot.Call(nameof(RegisterPreview), variableInfo, previewDrawingMethod);

    /// <summary>
    /// 注册全局预览绘制 | Register a global preview drawing
    /// </summary>
    public static bool OnGlobalConfigPreview(Mod qot, Action<UIElement, ModConfig, PropertyFieldWrapper, object, IList, int> previewDrawingMethod)
        => (bool)qot.Call(nameof(OnGlobalConfigPreview), previewDrawingMethod);
}