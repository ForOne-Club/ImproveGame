using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public interface ConfigOption
{
    public ModConfig Config { get; }

    public string OptionName { get; }
}