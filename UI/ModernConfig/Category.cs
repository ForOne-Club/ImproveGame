namespace ImproveGame.UI.ModernConfig;

public abstract class Category
{
    public abstract int ItemIconId { get; }

    public abstract string LocalizationKey { get; }

    public abstract void AddOptions(ConfigOptionsPanel panel);

    public string Label => GetText($"ModernConfig.{LocalizationKey}.Label");
    public string Tooltip => GetText($"ModernConfig.{LocalizationKey}.Tooltip");
}

public interface DoNotAutoload;