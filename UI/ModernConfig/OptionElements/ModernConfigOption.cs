using ImproveGame.Common.Configs.FavoritedSystem;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class ModernConfigOption : TimerView
{
    public ModernConfigOption(ModConfig config, PropertyFieldWrapper propertyFieldWrapper, int reservedWidth)
    {
        Config = config;
        OptionName = propertyFieldWrapper.Name;

        Width.Set(0f, 1f);
        Height.Set(46f, 0f);
        Rounded = new Vector4(12f);
        SetPadding(12, 4);

        VariableInfo = propertyFieldWrapper;

        RelativeMode = RelativeMode.Vertical;
        OverflowHidden = true;

        CheckAttributes();

        var labelElement = new OptionLabelElement(config, OptionName, reservedWidth)
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
    }
    public ModernConfigOption(ModConfig config, string optionName, int reservedWidth)
    {
        Config = config;
        OptionName = optionName;

        Width.Set(0f, 1f);
        Height.Set(46f, 0f);
        Rounded = new Vector4(12f);
        SetPadding(12, 4);

        var fieldInfo = Config.GetType().GetField(OptionName);
        var propertyInfo = Config.GetType().GetProperty(OptionName);
        if(fieldInfo != null)
            VariableInfo = new PropertyFieldWrapper(fieldInfo);
        else if(propertyInfo != null)
            VariableInfo = new PropertyFieldWrapper(propertyInfo);

        if (VariableInfo is null)
            throw new Exception($"Field \"{OptionName}\" not found in config \"{Config.GetType().Name}\"");

        RelativeMode = RelativeMode.Vertical;
        OverflowHidden = true;

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
        var panelColor = HoverTimer.Lerp(UIStyle.PanelBgLight, UIStyle.PanelBgLightHover);
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
        if(ReloadRequired)
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
            ConfigHelper.SetConfigValue(Config, VariableInfo, defaultValueAttribute.Value);
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
    }

    /// <summary>
    /// 是否被高光显示，用于搜索
    /// </summary>
    public bool Highlighted;

    protected bool Interactable => !CantOperateDueToOnlyGetter && (!CantOperateInGame || Main.gameMenu) ;

    public string Label => ConfigManager.GetLocalizedText<LabelKeyAttribute, LabelArgsAttribute>(VariableInfo, "Label") ?? ConfigHelper.GetLabel(Config, OptionName);
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
}