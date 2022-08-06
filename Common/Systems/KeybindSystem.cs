using System.Collections.Generic;
using System.Reflection;
using Terraria.UI.Chat;
using static On.Terraria.GameContent.UI.Elements.UIKeybindingListItem;
using static On.Terraria.GameContent.UI.States.UIManageControls;

namespace ImproveGame.Common.Systems
{
    /// <summary>
    /// 快捷键
    /// </summary>
    public class KeybindSystem : ModSystem
    {
        internal static bool UseKeybindTranslation = true;

        public static ModKeybind SuperVaultKeybind { get; private set; }
        public static ModKeybind BuffTrackerKeybind { get; private set; }
        public static ModKeybind GrabBagKeybind { get; private set; }
        public static ModKeybind HotbarSwitchKeybind { get; private set; }

        // 为各种Mod控件特制的翻译
        private readonly static Dictionary<string, string> zhTranslationSupports = new()
        {
            {"CalamityMod: Normality Relocator", "Calamity Mod (灾厄): 常态定位仪" },
            {"CalamityMod: Rage Mode", "Calamity Mod (灾厄): 怒气模式" },
            {"CalamityMod: Adrenaline Mode", "Calamity Mod (灾厄): 肾上腺素" },
            {"CalamityMod: Elysian Guard", "Calamity Mod (灾厄): 极乐守护" },
            {"CalamityMod: Armor Set Bonus", "Calamity Mod (灾厄): 套装奖励" },
            {"CalamityMod: Astral Teleport", "Calamity Mod (灾厄): 天魔星石传送" },
            {"CalamityMod: Astral Arcanum UI Toggle", "Calamity Mod (灾厄): 开关星辉秘术UI" },
            {"CalamityMod: Momentum Capacitor Effect", "Calamity Mod (灾厄): 动量变压器" },
            {"CalamityMod: Sand Cloak Effect", "Calamity Mod (灾厄): 沙尘披风" },
            {"CalamityMod: Spectral Veil Teleport", "Calamity Mod (灾厄): 幽灵披风传送" },
            {"CalamityMod: Booster Dash", "Calamity Mod (灾厄): 瘟疫燃料背包冲刺" },
            {"CalamityMod: Angelic Alliance Blessing", "Calamity Mod (灾厄): 圣天誓盟祝福" },
            {"CalamityMod: God Slayer Dash", "Calamity Mod (灾厄): 弑神者冲刺" },
            {"CalamityMod: Exo Chair Speed Up", "Calamity Mod (灾厄): 星流飞椅加速" },
            {"CalamityMod: Exo Chair Slow Down", "Calamity Mod (灾厄): 星流飞椅减速" },
            {"AlchemistNPCLite: Discord Buff Teleportation", "Alchemist NPC Lite: 混沌传送增益一键传送" },
            {"RecipeBrowser: Toggle Recipe Browser", "Recipe Browser (合成表): 开关合成表查询UI" },
            {"RecipeBrowser: Query Hovered Item", "Recipe Browser (合成表): 一键查询指定物品" },
            {"RecipeBrowser: Toggle Favorited Recipes Window", "Recipe Browser (合成表): 开关收藏配方展示界面" },
            {"Fargowiltas: Quick Recall/Mirror", "Fargo之突变: 快捷回忆药水/魔镜" },
            {"Fargowiltas: Quick Use Custom (Bottom Left Inventory Slot)", "Fargo之突变: 快捷使用物品栏左下角物品" },
            {"Fargowiltas: Open Stat Sheet", "Fargo之突变: 开关玩家数据UI" },
            {"BossChecklist: Toggle Boss Checklist", "Boss Checklist (Boss清单): 开关Boss清单" },
            {"BossChecklist: Toggle Boss Log", "Boss Checklist (Boss清单): 开关Boss日志" },
            {"HEROsMod: Quick Stack", "HERO's Mod: 快速堆叠至附近的箱子" },
            {"HEROsMod: Sort Inventory", "HERO's Mod: 快捷整理物品栏" },
            {"HEROsMod: Swap Hotbar", "HERO's Mod: 切换快捷栏物品" },
            {"CheatSheet: Toggle Cheat Sheet Hotbar", "Cheat Sheet (作弊小抄): 切换快捷栏物品" },
            {"OreExcavator: Excavate (while mining)", "Ore Excavator (连锁挖矿): 开启连锁" },
            {"OreExcavator: Whitelist hovered", "Ore Excavator (连锁挖矿): 加入白名单" },
            {"OreExcavator: Un-whitelist hovered", "Ore Excavator (连锁挖矿): 取消白名单" },
        };

        private readonly static Dictionary<string, string> zhTranslationKeybind = new()
        {
            {"Mouse1", "鼠标左键" }, {"Mouse2", "鼠标右键" }, {"Mouse3", "鼠标中键" },
            {"Mouse4", "鼠标侧键1" }, {"Mouse5", "鼠标侧键2" }, {"Space", "空格" },
            {"Escape", "Esc键" }, { "Back", "退格" }, {"Enter", "回车" }, 
            {"LeftShift", "左Shift" }, {"LeftControl", "左Ctrl" }, {"LeftAlt", "左Alt" },
            {"RightShift", "右Shift" }, {"RightControl", "右Ctrl" }, {"RightAlt", "右Alt" },
            {"VolumeUp", "提高音量"},{"VolumeDown", "减少音量"},
            {"Divide", "小键盘 /" }, {"Add", "小键盘 +" },
            {"Subtract", "小键盘 -" }, {"Multiply", "小键盘 *" },
            {"OemComma", "</," }, {"OemPeriod", ">/." }, {"OemQuestion", "? /" },
            {"OemSemicolon", ":/;" }, {"OemQuotes", "\"/\'" }, {"OemPipe", "| \\" },
            {"OemOpenBrackets", "[/{" }, {"OemCloseBrackets", "]/}" },
            {"OemPlus", "+/=" }, {"OemMinus", "-/_" }, {"OemTilde", "~/`"}
        };

        public override void Load()
        {
            DrawSelf += DrawHoverText;
            GenInput += TranslatedInput;
            GetFriendlyName += TranslatedFriendlyName;
            CreateBindingGroup += AddModifyTip;
            SuperVaultKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.HugeInventory", "I");
            BuffTrackerKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.BuffTracker", "NumPad3");
            GrabBagKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.GrabBagLoot", "OemQuotes");
            HotbarSwitchKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.ImproveGame.Keybind.HotbarSwitch", "OemQuestion");
        }

        private void DrawHoverText(orig_DrawSelf orig, UIKeybindingListItem self, SpriteBatch spriteBatch)
        {
            orig.Invoke(self, spriteBatch);

            string _keybind = self.GetType().GetField("_keybind", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) as string;
            if (self.IsMouseHovering)
            {
                switch (_keybind)
                {
                    case "ImproveGame: $Mods.ImproveGame.Keybind.HotbarSwitch":
                        Main.instance.MouseText(GetText("Keybind.HotbarSwitch.Tip"));
                        break;
                    case "ImproveGame: $Mods.ImproveGame.Keybind.BuffTracker":
                        Main.instance.MouseText(GetText("Keybind.BuffTracker.Tip"));
                        break;
                }
            }
        }

        private string TranslatedInput(orig_GenInput orig, UIKeybindingListItem self, List<string> list)
        {
            if (UseKeybindTranslation && Language.ActiveCulture.Name == "zh-Hans")
            {
                var displayTexts = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    string text = list[i].Replace("NumPad", "小键盘 ");
                    if (zhTranslationKeybind.TryGetValue(list[i], out string translatedString))
                    {
                        text = translatedString;
                    }
                    displayTexts.Add(text);
                }
                return orig.Invoke(self, displayTexts);
            }
            return orig.Invoke(self, list);
        }

        private Terraria.GameContent.UI.States.UISortableElement AddModifyTip(orig_CreateBindingGroup orig, Terraria.GameContent.UI.States.UIManageControls self, int elementIndex, List<string> bindings, Terraria.GameInput.InputMode currentInputMode)
        {
            var uISortableElement = orig.Invoke(self, elementIndex, bindings, currentInputMode);
            if (Language.ActiveCulture.Name == "zh-Hans" && (elementIndex == 5 || elementIndex == 0))
            {
                string str = "部分控件汉化由“更好的体验”提供";
                if (elementIndex == 0)
                    str = "“更好的体验”提供了键位汉化";
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
                if (UseKeybindTranslation)
                    str = "快换回去";
                else
                    str = "让我看看";
                UIText buttonText = new(str, 0.8f)
                {
                    VAlign = 0.5f,
                    HAlign = 0.5f
                };
                button.OnMouseDown += (_, _) => {
                    UseKeybindTranslation = !UseKeybindTranslation;
                    if (UseKeybindTranslation)
                        str = "快换回去";
                    else
                        str = "让我看看";
                    buttonText.SetText(str);
                    SoundEngine.PlaySound(SoundID.MenuTick);
                };
                button.Append(buttonText);
                uISortableElement.Append(button);

                uISortableElement.Recalculate();
            }
            return uISortableElement;
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
            if (UseKeybindTranslation && Language.ActiveCulture.Name == "zh-Hans" && zhTranslationSupports.TryGetValue(keybindName, out string translatedString)) {
                return translatedString;
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
