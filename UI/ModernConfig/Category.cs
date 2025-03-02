using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig;

public abstract class Category
{
    public virtual int ItemIconId => 0;

    public virtual Texture2D GetIcon()
    {
        Main.instance.LoadItem(ItemIconId);
        return TextureAssets.Item[ItemIconId].Value;
    }

    public virtual string LocalizationKey => GetType().Name;

    public abstract void AddOptions(ConfigOptionsPanel panel);

    public virtual string Label => GetText($"ModernConfig.{LocalizationKey}.Label");
    public virtual string Tooltip => GetText($"ModernConfig.{LocalizationKey}.Tooltip");

    public virtual Func<ModConfig, string, bool> CanOptionBeAdded => (config, optionName) => true;
}