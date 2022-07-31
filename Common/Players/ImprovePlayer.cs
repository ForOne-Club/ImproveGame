using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImproveGame.Common.Players
{
    public class ImprovePlayer : ModPlayer
    {
        public bool PiggyBank;
        public bool Safe;
        public bool DefendersForge;

        public override void ResetEffects()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                if (MyUtils.Config.SuperVoidVault)
                {
                    PiggyBank = Player.HasItem(ItemID.PiggyBank);
                    Safe = Player.HasItem(ItemID.Safe);
                    DefendersForge = Player.HasItem(ItemID.DefendersForge);
                }
            }

            if (MyUtils.Config.NoCD_FishermanQuest)
            {
                if (Main.anglerQuestFinished || Main.anglerWhoFinishedToday.Contains(Name))
                {
                    Main.anglerQuestFinished = false;
                    Main.anglerWhoFinishedToday.Clear();
                    Main.NewText(Language.GetTextValue($"Mods.ImproveGame.Tips.AnglerQuest"), ItemRarityID.Pink);
                }
            }
            if (Player.whoAmI == Main.myPlayer && !Player.HeldItem.IsAir && MyUtils.Config.ImproveTileSpeedAndTileRange)
            {
                string internalName = ItemID.Search.GetName(Player.HeldItem.type).ToLower(); // 《英文名》因为没法获取英文名，只能用内部名了
                string currentLanguageName = Lang.GetItemNameValue(Player.HeldItem.type).ToLower();
                foreach (string str in MyUtils.Config.TileSpeed_Blacklist)
                {
                    if (internalName.Contains(str) || currentLanguageName.Contains(str))
                    {
                        return;
                    }
                }
                Player.tileSpeed = 3f;
                Player.wallSpeed = 3f;
                Player.tileRangeX += 5;
                Player.tileRangeY += 4;
            }
        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="triggersSet"></param>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.SuperVaultKeybind.JustPressed)
            {
                if (Config.SuperVault)
                {
                    if (BigBagGUI.Visible)
                    {
                        UISystem.Instance.BigBagGUI.Close();
                    }
                    else
                    {
                        UISystem.Instance.BigBagGUI.Open();
                    }
                }
            }
            if (KeybindSystem.BuffTrackerKeybind.JustPressed)
            {
                if (BuffTrackerGUI.Visible)
                    UISystem.Instance.BuffTrackerGUI.Close();
                else
                    UISystem.Instance.BuffTrackerGUI.Open();
            }
            if (KeybindSystem.GrabBagKeybind.JustPressed && Main.HoverItem is not null)
            {
                var item = Main.HoverItem;
                bool hasLoot = Main.ItemDropsDB.GetRulesForItemID(item.type, includeGlobalDrops: false).Count > 0;
                if (GrabBagInfoGUI.Visible && (GrabBagInfoGUI.ItemID == item.type || item.IsAir || !hasLoot))
                    UISystem.Instance.GrabBagInfoGUI.Close();
                else if (item is not null && hasLoot)
                    UISystem.Instance.GrabBagInfoGUI.Open(Main.HoverItem.type);
            }
        }
    }
}
