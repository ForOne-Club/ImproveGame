using ImproveGame.Common.Configs;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.UI;

namespace ImproveGame.Common.ModSystems;

/// <summary>
/// 快捷键
/// </summary>
public class KeybindSystem : ModSystem
{
    internal static bool UseKeybindTranslation = true;

    public static ModKeybind MasterControlKeybind { get; private set; }
    public static ModKeybind SuperVaultKeybind { get; private set; }
    public static ModKeybind BuffTrackerKeybind { get; private set; }
    public static ModKeybind OpenBagKeybind { get; private set; }
    public static ModKeybind GrabBagKeybind { get; private set; }
    public static ModKeybind HotbarSwitchKeybind { get; private set; }
    public static ModKeybind AutoTrashKeybind { get; private set; }
    public static ModKeybind DiscordRodKeybind { get; private set; }
    public static ModKeybind HomeKeybind { get; private set; }

    private static readonly Dictionary<string, string> ZhTranslationKeybind = new()
    {
        {"Mouse1", "鼠标左键" }, {"Mouse2", "鼠标右键" }, {"Mouse3", "鼠标中键" },
        {"Mouse4", "鼠标侧键1" }, {"Mouse5", "鼠标侧键2" }, {"Space", "空格" },
        {"Escape", "Esc键" }, { "Back", "退格" }, {"Enter", "回车" },
        {"LeftShift", "左Shift" }, {"LeftControl", "左Ctrl" }, {"LeftAlt", "左Alt" },
        {"RightShift", "右Shift" }, {"RightControl", "右Ctrl" }, {"RightAlt", "右Alt" },
        {"VolumeUp", "提高音量"}, {"VolumeDown", "减少音量"},
        {"Divide", "小键盘 /" }, {"Add", "小键盘 +" },
        {"Subtract", "小键盘 -" }, {"Multiply", "小键盘 *" },
        {"OemComma", "< ," }, {"OemPeriod", "> ." }, {"OemQuestion", "? /" },
        {"OemSemicolon", ": ;" }, {"OemQuotes", "\" \'" }, {"OemPipe", "| \\" },
        {"OemOpenBrackets", "[ {" }, {"OemCloseBrackets", "] }" },
        {"OemPlus", "+ =" }, {"OemMinus", "- _" }, {"OemTilde", "~ `"}
    };

    public override void Load()
    {
        On_UIKeybindingListItem.DrawSelf += DrawHoverText;
        On_UIKeybindingListItem.GenInput += TranslatedInput;
        On_UIManageControls.CreateBindingGroup += AddModifyTip;
        MasterControlKeybind = KeybindLoader.RegisterKeybind(Mod, "MasterControl", "OemTilde");
        AutoTrashKeybind = KeybindLoader.RegisterKeybind(Mod, "AutoTrashKeybind", "NumPad1");
        BuffTrackerKeybind = KeybindLoader.RegisterKeybind(Mod, "BuffTracker", "NumPad2");
        OpenBagKeybind = KeybindLoader.RegisterKeybind(Mod, "OpenBagGUI", "NumPad3");
        SuperVaultKeybind = KeybindLoader.RegisterKeybind(Mod, "HugeInventory", "F");
        GrabBagKeybind = KeybindLoader.RegisterKeybind(Mod, "GrabBagLoot", "OemQuotes");
        HotbarSwitchKeybind = KeybindLoader.RegisterKeybind(Mod, "HotbarSwitch", "OemQuestion");
        DiscordRodKeybind = KeybindLoader.RegisterKeybind(Mod, "DiscordRodKeybind", "U");
        HomeKeybind = KeybindLoader.RegisterKeybind(Mod, "HomeKeybind", "Home");
    }

    private void DrawHoverText(On_UIKeybindingListItem.orig_DrawSelf orig, UIKeybindingListItem self, SpriteBatch spriteBatch)
    {
        orig.Invoke(self, spriteBatch);

        if (!self.IsMouseHovering)
            return;

        string key = "";
        switch (self._keybind)
        {
            case "ImproveGame/MasterControl":
                key = "MasterControl";
                break;
            case "ImproveGame/HotbarSwitch":
                key = "HotbarSwitch";
                break;
            case "ImproveGame/BuffTracker":
                key = "BuffTracker";
                break;
        }
        
        if (key is "")
            return;
        
        var localizedText = Language.GetOrRegister($"Mods.ImproveGame.Keybinds.{key}.Tip", () => "");
        UICommon.TooltipMouseText(localizedText.Value);
    }

    private string TranslatedInput(On_UIKeybindingListItem.orig_GenInput orig, UIKeybindingListItem self, List<string> list)
    {
        if (UseKeybindTranslation && Language.ActiveCulture.Name == "zh-Hans")
        {
            var displayTexts = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                string text = list[i].Replace("NumPad", "小键盘 ");
                if (ZhTranslationKeybind.TryGetValue(list[i], out string translatedString))
                {
                    text = translatedString;
                }

                displayTexts.Add(text);
            }

            return orig.Invoke(self, displayTexts);
        }

        return orig.Invoke(self, list);
    }

    private UISortableElement AddModifyTip(On_UIManageControls.orig_CreateBindingGroup orig,
        UIManageControls self, int elementIndex, List<string> bindings,
        Terraria.GameInput.InputMode currentInputMode)
    {
        var uISortableElement = orig.Invoke(self, elementIndex, bindings, currentInputMode);
        if (Language.ActiveCulture.Name == "zh-Hans" && elementIndex == 0)
        {
            string str = "“更好的体验”提供了键位汉化";
            UIText text = new(str, 0.8f)
            {
                VAlign = 0f,
                HAlign = 0f
            };
            uISortableElement.Append(text);

            UIPanel button = new()
            {
                Width = new(100f, 0f),
                Height = new(30f, 0f),
                Left = new(-100f, 1f),
                Top = new(0f, 0f)
            };
            UIText buttonText = new("Ima your father", 0.8f)
            {
                VAlign = 0.5f,
                HAlign = 0.5f
            };
            button.OnLeftMouseDown += (_, _) =>
            {
                UseKeybindTranslation = !UseKeybindTranslation;
                AdditionalConfig.Save();
                SoundEngine.PlaySound(SoundID.MenuTick);
            };
            button.OnUpdate += _ => buttonText.SetText(UseKeybindTranslation ? "快换回去" : "让我看看");
            button.Append(buttonText);
            uISortableElement.Append(button);

            uISortableElement.Recalculate();
        }

        return uISortableElement;
    }

    public override void Unload()
    {
        SuperVaultKeybind = null;
        BuffTrackerKeybind = null;
        GrabBagKeybind = null;
    }
}