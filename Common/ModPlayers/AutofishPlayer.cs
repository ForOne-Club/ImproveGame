using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetAutofisher;
using ImproveGame.UI;
using ImproveGame.UI.Autofisher;
using ImproveGame.UIFramework;

namespace ImproveGame.Common.ModPlayers
{
    public class AutofishPlayer : ModPlayer
    {
        public static AutofishPlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<AutofishPlayer>();
        public TEAutofisher Autofisher;
        public bool IsAutofisherOpened => Autofisher is not null;
        public static bool TryGet(Player player, out AutofishPlayer modPlayer) => player.TryGetModPlayer(out modPlayer);

        public override void OnEnterWorld()
        {
            Autofisher = null;
        }

        public override void PlayerDisconnect()
        {
            // 这是其他客户端和服务器都执行的
            if (TryGet(Player, out var modPlayer))
            {
                modPlayer.SetAutofisher(null, false);
            }
        }

        public void SetAutofisher(TEAutofisher autofisher, bool needSync = true)
        {
            Autofisher = autofisher;

            // 设置传输
            if (needSync && Main.netMode != NetmodeID.SinglePlayer)
            {
                SyncOpenPacket.Get(Autofisher?.ID ?? -1, Main.myPlayer).Send(runLocally: false);
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            // 服务器给新玩家发开箱了的玩家的箱子状态包
            if (Main.netMode != NetmodeID.Server)
            {
                return;
            }

            for (byte i = 0; i < Main.maxPlayers; i++)
            {
                var player = Main.player[i];
                if (player.active && !player.dead && TryGet(player, out var modPlayer) && modPlayer.Autofisher is not null)
                {
                    SyncOpenPacket.Get(modPlayer.Autofisher.ID, i).Send(toWho, fromWho, runLocally: false);
                }
            }
        }

        public override void PreUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server)
                return;

            switch (AutofisherGUI.Visible)
            {
                // 不显示 UI，Autofisher 不是 null
                case false when Autofisher is not null:
                    SetAutofisher(null);
                    break;
                // 显示 UI，Autofisher 是 null
                case true when Autofisher is null:
                    SidedEventTrigger.ToggleViewBody(UISystem.Instance.AutofisherGUI);
                    break;
                // 显示 UI，Autofisher 不是 null
                case true:
                    int playerX = (int)(Player.Center.X / 16f);
                    int playerY = (int)(Player.Center.Y / 16f);
                    if (playerX < Autofisher.Position.X - Player.lastTileRangeX ||
                        playerX > Autofisher.Position.X + Player.lastTileRangeX + 1 ||
                        playerY < Autofisher.Position.Y - Player.lastTileRangeY ||
                        playerY > Autofisher.Position.Y + Player.lastTileRangeY + 1)
                    {
                        SoundEngine.PlaySound(SoundID.MenuClose);
                        SidedEventTrigger.ToggleViewBody(UISystem.Instance.AutofisherGUI);
                    }
                    else if (TileLoader.GetTile(Main.tile[Autofisher.Position.ToPoint()].TileType) is not Content.Tiles.Autofisher)
                    {
                        SoundEngine.PlaySound(SoundID.MenuClose);
                        SidedEventTrigger.ToggleViewBody(UISystem.Instance.AutofisherGUI);
                    }
                    break;
            }
        }
    }
}
