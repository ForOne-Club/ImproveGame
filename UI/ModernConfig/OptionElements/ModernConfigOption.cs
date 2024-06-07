using ImproveGame.UIFramework.BaseViews;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class ModernConfigOption : View
{
    public ModernConfigOption(ModConfig config, string optionName, int reservedWidth)
    {
        Config = config;
        OptionName = optionName;

        Width.Set(0f, 1f);
        Height.Set(40f, 0f);

        FieldInfo = Config.GetType().GetField(OptionName);
        if (FieldInfo is null)
            throw new Exception($"Field \"{OptionName}\" not found in config \"{Config.GetType().Name}\"");

        RelativeMode = RelativeMode.Vertical;
        Spacing = new Vector2(4);
        OverflowHidden = true;

        SetPadding(4f);
        var labelElement = new OptionLabelElement(config, optionName, reservedWidth);
        labelElement.JoinParent(this);
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
                Height.Set(40, 0f);
                Spacing = new Vector2(4f);
                Parent.Recalculate();
            }
        }
        else
        {
            if (Height.Pixels != 0)
            {
                Height.Set(0, 0f);
                Spacing = Vector2.Zero;
                Parent.Recalculate();
            }
        }

        base.Draw(spriteBatch);
    }

    public string Tooltip => ConfigHelper.GetTooltip(Config, OptionName);

    public FieldInfo FieldInfo { get; }

    public ModConfig Config { get; }

    public string OptionName { get; }
}