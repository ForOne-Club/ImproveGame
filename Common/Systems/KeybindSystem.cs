using System.Reflection;
using static On.Terraria.GameContent.UI.Elements.UIKeybindingListItem;

namespace ImproveGame.Common.Systems
{
    /// <summary>
    /// 快捷键
    /// </summary>
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind SuperVaultKeybind { get; private set; }
        public static ModKeybind BuffTrackerKeybind { get; private set; }
        public static ModKeybind GrabBagKeybind { get; private set; }

        public override void Load()
        {
            GetFriendlyName += TranslatedFriendlyName;
            SuperVaultKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.HugeInventory", "I");
            BuffTrackerKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.BuffTracker", "NumPad3");
            GrabBagKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.GrabBagLoot", "OemQuotes");
        }

        // 不让我用翻译是吧，我直接给你On掉
        private string TranslatedFriendlyName(orig_GetFriendlyName orig, UIKeybindingListItem item)
        {
            string keybindName = item.GetType().GetField("_keybind", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(item) as string;
            if (keybindName.StartsWith("ImproveGame: $Mods.ImproveGame.Keybind"))
            {
                keybindName = Language.GetTextValue(keybindName["ImproveGame: $".Length..]);
                keybindName = GetText("ModName") + ": " + keybindName;
                return Language.GetTextValue(keybindName);
            }
            return orig.Invoke(item);
        }

        public override void Unload()
        {
            SuperVaultKeybind = null;
            BuffTrackerKeybind = null;
            GrabBagKeybind = null;
        }
    }
}
