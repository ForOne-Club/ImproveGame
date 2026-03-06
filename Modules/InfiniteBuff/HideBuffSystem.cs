using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Content.Items.ItemContainer;

namespace ImproveGame.Modules.InfiniteBuff;

/// <summary>
/// 生成并维护“本帧需要隐藏哪些 Buff”的查询表。
/// </summary>
/// <remarks>
/// 该系统只负责“标记与统计”；实际绘制拦截由 <see cref="HideGlobalBuff"/> 处理。
/// </remarks>
public class HideBuffSystem : ModSystem
{
    /// <summary>
    /// 可在原版 Buff UI 隐藏的 Buff 标记表 (Index = BuffType)
    /// </summary>
    public static bool[] HideFlags => _hideFlags;
    private static bool[] _hideFlags = new bool[BuffLoader.BuffCount];

    public override void PostSetupContent() => Array.Resize(ref _hideFlags, BuffLoader.BuffCount);

    /// <summary>
    /// 重建 <see cref="HideFlags"/>。
    /// </summary>
    public override void PostDrawInterface(SpriteBatch spriteBatch)
    {
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer)) return;

        // 清空上一帧残留。
        Array.Clear(_hideFlags, 0, _hideFlags.Length);

        // 供 HideGlobalBuff 在本帧重新累计隐藏数量。
        HideGlobalBuff.HidedBuffCountThisFrame = 0;

        // 1) 本地玩家可用物品
        SetupHideFlags(infinitePlayer.PlayerAvailableItems);

        // 2) 队友可共享物品 (开启时)
        if (Config.ShareInfBuffs)
        {
            InfiniteBuffPlayer.ForEachTeammate(player.whoAmI,
                (player) => SetupHideFlags(player.GetModPlayer<InfiniteBuffPlayer>().PlayerAvailableItems));
        }

        // 3) 储存系统可用物品
        SetupHideFlags(infinitePlayer.ExStorageAvailableItems);
    }

    /// <summary>
    /// 根据物品集合更新隐藏标记表。
    /// </summary>
    /// <param name="items">可触发无限 Buff 的物品集合。</param>
    /// <remarks>
    /// 规则：
    /// <br/>1) 读取物品自身可提供的 Buff
    /// <br/>2) 若是药水袋，额外展开内部药水 (需满足无消耗堆叠阈值)
    /// <br/>3) 跳过 -1（无效 Buff ID），避免写入非法下标。
    /// </remarks>
    private static void SetupHideFlags(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            if (item is null || item.IsAir) continue;

            // 药水袋
            if (item.ModItem is PotionBag bag && bag.ItemContainer is { Count: > 0 } container)
            {
                foreach (var potion in container)
                {
                    if (item is null || item.IsAir) continue;
                    if (item.stack < Config.NoConsume_PotionRequirement) continue;
                    _hideFlags[potion.buffType] = true;
                }

                continue;
            }

            foreach (var buffType in ApplyBuffItem.GetItemBuffTypes(item))
            {
                if (buffType == -1) continue;
                _hideFlags[buffType] = true;
            }
        }
    }
}