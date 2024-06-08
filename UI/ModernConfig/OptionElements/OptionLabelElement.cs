using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionLabelElement (ModConfig config, string optionName, int reservedWidth = 60)
    : SlideText(GetText($"Configs.{config.GetType().Name}.{optionName}.Label"), reservedWidth);