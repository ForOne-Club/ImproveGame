using ImproveGame.Common.GlobalBuffs;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Content.Items.ItemContainer;

namespace ImproveGame.IndependentModules.InfiniteBuff;

/// <summary>
/// 生成并维护“本帧需要隐藏哪些 Buff”的查询表。
/// </summary>
/// <remarks>
/// 该系统只负责“标记与统计”；实际绘制拦截由 <see cref="HideGlobalBuff"/> 处理。
/// </remarks>
public class HideBuffSystem : ModSystem
{
    /// <summary>
    /// 维护“本帧应隐藏的 Buff”标记表（索引 = Buff ID）。
    /// </summary>
    /// <remarks>
    /// 该表会被 <see cref="HideGlobalBuff"/> 和无限 Buff UI 直接读取，因此要求：
    /// <br/>1) 读取快（数组 O(1)）；
    /// <br/>2) 长度始终等于 <see cref="BuffLoader.BuffCount"/>；
    /// <br/>3) 每帧重建，避免跨帧残留。
    /// </remarks>
    internal static bool[] HideFlags => _hideFlags;

    /// <summary>
    /// 实际存储的隐藏标记表。
    /// </summary>
    /// <remarks>
    /// 在 <see cref="PostSetupContent"/> 对齐长度，
    /// 在 <see cref="PostDrawInterface"/> 每帧清空并重建。
    /// </remarks>
    private static bool[] _hideFlags = new bool[BuffLoader.BuffCount];

    /// <summary>
    /// 内容注册完成后，对齐隐藏标记表长度。
    /// </summary>
    /// <remarks>
    /// Buff 数量在加载阶段可能变化，此时才能拿到最终值。
    /// </remarks>
    public override void PostSetupContent() => Array.Resize(ref _hideFlags, BuffLoader.BuffCount);

    /// <summary>
    /// 返回本帧被标记为隐藏的 Buff 数量。
    /// </summary>
    /// <remarks>
    /// 结果仅代表当前帧快照。
    /// </remarks>
    public static int GetHideCount()
    {
        var count = 0;
        for (int i = 0; i < _hideFlags.Length; i++)
        {
            if (_hideFlags[i]) count++;
        }

        return count;
    }

    /// <summary>
    /// 每帧重建 <see cref="HideFlags"/>。
    /// </summary>
    /// <remarks>
    /// 数据来源顺序与 <see cref="InfiniteBuffPlayer.PostUpdateBuffs"/> 保持一致：
    /// 本地玩家 -> 队友共享（可选）-> 储存系统。
    /// </remarks>
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

        // 2) 队友可共享物品（开启时）
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
    /// <br/>1) 读取物品自身可提供的 Buff；
    /// <br/>2) 若是药水袋，额外展开内部药水（需满足无消耗堆叠阈值）；
    /// <br/>3) 跳过 -1（无效 Buff ID），避免写入非法下标。
    /// </remarks>
    private static void SetupHideFlags(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            // 可能返回多个 Buff。
            var buffTypes = ApplyBuffItem.GetItemBuffType(item);
            buffTypes.ForEach(buffType =>
            {
                // -1 是占位值，不对应有效 Buff ID。
                if (buffType is not -1)
                    _hideFlags[buffType] = true;

                // 药水袋内部药水达到阈值后，同样视为“可隐藏 Buff”。
                if (!item.IsAir && item.ModItem is PotionBag potionBag && potionBag.ItemContainer.Count > 0)
                {
                    foreach (var potion in from p in potionBag.ItemContainer
                             where p.stack >= Config.NoConsume_PotionRequirement select p)
                        _hideFlags[potion.buffType] = true;
                }
            });
        }
    }
}