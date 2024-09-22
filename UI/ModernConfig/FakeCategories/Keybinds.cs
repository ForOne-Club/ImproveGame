using ImproveGame.UI.ModernConfig.OptionElements;

namespace ImproveGame.UI.ModernConfig.FakeCategories;

public class Keybinds : Category
{
    // 为没有给总控绑定快捷键的玩家高亮显示总控
    public static bool HighlightMasterControl;
    
    public override int ItemIconId => ItemID.RainbowCursor;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        if (Language.ActiveCulture.Name == "zh-Hans")
        {
            panel.AddToOptionsDirect(new KeybindChineseToggle());
        }

        foreach (var modKeybind in KeybindLoader.Keybinds)
        {
            if (modKeybind.Mod.Name == ImproveGame.Instance.Name)
            {
                panel.AddToOptionsDirect(new OptionKeybind(modKeybind.FullName));
            }
        }
    }
}