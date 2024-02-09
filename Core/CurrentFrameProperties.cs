using ImproveGame.UI.ItemContainer;

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
        public readonly static HashSet<int> Inventory = [];

        /// <summary>
        /// 玩家当前大背包拥有的物品的ID
        /// </summary>
        public readonly static HashSet<int> BigBag = [];

        /// <summary>
        /// 玩家当前四大Bank拥有的物品的ID
        /// </summary>
        public readonly static HashSet<int> Banks = [];

        /// <summary>
        /// 玩家当前帧背包中拥有此物品的数量
        /// </summary>
        public readonly static Dictionary<int, ItemTotalStack> InventoryTotalNumber = [];

        /// <summary>
        /// 玩家当前帧大背包中拥有此物品的数量
        /// </summary>
        public readonly static Dictionary<int, ItemTotalStack> BigBagTotalNumber = [];

        /// <summary>
        /// 玩家当前帧 Bank 中拥有此物品的数量
        /// </summary>
        public readonly static Dictionary<int, ItemTotalStack> BanksTotalNumber = [];

        public static int GetTotalNumber(int type)
        {
            int count = 0;

            if (InventoryTotalNumber.TryGetValue(type, out var invTotalStack))
                count += invTotalStack.Number;

            if (BanksTotalNumber.TryGetValue(type, out var banksTotalStack))
                count += banksTotalStack.Number;

            if (BigBagTotalNumber.TryGetValue(type, out var bigbagTotalStack))
                count += bigbagTotalStack.Number;

            return count;
        }
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

        CurrentFrameProperties.ExistItems.InventoryTotalNumber.Clear();
        CurrentFrameProperties.ExistItems.BanksTotalNumber.Clear();
        CurrentFrameProperties.ExistItems.BigBagTotalNumber.Clear();

        // 玩家背包
        foreach (var item in Player.inventory.Where(item => !item.IsAir))
        {
            CurrentFrameProperties.ExistItems.Inventory.Add(item.type);

            if (item.ModItem is IItemContainer iic)
                ItemStatistics(CurrentFrameProperties.ExistItems.InventoryTotalNumber, iic.ItemContainer);

            if (CurrentFrameProperties.ExistItems.InventoryTotalNumber.ContainsKey(item.type))
                CurrentFrameProperties.ExistItems.InventoryTotalNumber[item.type].Number += item.stack;
            else
                CurrentFrameProperties.ExistItems.InventoryTotalNumber.Add(item.type, new ItemTotalStack(item));
        }

        // Banks
        var bankItems = GetAllInventoryItemsList(Player, "inv mod");
        foreach (var item in bankItems.Where(item => !item.IsAir))
        {
            CurrentFrameProperties.ExistItems.Banks.Add(item.type);

            if (item.ModItem is IItemContainer iic)
                ItemStatistics(CurrentFrameProperties.ExistItems.BanksTotalNumber, iic.ItemContainer);

            if (CurrentFrameProperties.ExistItems.BanksTotalNumber.ContainsKey(item.type))
                CurrentFrameProperties.ExistItems.BanksTotalNumber[item.type].Number += item.stack;
            else
                CurrentFrameProperties.ExistItems.BanksTotalNumber.Add(item.type, new ItemTotalStack(item));
        }

        // 大背包
        var bigBagItems = GetAllInventoryItemsList(Player, "inv portable");
        foreach (var item in bigBagItems.Where(item => !item.IsAir))
        {
            CurrentFrameProperties.ExistItems.BigBag.Add(item.type);

            if (item.ModItem is IItemContainer iic)
                ItemStatistics(CurrentFrameProperties.ExistItems.BigBagTotalNumber, iic.ItemContainer);

            if (CurrentFrameProperties.ExistItems.BigBagTotalNumber.ContainsKey(item.type))
                CurrentFrameProperties.ExistItems.BigBagTotalNumber[item.type].Number += item.stack;
            else
                CurrentFrameProperties.ExistItems.BigBagTotalNumber.Add(item.type, new ItemTotalStack(item));
        }
    }

    public static void ItemStatistics(Dictionary<int, ItemTotalStack> count, IEnumerable<Item> items)
    {
        foreach (var item in items.Where(chestItem => !chestItem.IsAir))
        {
            if (count.ContainsKey(item.type))
                count[item.type].Number += item.stack;
            else
                count.Add(item.type, new ItemTotalStack(item));
        }
    }
}

public class ItemTotalStack(Item item)
{
    public int Number = item.stack;

    public readonly int Type = item.type;
    public readonly int BuffType = item.buffType;
    public readonly int BuffTime = item.buffTime;
    public readonly int PlaceStyle = item.placeStyle;
    public readonly int CreateTile = item.createTile;

    public override bool Equals(object obj) => obj is ItemTotalStack its && item.type == its.Type;
    public override int GetHashCode() => Type;
}