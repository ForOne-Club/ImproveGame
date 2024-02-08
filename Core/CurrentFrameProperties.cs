namespace ImproveGame.Core;

// 备用名 CurrentFrameFlags，但会和原版重了
/// <summary>
/// 记录当前帧的一些属性，如是否有Boss存在，这样可以避免重复遍历NPC数组
/// </summary>
public static class CurrentFrameProperties
{
    public static class ExistItems
    {
        /// <summary>
        /// 玩家当前物品栏拥有的物品的ID
        /// </summary>
        public static HashSet<int> Inventory { get; set; } = [];

        /// <summary>
        /// 玩家当前大背包拥有的物品的ID
        /// </summary>
        public static HashSet<int> BigBag { get; set; } = [];

        /// <summary>
        /// 玩家当前四大Bank拥有的物品的ID
        /// </summary>
        public static HashSet<int> Banks { get; set; } = [];
    }

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
            var npc = Main.npc[l];
            // 排除四柱
            if (npc.type is NPCID.LunarTowerNebula or NPCID.LunarTowerSolar or NPCID.LunarTowerStardust
                or NPCID.LunarTowerVortex)
                continue;
            if (npc.active && (npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type]))
            {
                CurrentFrameProperties.AnyActiveBoss = true;
                return;
            }
        }
    }
}

internal class PlayerUpdater : ModPlayer
{
    public override void ResetEffects()
    {
        UpdateHasItemLists();
    }

    public override void UpdateDead()
    {
        UpdateHasItemLists();
    }

    private void UpdateHasItemLists()
    {
        // 本地玩家才更新
        if (Main.myPlayer != Player.whoAmI) return;

        CurrentFrameProperties.ExistItems.Inventory.Clear();
        CurrentFrameProperties.ExistItems.Banks.Clear();
        CurrentFrameProperties.ExistItems.BigBag.Clear();

        foreach (var item in Player.inventory.Where(item => !item.IsAir))
        {
            CurrentFrameProperties.ExistItems.Inventory.Add(item.type);
        }

        var bankItems = GetAllInventoryItemsList(Player, "inv mod");
        foreach (var item in bankItems.Where(item => !item.IsAir))
        {
            CurrentFrameProperties.ExistItems.Banks.Add(item.type);
        }

        var bigBagItems = GetAllInventoryItemsList(Player, "inv portable");
        foreach (var item in bigBagItems.Where(item => !item.IsAir))
        {
            CurrentFrameProperties.ExistItems.BigBag.Add(item.type);
        }
    }
}