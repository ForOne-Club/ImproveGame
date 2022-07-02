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

        public override void ResetEffects() {
            if (Player.whoAmI == Main.myPlayer) {
                if (MyUtils.Config.SuperVoidVault) {
                    PiggyBank = Player.HasItem(ItemID.PiggyBank);
                    Safe = Player.HasItem(ItemID.Safe);
                    DefendersForge = Player.HasItem(ItemID.DefendersForge);
                }
            }

            if (MyUtils.Config.NoCD_FishermanQuest) {
                if (Main.anglerQuestFinished || Main.anglerWhoFinishedToday.Contains(Name)) {
                    Main.anglerQuestFinished = false;
                    Main.anglerWhoFinishedToday.Clear();
                    Main.NewText(Language.GetTextValue($"Mods.ImproveGame.Tips.AnglerQuest"), ItemRarityID.Pink);
                }
            }
            if (Player.whoAmI == Main.myPlayer && MyUtils.Config.ImproveTileSpeedAndTileRange) {
                Player.tileSpeed += 1.5f;
                Player.wallSpeed += 1f;
                Player.tileRangeX += 5;
                Player.tileRangeY += 4;
            }
        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="triggersSet"></param>
        public override void ProcessTriggers(TriggersSet triggersSet) {
            if (KeybindSystem.SuperVaultKeybind.JustPressed) {
                if (MyUtils.Config.SuperVault) {
                    if (BigBagGUI.Visible) {
                        UISystem.Instance.JuVaultUIGUI.Close();
                    }
                    else {
                        UISystem.Instance.JuVaultUIGUI.Open();
                    }
                }
            }
        }
    }
}
