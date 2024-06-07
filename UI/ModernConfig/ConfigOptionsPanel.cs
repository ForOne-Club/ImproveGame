using ImproveGame.UI.ModernConfig.OptionElements;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig;

public sealed class ConfigOptionsPanel : SUIPanel
{
    internal static ConfigOptionsPanel Instance;
    private static Category _currentCategory;
    
    public static Category CurrentCategory
    {
        get => _currentCategory;
        set
        {
            if (_currentCategory != value)
            {
                _currentCategory = value;
                Instance.RemoveAllChildren();
                _currentCategory.AddOptions(Instance);
            }
        }
    }

    public ConfigOptionsPanel() : base(Color.Black * 0.4f, Color.Black * 0.4f, 12, 2, false)
    {
        Instance = this;
    }

    public void AddToggle(ModConfig config, string name)
    {
        new OptionToggle(config, name).JoinParent(this);
    }

    public void AddValueSlider(ModConfig config, string name)
    {
        new OptionSlider(config, name).JoinParent(this);
    }
}