using FullSerializer;
using ImproveGame.Common.Configs.FavoritedSystem;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using tModPorter;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class ModernConfigOption : TimerView
{
    public static UIElement WrapIt(UIElement parent, ref int top, ModConfig modConfig, PropertyFieldWrapper variable, object item, int order, object list = null, Type arrayType = null, int index = -1)
    {
        Type type = variable.Type;
        ModernConfigOption option;
        bool unsupported = false;
        if (type == typeof(bool))
            option = new OptionToggle(modConfig, variable);
        else if (OptionSlider.SupportedTypes.Contains(type))
            option = new OptionSlider(modConfig, variable);
        else if (type.IsEnum)
            option = new OptionDropdownList(modConfig, variable);
        else if (type == typeof(string))
            option = new OptionEditableText(modConfig, variable);
        else if (type.IsArray)
        {
            option = new OptionNotSupportText(modConfig, variable);
            unsupported = true;
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            option = new OptionNotSupportText(modConfig, variable);
            unsupported = true;
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
        {
            option = new OptionNotSupportText(modConfig, variable);
            unsupported = true;
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            option = new OptionNotSupportText(modConfig, variable);
            unsupported = true;
        }
        else
        {
            option = new OptionObject(modConfig, variable);
            //unsupported = true;
        }
        if (!unsupported)
        {
            option.index = index;
            option.List = (IList)list;
            option.Item = item;
            option.path = [];
            if (parent is ModernConfigOption parentOption)
            {
                if (parentOption.path != null)
                    option.path.AddRange(parentOption.path);
                option.path.Add(parentOption.VariableInfo.Name);
            }
            option.OnBind2();
        }

        option.JoinParent(parent);

        return option;
    }


    public ModernConfigOption(ModConfig config, string optionName, int reservedWidth, object ownerItem = null)
    {
        Config = config;
        OptionName = optionName;
        var fieldInfo = config.GetType().GetField(OptionName);
        var propertyInfo = config.GetType().GetProperty(OptionName);
        if (fieldInfo != null)
            VariableInfo = new PropertyFieldWrapper(fieldInfo);
        else if (propertyInfo != null)
            VariableInfo = new PropertyFieldWrapper(propertyInfo);
        if (VariableInfo is null)
            throw new Exception($"Field \"{OptionName}\" not found in config \"{config.GetType().Name}\"");
        Item = ownerItem ?? config;
        OnBind(config, optionName, reservedWidth);
    }

    public ModernConfigOption(ModConfig config, PropertyFieldWrapper propertyFieldWrapper, int reservedWidth, object ownerItem = null)
    {
        Config = config;
        OptionName = propertyFieldWrapper.Name;
        VariableInfo = propertyFieldWrapper;
        Item = ownerItem ?? config;
        OnBind(config, OptionName, reservedWidth);
    }

    protected virtual void OnBind(ModConfig config, string optionName, int reservedWidth)
    {
        RelativeMode = RelativeMode.Vertical;
        OverflowHidden = true;
        Width.Set(0f, 1f);
        Height.Set(46f, 0f);
        Rounded = new Vector4(12f);
        SetPadding(12, 4);
        var labelElement = new OptionLabelElement(config, optionName, reservedWidth, Label)
        {
            RelativeMode = RelativeMode.None
        };
        labelElement.OnUpdate += _ =>
        {
            labelElement.TextColor = MarkedAsFavorite
                ? Color.Gold
                : Color.White;

        };
        labelElement.JoinParent(this);
        CheckAttributes();

    }
    protected virtual void OnBind2() 
    {

    }
    protected void SetValueDirect(object value)
    {
        if (!Interactable) return;
        ConfigHelper.SetConfigValue(Config, VariableInfo, value, Item, path: path);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        var displayConditionAttribute = VariableInfo.MemberInfo.GetCustomAttribute<DisplayConditionAttribute>();
        if (displayConditionAttribute == null)
        {
            base.Draw(spriteBatch);
            return;
        }

        if (displayConditionAttribute.IsVisible)
        {
            if (Height.Pixels != 40)
            {
                Height.Set(44, 0f);
                Parent.Recalculate();
            }
        }
        else
        {
            if (Height.Pixels != 0)
            {
                Height.Set(0, 0f);
                Parent.Recalculate();
            }
        }

        base.Draw(spriteBatch);
    }

    // 为了让UI之间实际上无间隔，防止鼠标滑过时Tooltip文字闪现，这里重写绘制，而不使用Spacing
    public override void DrawSelf(SpriteBatch spriteBatch)
    {

        var dimensions = GetDimensions();
        var dimensionsRect = dimensions.ToRectangle();
        var position = dimensions.Position();
        var size = dimensions.Size();

        // 这里修改这两个值，而不使用Spacing
        position.Y += 3f;
        size.Y -= 6f;

        // 背景板
        //var panelColor = HoverTimer.Lerp(UIStyle.PanelBgLight, UIStyle.PanelBgLightHover);
        Color panelColor;
        if (BgColor != Color.Transparent)
            panelColor = HoverTimer.Lerp(BgColor.MultiplyRGBA(new Color(180, 180, 180)), BgColor);
        else
            panelColor = HoverTimer.Lerp(UIStyle.PanelBgLight, UIStyle.PanelBgLightHover);
        if (!Interactable)
            panelColor = Color.Gray * 0.3f;

        if (Highlighted)
        {
            SDFRectangle.HasBorder(position, size, new Vector4(8f), panelColor, 2f, UIStyle.ItemSlotBorderFav * 0.8f);
        }
        else
        {
            SDFRectangle.NoBorder(position, size, new Vector4(8f), panelColor * 0.8f);
        }

        // 提示
        if (!IsMouseHovering)
            return;

        string text = "";
        if (ReloadRequired)
            text += $" - [c/{Color.Orange.Hex3()}:{Language.GetTextValue("tModLoader.ModReloadRequiredMemberTooltip")}]\n";
        text += Tooltip;
        TooltipPanel.SetText(text);
        // 不可控制，为什么呢？

        bool f = CantOperateDueToOnlyGetter;

        if (f)
        {
            string readOnlyTip = GetText("Configs.ModernConfig.ReadOnlyTip");
            UICommon.TooltipMouseText(readOnlyTip);
        }
        if (Interactable)
            return;

        if (CantOperateDueToOnlyGetter)
        {
            string readOnlyTip = GetText("Configs.ModernConfig.ReadOnlyTip");
            UICommon.TooltipMouseText(readOnlyTip);
        }
        else if (ReloadRequired)
        {
            string reloadTip =
                Language.GetTextValue("tModLoader.ModConfigCantSaveBecauseChangesWouldRequireAReload");
            UICommon.TooltipMouseText(reloadTip);
        }
        else if (CantOperateDueToHostVerification)
        {
            string hostTip = GetText("Configs.ImproveConfigs.OnlyHost.Tips");
            UICommon.TooltipMouseText(hostTip);
        }
        else if (CantOperateDueToPasswordVerification)
        {
            string passwordTip = GetText("Configs.ImproveConfigs.OnlyHostByPassword.Tips");
            UICommon.TooltipMouseText(passwordTip);
        }
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        base.RightMouseDown(evt);
        var defaultValueAttribute = VariableInfo.MemberInfo.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValueAttribute != null)
        {
            SetValueDirect(defaultValueAttribute.Value);
            //ConfigHelper.SetConfigValue(Config, VariableInfo, defaultValueAttribute.Value, Item, path: path);
            SoundEngine.PlaySound(SoundID.Chat);
        }
    }

    public override void MiddleMouseDown(UIMouseEvent evt)
    {
        base.MiddleMouseDown(evt);
        FavoritedOptionDatabase.ToggleFavoriteForOption(Config, OptionName);

        if (ConfigOptionsPanel.CurrentCategory.LocalizationKey is nameof(Favorites))
            ConfigOptionsPanel.Instance.RefreshCurrentPage();

        // 特效
        SoundEngine.PlaySound(FavoritedOptionDatabase.IsFavorited(Config, OptionName)
            ? SoundID.ResearchComplete
            : SoundID.Research);
        ModernConfigUI.Instance.GenerateParticleAtMouse();
    }

    protected virtual void CheckAttributes()
    {
        //我把只有输入框那个删了，然后Min这些值交由Slider处理，毕竟只有它用得到这些
        ReloadRequired = VariableInfo.MemberInfo.GetCustomAttribute<ReloadRequiredAttribute>() is not null;

        var colorAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<BackgroundColorAttribute>(VariableInfo, Item, List);
        if (colorAttribute != null)
            BgColor = colorAttribute.Color;
    }

    /// <summary>
    /// 是否被高光显示，用于搜索
    /// </summary>
    public bool Highlighted;

    protected bool Interactable => !CantOperateDueToOnlyGetter && (!CantOperateInGame || Main.gameMenu);

    public string Label => List != null ? index.ToString() : (ConfigManager.GetLocalizedText<LabelKeyAttribute, LabelArgsAttribute>(VariableInfo, "Label") ?? ConfigHelper.GetLabel(Config, OptionName));
    public string Tooltip => ConfigManager.GetLocalizedText<TooltipKeyAttribute, TooltipArgsAttribute>(VariableInfo, "Tooltip") ?? ConfigHelper.GetTooltip(Config, OptionName);

    //public FieldInfo FieldInfo { get; }
    public PropertyFieldWrapper VariableInfo { get; }

    public ModConfig Config { get; }
    public string OptionName { get; }

    internal bool ReloadRequired;
    internal LabelKeyAttribute LabelKeyAttribute;
    internal TooltipKeyAttribute TooltipKeyAttribute;


    private bool CantOperateDueToHostVerification =>
        Config.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient &&
        MyUtils.Config.OnlyHost && !Main.countsAsHostForGameplay[Main.myPlayer];

    private bool CantOperateDueToPasswordVerification =>
        Config.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient &&
        MyUtils.Config.OnlyHostByPassword && !NetPasswordSystem.LocalPlayerRegistered;

    private bool CantOperateDueToOnlyGetter =>
        !VariableInfo.CanWrite;

    private bool CantOperateInGame =>
        ReloadRequired || CantOperateDueToPasswordVerification || CantOperateDueToHostVerification;

    private bool MarkedAsFavorite =>
        FavoritedOptionDatabase.FavoritedOptions.Contains($"{Config.Name}.{OptionName}") &&
        ConfigOptionsPanel.CurrentCategory.LocalizationKey is not nameof(Favorites);

    // 是的是的，我不仅把原版那一套抄了一点过来让一切变得更混乱，还自己加了点更奇怪的东西
    /// <summary>
    /// 指向的目标所属的List
    /// </summary>
    public IList List { get; set; }
    /// <summary>
    /// List中的索引
    /// </summary>
    public int index;
    /// <summary>
    /// 当前设置块指向的目标
    /// <br>为了和object等适配，不能直接在config改了</br>
    /// </summary>
    public object Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }
    object item;
    /// <summary>
    /// 导航到目标字段的路径，具体ctrl点进来看注释
    /// </summary>
    public List<string> path;
    // 到当前目标的字段/属性路径  直接在某config下是null 在它的字段myField下是["myField"]，再在字段myField2下是["myField","myField2"]，依此类推
    // 是为了和联机同步那边的代码实现妥协的产物，那边之前直接是给config的某个字段设置就直接很多，这里不得不记录下字段路径了
    // 原版的做法似乎是直接把整个config都重新写入了一遍？
}