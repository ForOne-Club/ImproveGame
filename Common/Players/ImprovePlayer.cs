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
        public bool PiggyBank => Player.HasItem(ItemID.PiggyBank);
        public bool Safe => Player.HasItem(ItemID.Safe);
        public bool DefendersForge => Player.HasItem(ItemID.DefendersForge);
        public float PlayerTimer;
        public override void ResetEffects() {
            PlayerTimer++;
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
            if (KeybinSystem.RandomBuffKeybind.JustPressed) {
                if (MyUtils.Config.SuperVault) {
                    if (BigBagGUI.Visible) {
                        BigBagGUI.Close();
                    }
                    else {
                        BigBagGUI.Open();
                    }
                }
            }
        }
    }
}
