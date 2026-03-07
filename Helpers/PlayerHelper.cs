namespace ImproveGame.Helpers;

public static class PlayerHelper
{
    /// <summary>
    /// 遍历队友，然后执行某个事件
    /// </summary>
    public static void ForEachTeammate(int myself, Action<Player> action, bool requireAlive = true)
    {
        var teammates = Main.player.AsSpan();
        if (myself < 0 || myself >= teammates.Length ||
            Main.netMode != NetmodeID.MultiplayerClient) return;

        var player = teammates[myself];
        for (int i = 0; i < teammates.Length; i++)
        {
            // 真实队友检测
            if (i == myself || teammates[i] is not { active: true } teammate) continue;
            // 同队检测
            if (teammate.team == 0 || teammate.team != player.team) continue;
            // 死亡检测
            if (requireAlive && teammate.DeadOrGhost) continue;
            // 距离检测
            var distance = teammate.Distance(player.Center);
            if (Config.ShareRange != -1 && distance > Config.ShareRange * 16f) continue;

            action.Invoke(teammate);
        }
    }
}
