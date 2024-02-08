namespace ImproveGame.UI.DeathSpectating;

public class SpectatingPlayer : ModPlayer
{
    public int SpectatingTarget { get; set; }

    public readonly List<Player> AlivePlayers = [];
    public readonly List<Player> ActivePlayers = [];

    public override void OnEnterWorld()
    {
        SpectatingTarget = Main.myPlayer;
    }

    public override void UpdateEquips()
    {
        if (Main.netMode is NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer &&
            Main.player.IndexInRange(SpectatingTarget) && Main.player[SpectatingTarget].active &&
            SpectatingTarget != Main.myPlayer)
        {
            Player.isOperatingAnotherEntity = true;
        }

        if (Main.netMode is NetmodeID.Server &&
            Main.player.IndexInRange(SpectatingTarget) && Main.player[SpectatingTarget].active &&
            SpectatingTarget != Main.myPlayer)
        {
            RemoteClient.CheckSection(Player.whoAmI, Main.player[SpectatingTarget].position);
        }
    }

    public static void SyncData()
    {

    }

    public override void ModifyScreenPosition()
    {
        if (Main.netMode is NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
        {
            UpdatePlayers();

            if (Main.player.IndexInRange(SpectatingTarget) &&
                Main.player[SpectatingTarget].active && SpectatingTarget != Main.myPlayer)
            {
                Main.screenPosition = Main.player[SpectatingTarget].Center - Main.ScreenSize.ToVector2() / 2f;
            }
        }
    }

    /// <summary>
    /// 刷新 <see cref="ActivePlayers"/> <see cref="AlivePlayers"/>
    /// </summary>
    private void UpdatePlayers()
    {
        AlivePlayers.Clear();
        ActivePlayers.Clear();

        foreach (var player in Main.player)
        {
            if (player.active)
            {
                ActivePlayers.Add(player);

                if (!player.dead)
                {
                    AlivePlayers.Add(player);
                }
            }
        }
    }
}
