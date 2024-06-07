using ImproveGame.Common.Configs;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionToggle : View, ConfigOption
{
    public OptionToggle(ModConfig config, string optionName)
    {
        Config = config;
        OptionName = optionName;

        Width.Set(0f, 1f);
        Height.Set(40f, 0f);

        FieldInfo = Config.GetType().GetField(OptionName);
        if (FieldInfo is null)
            throw new Exception($"Field \"{OptionName}\" not found in config \"{Config.GetType().Name}\"");
        if (FieldInfo.FieldType != typeof(bool))
            throw new Exception($"Field \"{OptionName}\" is not a bool");

        RelativeMode = RelativeMode.Vertical;
        Spacing = new Vector2(4);
        
        if (Enabled)
            _timer.ImmediateOpen();

        SetPadding(4f);
        var labelElement = new OptionLabelElement(config, optionName);
        labelElement.JoinParent(this);
    }

    public override void Update(GameTime gameTime)
    {
        _timer.Update();
        base.Update(gameTime);
        if (Enabled)
            _timer.Open();
        else
            _timer.Close();
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        Enabled = !Enabled;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        Color color = Color.Lerp(UIStyle.SwitchBg, UIStyle.SwitchBgHover, _timer.Schedule);
        Color color2 = Color.Lerp(UIStyle.SwitchBorder, UIStyle.SwitchBorderHover, _timer.Schedule);
        Color color3 = Color.Lerp(UIStyle.SwitchRound, UIStyle.SwitchRoundHover, _timer.Schedule);

        var dimensions = GetDimensions();
        var position = dimensions.Position();
        var center = dimensions.Center();
        var size = dimensions.Size();

        // 背景板
        var panelColor = IsMouseHovering ? UIStyle.PanelBgLightHover : UIStyle.PanelBgLight;
        SDFRectangle.NoBorder(position, size, new Vector4(8f), panelColor);

        // 开关
        var boxSize = new Vector2(48, 26);
        var boxPosition = new Vector2(position.X + size.X - boxSize.X - 6f, position.Y);

        Vector2 position1 = boxPosition + new Vector2(0, size.Y / 2 - boxSize.Y / 2);
        SDFRectangle.HasBorder(position1, boxSize, new Vector4(MathF.Min(boxSize.X, boxSize.Y) / 2), color, 2, color2);

        Vector2 boxSize2 = new(boxSize.Y - 10);
        Vector2 position2 = boxPosition + Vector2.Lerp(new Vector2(3 + 2, size.Y / 2 - boxSize2.Y / 2),
            new Vector2(boxSize.X - 3 - 2 - boxSize2.X, size.Y / 2 - boxSize2.Y / 2), _timer.Schedule);
        SDFGraphics.NoBorderRound(position2, boxSize2.X, color3);

        // 提示
        if (IsMouseHovering)
        {
            UICommon.TooltipMouseText(Tooltip);
        }
    }

    private readonly AnimationTimer _timer = new (4);

    private string Tooltip => ConfigHelper.GetTooltip(Config, OptionName);

    public bool Enabled
    {
        get => (bool)FieldInfo.GetValue(Config);
        set => ConfigHelper.SetConfigValue(Config, FieldInfo, value);
    }

    public FieldInfo FieldInfo { get; }
    public ModConfig Config { get; }
    public string OptionName { get; }
}