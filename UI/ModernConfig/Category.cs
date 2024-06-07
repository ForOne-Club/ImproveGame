namespace ImproveGame.UI.ModernConfig;

public abstract class Category
{
    public abstract int ItemIconId { get; }

    public abstract string LocalizationKey { get; }

    public abstract void AddOptions(ConfigOptionsPanel panel);
}