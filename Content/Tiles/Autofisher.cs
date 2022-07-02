using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Concurrent;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ObjectInteractions;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using ImproveGame.Common.Players;

namespace ImproveGame.Content.Tiles
{
	public class Autofisher : TETileBase
    {
        public override ModTileEntity GetTileEntity() => ModContent.GetInstance<TEAutofisher>();

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override int ItemType(int frameX, int frameY) => ModContent.ItemType<Items.Placeable.Autofisher>();
        
        public override void MouseOver(int i, int j) {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType(tile.TileFrameX, tile.TileFrameY);
            player.noThrow = 2;
        }

        public override bool OnRightClick(int i, int j) {
            var origin = MyUtils.GetTileOrigin(i, j);
            if (AutofisherGUI.Visible && AutofishPlayer.LocalPlayer.Autofisher == origin) {
                UISystem.Instance.AutofisherGUI.Close();
            }
            else {
                UISystem.Instance.AutofisherGUI.Open(origin);
            }
            return true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) {
            if (!MyUtils.TryGetTileEntityAs<TEAutofisher>(i, j, out var autofisher))
                return true;
            if (autofisher.accessory.IsAir && autofisher.bait.IsAir && autofisher.fishingPole.IsAir) {
                for (int k = 0; k < autofisher.fish.Length; k++) {
                    if (!autofisher.fish[k].IsAir) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
