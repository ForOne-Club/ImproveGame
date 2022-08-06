using ImproveGame.Content.Tiles;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.Audio;
using Terraria.DataStructures;

namespace ImproveGame.Common.Players
{
    public class AutofishPlayer : ModPlayer
    {
        public static AutofishPlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<AutofishPlayer>();
        internal Point16 Autofisher { get; private set; } = Point16.NegativeOne;
        public static bool TryGet(Player player, out AutofishPlayer modPlayer) => player.TryGetModPlayer(out modPlayer);

        public override void OnEnterWorld(Player player) {
            Autofisher = Point16.NegativeOne;
        }

        public override void PlayerDisconnect(Player player) {
            // 这是其他客户端和服务器都执行的
            if (TryGet(player, out var modPlayer)) {
                modPlayer.SetAutofisher(Point16.NegativeOne, false);
            }
        }

        public void SetAutofisher(Point16 point, bool needSync = true) {
            // 切换两边（如果有的话）Autofisher的状态
            TryGetAutofisher(out var fisherOld);
            fisherOld.Opened = false;
            if (MyUtils.TryGetTileEntityAs<TEAutofisher>(point.X, point.Y, out var fisherNew)) {
                fisherNew.Opened = true;
            }

            // 应用开关
            Autofisher = point;

            // 设置传输
            if (needSync && Main.netMode != NetmodeID.SinglePlayer)
                NetAutofish.ClientSendAutofisherPosition(point.X, point.Y);
        }

        public void SetLocatePoint(TEAutofisher autofisher, Point16 point) {
            autofisher.locatePoint = point;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, autofisher.ID, autofisher.Position.X, autofisher.Position.Y);
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            // 服务器给新玩家发开箱了的玩家的箱子状态包
            if (Main.netMode == NetmodeID.Server) {
                for (byte i = 0; i < Main.maxPlayers; i++) {
                    var player = Main.player[i];
                    if (player.active && !player.dead && TryGet(player, out var modPlayer) && modPlayer.Autofisher.X > 0 && modPlayer.Autofisher.Y > 0) {
                        NetAutofish.ServerSendPlayerToggle(modPlayer.Autofisher, i, toWho, fromWho);
                    }
                }
            }
        }

        public override void UpdateDead() {
            if (Player.whoAmI == Main.myPlayer && AutofisherGUI.Visible)
                UISystem.Instance.AutofisherGUI.Close();
        }

        public override void ResetEffects() {
            if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server)
                return;

            if (!AutofisherGUI.Visible && Autofisher.X > 0 && Autofisher.Y > 0) {
                SetAutofisher(Point16.NegativeOne);
            }

            if (AutofisherGUI.Visible && GetAutofisher() is null) {
                UISystem.Instance.AutofisherGUI.Close();
                return;
            }

            if (AutofisherGUI.Visible && Autofisher.X >= 0 && Autofisher.Y >= 0 && (Player.chest != -1 || !Main.playerInventory || Player.sign > -1 || Player.talkNPC > -1)) {
                UISystem.Instance.AutofisherGUI.Close();
            }
            else if (AutofisherGUI.Visible && Autofisher.X >= 0 && Autofisher.Y >= 0) {
                int playerX = (int)(Player.Center.X / 16f);
                int playerY = (int)(Player.Center.Y / 16f);
                if (playerX < Autofisher.X - Player.lastTileRangeX ||
                     playerX > Autofisher.X + Player.lastTileRangeX + 1 ||
                     playerY < Autofisher.Y - Player.lastTileRangeY ||
                     playerY > Autofisher.Y + Player.lastTileRangeY + 1) {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                    UISystem.Instance.AutofisherGUI.Close();
                }
                else if (TileLoader.GetTile(Main.tile[Autofisher.X, Autofisher.Y].TileType) is not Content.Tiles.Autofisher) {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                    UISystem.Instance.AutofisherGUI.Close();
                }
            }
        }

        public bool TryGetAutofisher(out TEAutofisher autofisher) {
            autofisher = GetAutofisher();
            if (autofisher == null) {
                autofisher = new();
                return false;
            }
            return true;
        }

        public TEAutofisher GetAutofisher() {
            if (Autofisher.X < 0 || Autofisher.Y < 0)
                return null;
            Tile tile = Main.tile[Autofisher.X, Autofisher.Y];
            if (!tile.HasTile)
                return null;
            if (!MyUtils.TryGetTileEntityAs<TEAutofisher>(Autofisher.X, Autofisher.Y, out var fisher))
                return null;
            return fisher;
        }
    }
}
