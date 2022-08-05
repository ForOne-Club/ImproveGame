using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using Terraria.GameInput;

namespace ImproveGame.Common.Players
{
    public class ImprovePlayer : ModPlayer
    {
        public bool PiggyBank;
        public bool Safe;
        public bool DefendersForge;

        public bool ShouldUpdateTeam;

        public override void OnEnterWorld(Player player)
        {
            if (Config.TeamAutoJoin && Main.netMode is NetmodeID.MultiplayerClient)
            {
                ShouldUpdateTeam = true;
            }
        }

        public override void ResetEffects()
        {
            if (MyUtils.Config.SuperVoidVault)
            {
                PiggyBank = Player.HasItem(ItemID.PiggyBank);
                Safe = Player.HasItem(ItemID.Safe);
                DefendersForge = Player.HasItem(ItemID.DefendersForge);
            }

            if (Player.whoAmI == Main.myPlayer)
            {
                if (ShouldUpdateTeam)
                {
                    Player.team = 1;
                    NetMessage.SendData(MessageID.PlayerTeam, -1, -1, null, Player.whoAmI);
                    ShouldUpdateTeam = false;
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

        private bool _cacheSwitchSlot;

        /// <summary>
        /// 快捷键
        /// </summary>
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
            if (KeybindSystem.GrabBagKeybind.JustPressed && Main.HoverItem is not null && ItemLoader.CanRightClick(Main.HoverItem))
            {
                var item = Main.HoverItem;
                bool hasLoot = Main.ItemDropsDB.GetRulesForItemID(item.type, includeGlobalDrops: true).Count > 0;
                if (GrabBagInfoGUI.Visible && (GrabBagInfoGUI.ItemID == item.type || item.IsAir || !hasLoot))
                    UISystem.Instance.GrabBagInfoGUI.Close();
                else if (item is not null && hasLoot)
                    UISystem.Instance.GrabBagInfoGUI.Open(Main.HoverItem.type);
            }
            if (KeybindSystem.HotbarSwitchKeybind.JustPressed || _cacheSwitchSlot)
            {
                if (Main.LocalPlayer.ItemTimeIsZero && Main.LocalPlayer.itemAnimation is 0)
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        (Player.inventory[i], Player.inventory[i + 10]) = (Player.inventory[i + 10], Player.inventory[i]);
                        if (Main.netMode is NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncEquipment, number: Main.myPlayer, number2: i, number3: Main.LocalPlayer.inventory[i].prefix);
                            NetMessage.SendData(MessageID.SyncEquipment, number: Main.myPlayer, number2: i + 10, number3: Main.LocalPlayer.inventory[i].prefix);
                        }
                    }
                    _cacheSwitchSlot = false;
                }
                else
                {
                    _cacheSwitchSlot = true;
                }
            }
        }
    }
}
