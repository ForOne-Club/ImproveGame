using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig.OptionElements;

public class OptionLabelElement(ModConfig config, string optionName, int reservedWidth = 60, string forcedName = null)
    : SlideText(ConvertLeftRight(forcedName ?? Language.GetTextValue($"Mods.{config.Mod.Name}.Configs.{config.GetType().Name}.{optionName}.Label")), reservedWidth);