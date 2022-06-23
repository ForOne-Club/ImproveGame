using ImproveGame.Common.Systems;
using ImproveGame.UI;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImproveGame.Common.Players
{
    public class ImprovePlayer : ModPlayer
    {
        public static ImprovePlayer G(Player player) => player.GetModPlayer<ImprovePlayer>();

        public bool PiggyBank;
        public bool Safe;
        public bool DefendersForge;
        public float PlayerTimer;
        public override void ResetEffects()
        {
            PlayerTimer++;
            PiggyBank = Player.HasItem(ItemID.PiggyBank);
            Safe = Player.HasItem(ItemID.Safe);
            DefendersForge = Player.HasItem(ItemID.DefendersForge);
            if (MyUtils.Config().NoCD_FishermanQuest)
            {
                if (Main.anglerQuestFinished || Main.anglerWhoFinishedToday.Contains(Name))
                {
                    Main.anglerQuestFinished = false;
                    Main.anglerWhoFinishedToday.Clear();
                    Main.NewText(Language.GetTextValue($"Mods.ImproveGame.Tips.AnglerQuest"), ItemRarityID.Pink);
                }
            }
            if (Player.whoAmI == Main.myPlayer && MyUtils.Config().ImproveTileSpeedAndTileRange)
            {
                Player.tileSpeed += 1.5f;
                Player.wallSpeed += 1f;
                Player.tileRangeX += 5;
                Player.tileRangeY += 4;
            }
            if (MyUtils.Config().ImproveToolSpeed)
            {
                Player.pickSpeed -= 0.25f;
            }
        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="triggersSet"></param>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybinSystem.RandomBuffKeybind.JustPressed)
            {
                if (MyUtils.Config().SuperVault)
                {
                    if (JuBigVault.Visible)
                    {
                        JuBigVault._visible = false;
                    }
                    else
                    {
                        JuBigVault._visible = true;
                        Main.playerInventory = true;
                    }
                }
            }
        }
    }
}
