using ImproveGame.Common.Players;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;

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

        public bool ServerOpenRequest = false;
        public override bool OnRightClick(int i, int j) {
            var origin = MyUtils.GetTileOrigin(i, j);
            if (AutofisherGUI.Visible && AutofishPlayer.LocalPlayer.Autofisher == origin) {
                UISystem.Instance.AutofisherGUI.Close();
            }
            else {
                if (Main.netMode == NetmodeID.MultiplayerClient && !ServerOpenRequest) {
                    NetAutofish.ClientSendOpenRequest(origin);
                    return false;
                }
                ServerOpenRequest = false;
                UISystem.Instance.AutofisherGUI.Open(origin);
            }
            return true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) {
            if (!MyUtils.TryGetTileEntityAs<TEAutofisher>(i, j, out var autofisher))
                return true;
            if (autofisher.accessory.IsAir && autofisher.bait.IsAir && autofisher.fishingPole.IsAir) {
                if (autofisher.fish is null)
                    return true;
                for (int k = 0; k < 15; k++) {
                    if (autofisher.fish[k] is not null && !autofisher.fish[k].IsAir) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            if (!MyUtils.TryGetTileEntityAs<TEAutofisher>(i, j, out var fisher))
                return;
            if (Main.tile[i, j].TileFrameX != 0 || Main.tile[i, j].TileFrameY != 0)
                return;

            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen) {
                offScreen = Vector2.Zero;
            }

            var worldPos = new Point(i, j).ToWorldCoordinates(-4, -20);
            var screenPos = worldPos + offScreen - Main.screenPosition;
            string text = fisher.Opened ? "On" : "Off";
            Color textColor = fisher.Opened ? new(20, 240, 20) : new(240, 20, 20);
            Utils.DrawBorderString(spriteBatch, text, screenPos, textColor);
        }
    }
}
