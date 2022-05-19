using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImproveGame.Common.ModPlayers
{
    public class UpdatePlayer : ModPlayer
    {
        public bool MagiskKillTiles;
        public override void ResetEffects()
        {
            MagiskKillTiles = false;
            if (Utils.GetConfig().NoCD_FishermanQuest)
            {
                if (Main.anglerQuestFinished || Main.anglerWhoFinishedToday.Contains(Name))
                {
                    Main.anglerQuestFinished = false;
                    Main.anglerWhoFinishedToday.Clear();
                    Main.NewText(Language.GetTextValue($"Mods.ImproveGame.Tips.AnglerQuest"), ItemRarityID.Pink);
                }
            }
            if (Player.whoAmI == Main.myPlayer && Utils.GetConfig().ImproveTileSpeedAndTileRange)
            {
                Player.tileSpeed += 1.5f;
                Player.wallSpeed += 1f;
                Player.tileRangeX += 5;
                Player.tileRangeY += 4;
            }
            if (Utils.GetConfig().ImproveToolSpeed)
            {
                Player.pickSpeed -= 0.25f;
            }
        }
    }
}
