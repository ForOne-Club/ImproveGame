using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.Categories;

public sealed class Presets : Category
{
    public override Texture2D GetIcon()
    {
        return ModAsset.PaintWand.Value;
    }

    public override void AddOptions(ConfigOptionsPanel panel)
    {
    }
}