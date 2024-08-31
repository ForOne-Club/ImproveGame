using ImproveGame.Common.Configs.FavoritedSystem;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class ModernConfigOption : TimerView
{
    public ModernConfigOption(ModConfig config, string optionName, int reservedWidth)
    {
        Config = config;
        OptionName = optionName;

        Width.Set(0f, 1f);
        Height.Set(46f, 0f);
        Rounded = new Vector4(12f);
        SetPadding(12, 4);

        FieldInfo = Config.GetType().GetField(OptionName);
        if (FieldInfo is null)
            throw new Exception($"Field \"{OptionName}\" not found in config \"{Config.GetType().Name}\"");

        RelativeMode = RelativeMode.Vertical;
        OverflowHidden = true;

        var labelElement = new OptionLabelElement(config, optionName, reservedWidth)
        {
            RelativeMode = RelativeMode.None
        };
        labelElement.OnUpdate += _ =>
        {
            labelElement.TextColor = MarkedAsFavorite
                ? Color.Yellow
                : Color.White;
        };
        labelElement.JoinParent(this);
        CheckAttributes();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var displayConditionAttribute = FieldInfo.GetCustomAttribute<DisplayConditionAttribute>();
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

        TooltipPanel.SetText(Tooltip);

        // 不可控制，为什么呢？
        if (Interactable)
            return;

        if (ReloadRequired)
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
        var defaultValueAttribute = FieldInfo.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValueAttribute != null)
        {
            ConfigHelper.SetConfigValue(Config, FieldInfo, defaultValueAttribute.Value);
            SoundEngine.PlaySound(SoundID.Chat);
        }
    }

    public override void MiddleMouseDown(UIMouseEvent evt)
    {
        base.MiddleMouseDown(evt);
        FavoritedOptionDatabase.ToggleFavoriteForOption(Config, OptionName);

        if (ConfigOptionsPanel.CurrentCategory.LocalizationKey is nameof(Favorites))
            ConfigOptionsPanel.Instance.RefreshCurrentPage();
    }

    private void CheckAttributes()
    {
        ReloadRequired = FieldInfo.GetCustomAttribute<ReloadRequiredAttribute>() is not null;

        var rangeAttribute = FieldInfo.GetCustomAttribute<RangeAttribute>();
        if (rangeAttribute is {Min: int, Max: int })
        {
            Max = (int)rangeAttribute.Max;
            Min = (int)rangeAttribute.Min;
        }

        if (rangeAttribute is {Min: float, Max: float })
        {
            Max = (float)rangeAttribute.Max;
            Min = (float)rangeAttribute.Min;
        }

        if (rangeAttribute is {Min: double, Max: double })
        {
            Max = (double)rangeAttribute.Max;
            Min = (double)rangeAttribute.Min;
        }

        var defaultValueAttribute = FieldInfo.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValueAttribute is {Value: int })
        {
            Default = (int)defaultValueAttribute.Value;
        }

        if (defaultValueAttribute is {Value: float })
        {
            Default = (float)defaultValueAttribute.Value;
        }

        if (defaultValueAttribute is {Value: double })
        {
            Default = (double)defaultValueAttribute.Value;
        }
    }

    /// <summary>
    /// 是否被高光显示，用于搜索
    /// </summary>
    public bool Highlighted;

    protected bool Interactable => !CantOperateInGame || Main.gameMenu;

    public string Label => ConfigHelper.GetLabel(Config, OptionName);
    public string Tooltip => ConfigHelper.GetTooltip(Config, OptionName);

    public FieldInfo FieldInfo { get; }
    public ModConfig Config { get; }
    public string OptionName { get; }

    internal double Min = 0;
    internal double Max = 1;
    internal double Default = 1;
    internal bool ReloadRequired;

    private bool CantOperateDueToHostVerification =>
        Config.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient &&
        MyUtils.Config.OnlyHost && !Main.countsAsHostForGameplay[Main.myPlayer];

    private bool CantOperateDueToPasswordVerification =>
        Config.Mode is ConfigScope.ServerSide && Main.netMode is NetmodeID.MultiplayerClient &&
        MyUtils.Config.OnlyHostByPassword && !NetPasswordSystem.LocalPlayerRegistered;

    private bool CantOperateInGame =>
        ReloadRequired || CantOperateDueToPasswordVerification || CantOperateDueToHostVerification;

    private bool MarkedAsFavorite =>
        FavoritedOptionDatabase.FavoritedOptions.Contains($"{Config.Name}.{OptionName}") &&
        ConfigOptionsPanel.CurrentCategory.LocalizationKey is not nameof(Favorites);
}