namespace ImproveGame.Common.ModPlayers;

/// <summary>
/// 用于检查当前玩家是否完成模组同步从而能够接收模组包
/// <br>因为目前有遇见在完成模组列表同步之前先被钓鱼机盯上作为目标进行roll从而导致错错报的情况</br>
/// </summary>
public class AutofishAvailabliltyCheckPlayer : ModPlayer
{
    public bool Available { get; private set; }
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (Main.dedServ 
            && fromWho != -1
            && Main.player[fromWho].TryGetModPlayer<AutofishAvailabliltyCheckPlayer>(out var mplr))
            mplr.Available = true;
    }
}
