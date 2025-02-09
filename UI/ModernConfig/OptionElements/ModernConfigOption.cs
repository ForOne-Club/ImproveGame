using ImproveGame.Common.Configs;
using ImproveGame.Common.Configs.FavoritedSystem;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class ModernConfigOption : TimerView
{
    public static ModernConfigOption WrapIt(UIElement parent, ModConfig modConfig, PropertyFieldWrapper variable, object item, object list = null, Type arrayType = null, int index = -1, ModernConfigOption owner = null)
    {
        Type type = variable.Type;
        if (arrayType != null)
        {
            type = arrayType;
        }
        ModernConfigOption option;
        if (type == typeof(bool))
            option = new OptionToggle();
        else if (OptionSlider.SupportedTypes.Contains(type))
            option = new OptionSlider();
        else if (type == typeof(Vector2))
            option = new OptionVector2();
        else if(type == typeof(Color))
            option = new OptionColor(); 
        else if (type.IsEnum)
            option = new OptionDropdownList();
        else if (type == typeof(string))
        {
            var ost = ConfigManager.GetCustomAttributeFromMemberThenMemberType<OptionStringsAttribute>(variable, item, list);
            option = ost != null ? new OptionDropdownList() : new OptionEditableText();
        }
        else if (type.IsArray)
            option = new OptionArray();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            option = new OptionList();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
            option = new OptionHashSet();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            option = new OptionDictionary();
        else if (type.IsSubclassOf(typeof(EntityDefinition)))
            option = new OptionDefinition();
        else
            option = new OptionObject();

        option.index = index;
        option.List = (IList)list;
        option.Item = item;
        option.path = [];
        if (owner != null)
        {
            if (owner.path != null)
                option.path.AddRange(owner.path);
            if (owner.List != null)
                option.path.Add(owner.index.ToString());
            else
                option.path.Add(owner.VariableInfo.Name);
            option.owner = owner;
        }
        /*if (parent is ModernConfigOption parentOption)
        {
            if (parentOption.path != null)
                option.path.AddRange(parentOption.path);
            option.path.Add(parentOption.VariableInfo.Name);
        }*/
        option.Bind(modConfig, variable);
        /*if (parent.Parent.Parent is SUIScrollViewDraggingSortable scroll)
            scroll.AddToList(option);
        else*/
            option.JoinParent(parent);

        return option;
    }
    public static PropertyFieldWrapper GetWrapper(Type type, string optionName)
    {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.Public;
        var fieldInfo = type.GetField(optionName, flag);
        var propertyInfo = type.GetProperty(optionName, flag);
        PropertyFieldWrapper result = null;
        if (fieldInfo != null)
            result = new PropertyFieldWrapper(fieldInfo);
        else if (propertyInfo != null)
            result = new PropertyFieldWrapper(propertyInfo);
        else
            throw new Exception($"Field \"{optionName}\" not found in type \"{type.Name}\"");
        return result;
    }

    public virtual int labelReservedWidth => 70;
    public Type VarType => List != null ? List[index].GetType() : VariableInfo.Type;
    //原来的构造函数改成OnBind了
    //因为现在要构造出来另外赋一些值再Bind，都写构造函数太杂乱了
    public void Bind(ModConfig config, PropertyFieldWrapper propertyFieldWrapper)
    {
        Config = config;
        OptionName = propertyFieldWrapper.Name;
        VariableInfo = propertyFieldWrapper;
        Item ??= ConfigOptionsPanel.GlobalItem ?? config;

        RelativeMode = RelativeMode.Vertical;
        OverflowHidden = true;
        Width.Set(0f, 1f);
        Height.Set(46f, 0f);
        Rounded = new Vector4(12f);
        SetPadding(12, 4);
        var labelElement = new OptionLabelElement(config, OptionName, labelReservedWidth, Label)
        {
            RelativeMode = RelativeMode.None
        };
        labelElement.OnUpdate += _ =>
        {
            labelElement.TextColor = MarkedAsFavorite
                ? Color.Gold
                : Color.White;
            if (ReloadRequired)
                labelElement.DisplayText = labelElement.OriginLabel() + (ValueChanged ? $" - [c/FF0000:{Language.GetTextValue("tModLoader.ModReloadRequired")}]" : "");
            else if (labelElement.DisplayText == "")
                labelElement.DisplayText = labelElement.OriginLabel();

        };
        labelElement.JoinParent(this);
        CheckAttributes();
        OnBind();
    }

    protected virtual void OnBind()
    {


    }
    protected void SetValueDirect(object value)
    {
        if (!Interactable) return;


        if (item.GetType().IsValueType)//VariableInfo.Type
        {
            VariableInfo.SetValue(item, value);
            owner?.SetValueDirect(item);
        }
        else
            ConfigHelper.SetConfigValue(Config, VariableInfo, value, Item, path: path, List: List, Index: index);

    }
    protected T GetAttribute<T>() where T : Attribute => ConfigManager.GetCustomAttributeFromMemberThenMemberType<T>(VariableInfo, Item, List);
    protected object GetValue()
    {
        if (List != null)
            return List[index];

        return VariableInfo.GetValue(Item);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        var displayConditionAttribute = VariableInfo.MemberInfo.GetCustomAttribute<DisplayConditionAttribute>();
        if (displayConditionAttribute == null)
        {
            base.Draw(spriteBatch);
            DrawDebugText(spriteBatch);
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

        if (displayConditionAttribute.IsVisible)
            DrawDebugText(spriteBatch);
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
            string readOnlyTip = GetText("ModernConfig.ReadOnlyTip");
            UICommon.TooltipMouseText(readOnlyTip);
        }
        if (Interactable)
            return;

        if (CantOperateDueToOnlyGetter)
        {
            string readOnlyTip = GetText("ModernConfig.ReadOnlyTip");
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

    private void DrawDebugText(SpriteBatch spriteBatch)
    {
        if (!UIConfigs.Instance.ShowMoreData)
            return;
        
        var dimensions = GetDimensions();
        var dimensionsRect = dimensions.ToRectangle();
        var position = dimensions.Position();
        var size = dimensions.Size();
        
        // 文字
        var text = DebugText ?? "";
        var textPosition = dimensionsRect.Top();
        textPosition.Y += 6;
        textPosition.X -= 50;

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, textPosition,
            Color.Gray, Color.Black, 0f, Vector2.Zero, new Vector2(0.8f), -1f, 1f);
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        base.RightMouseDown(evt);
        if (evt.Target != this) return;
        var defaultValueAttribute = GetAttribute<DefaultValueAttribute>();
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
        if (evt.Target != this) return;
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
        ReloadRequired = GetAttribute<ReloadRequiredAttribute>() is not null;
        if (ReloadRequired && List == null && Item is ModConfig modConfig)
        {
            ModConfig loadTimeConfig = ConfigManager.GetLoadTimeConfig(modConfig.Mod, modConfig.Name);
            OldValue = VariableInfo.GetValue(loadTimeConfig);
        }
        var colorAttribute = GetAttribute<BackgroundColorAttribute>();
        if (colorAttribute != null)
            BgColor = colorAttribute.Color;
    }

    /// <summary>
    /// 是否被高光显示，用于搜索
    /// </summary>
    public bool Highlighted;

    protected bool Interactable => !CantOperateDueToOnlyGetter && (!CantOperateInGame || Main.gameMenu);

    public string Label => List != null ? (index + 1).ToString() : (ConfigManager.GetLocalizedText<LabelKeyAttribute, LabelArgsAttribute>(VariableInfo, "Label") ?? ConfigHelper.GetLabel(Config, OptionName));
    public string Tooltip => ConfigManager.GetLocalizedText<TooltipKeyAttribute, TooltipArgsAttribute>(VariableInfo, "Tooltip") ?? ConfigHelper.GetTooltip(Config, OptionName);

    //public FieldInfo FieldInfo { get; }
    public PropertyFieldWrapper VariableInfo { get; private set; }
    public string DebugText { get; set; }
    public ModConfig Config { get; private set; }
    public string OptionName { get; private set; }

    internal bool ReloadRequired;
    internal LabelKeyAttribute LabelKeyAttribute;
    internal TooltipKeyAttribute TooltipKeyAttribute;



    private bool CantOperateDueToHostVerification =>
        Config.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient &&
        (Config is not ImproveConfigs configs || configs.OnlyHost) && !Main.countsAsHostForGameplay[Main.myPlayer];

    private bool CantOperateDueToPasswordVerification =>
        Config.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient &&
        (Config is not ImproveConfigs configs || configs.OnlyHostByPassword) && !NetPasswordSystem.LocalPlayerRegistered;

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
        get => item;
        set => item = value;
    }
    object item;
    /// <summary>
    /// 导航到目标字段的路径，具体ctrl点进来看注释
    /// </summary>
    public List<string> path;
    // 到当前目标的字段/属性路径  直接在某config下是null 在它的字段myField下是["myField"]，再在字段myField2下是["myField","myField2"]，依此类推
    // 是为了和联机同步那边的代码实现妥协的产物，那边之前直接是给config的某个字段设置就直接很多，这里不得不记录下字段路径了
    // 原版的做法似乎是直接把整个config都重新写入了一遍？
    public ModernConfigOption owner;//当前选项所属的设置选项
    object OldValue;
    protected bool ValueChanged => !ConfigManager.ObjectEquals(OldValue, GetValue());

}