using ImproveGame.Common.Systems;
using ImproveGame.Content.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImproveGame.Common.ModPlayers
{
    public class ImprovePlayer : ModPlayer
    {
        public static ImprovePlayer G(Player player)
        {
            return player.GetModPlayer<ImprovePlayer>();
        }
        public bool MagiskKillTiles;
        public bool PiggyBank;
        public bool Safe;
        public bool DefendersForge;
        public override void ResetEffects()
        {
            MagiskKillTiles = false;
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
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybinSystem.RandomBuffKeybind.JustPressed)
            {
                if (MyUtils.Config().SuperVault)
                {
                    if (VaultUI._visible)
                    {
                        VaultUI._visible = false;
                    }
                    else
                    {
                        VaultUI._visible = true;
                        Main.playerInventory = true;
                    }
                }
            }
        }
    }
}
