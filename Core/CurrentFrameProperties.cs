namespace ImproveGame.Core;

// 备用名 CurrentFrameFlags，但会和原版重了
/// <summary>
/// 记录当前帧的一些属性，如是否有Boss存在，这样可以避免重复遍历NPC数组
/// </summary>
public static class CurrentFrameProperties
{
    /// <summary>
    /// 判断场上有没有Boss存在，避免重复遍历NPC数组
    /// 不知道为什么，原版的不包括世吞，这里包括
    /// </summary>
    public static bool AnyActiveBoss { get; set; }
}

internal class SystemUpdater : ModSystem
{
    public override void PreUpdateNPCs()
    {
        CurrentFrameProperties.AnyActiveBoss = false;

        for (int l = 0; l < 200; l++)
        {
            if (Main.npc[l].active && (Main.npc[l].boss || NPCID.Sets.DangerThatPreventsOtherDangers[Main.npc[l].type]))
            {
                CurrentFrameProperties.AnyActiveBoss = true;
                return;
            }
        }
    }
}