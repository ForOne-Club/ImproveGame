using ImproveGame.Content.Functions.PortableBuff;
using SilkyUIFramework;
using Terraria.ModLoader.IO;

namespace ImproveGame.IndependentModules.InfiniteBuff;

/// <summary>
/// 保存并判定“刷怪倍率控制”所需的玩家侧状态。
/// </summary>
/// <remarks>
/// 该类只维护状态与判定；实际刷怪参数修改在 <see cref="BattlerSpawnRateGlobalNPC"/>。
/// </remarks>
public class BattlerModPlayer : ModPlayer
{
    /// <summary>
    /// 当 <see cref="SpawnRateSliderValue"/> 变化时触发。
    /// </summary>
    /// <remarks>
    /// UI 会订阅该事件以刷新滑块显示。
    /// </remarks>
    public event EventHandler<float> SpawnRateSliderValueChanged;

    /// <summary>
    /// 刷怪倍率滑块值（归一化后，范围 0~1）。
    /// </summary>
    /// <remarks>
    /// Setter 会做一次“滑块值 -> 倍率 -> 滑块值”往返映射，让最终值对齐当前配置允许的有效刻度。
    /// </remarks>
    public float SpawnRateSliderValue
    {
        get; set
        {
            // 先投影到实际倍率区间，再映射回滑块空间，得到稳定值。
            value = InfiniteBuffHelper.RemapSliderToSpawnRate(value);
            value = InfiniteBuffHelper.RemapSpawnRateToSlider(value);

            // 数值近似不变时不触发事件，避免 UI 与网络层重复刷新。
            if (SMath.NearlyEqual(field, value)) return;

            field = value;
            OnSpawnRateSliderValueChanged(field);
        }
    } = 0.5f; // 默认置中，避免首次进入时落在极端倍率。

    /// <summary>
    /// 触发 <see cref="SpawnRateSliderValueChanged"/>。
    /// </summary>
    /// <param name="sliderValue">变更后的标准化滑块值。</param>
    private void OnSpawnRateSliderValueChanged(float sliderValue) => SpawnRateSliderValueChanged?.Invoke(this, sliderValue);

    /// <summary>
    /// 激活刷怪倍率功能所需的 Buff 组合。
    /// </summary>
    /// <remarks>
    /// 判定规则是“全包含”：组合内所有 Buff 都必须存在。
    /// </remarks>
    private HashSet<int> ActivationCombination { get; } = [
        BuffID.Sunflower,
        BuffID.Calm,
        BuffID.PeaceCandle,
        BuffID.WaterCandle,
        BuffID.Battle
    ];

    /// <summary>
    /// 判断当前 Buff 标记表是否满足激活组合。
    /// </summary>
    /// <param name="types">按 Buff ID 索引的布尔标记表。</param>
    /// <returns>组合内所有 Buff 都存在时返回 <see langword="true"/>。</returns>
    public bool MeetsActivationConditions(ReadOnlySpan<bool> types)
    {
        var combinationLength = ActivationCombination.Count;
        var count = 0;

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] && ActivationCombination.Contains(i))
            {
                if (++count == combinationLength) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 从角色存档读取滑块值。
    /// </summary>
    /// <remarks>
    /// 读取后会走属性 Setter，因此会自动执行归一流程。
    /// </remarks>
    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet<float>(nameof(SpawnRateSliderValue), out var spawnRateSliderValue))
        {
            SpawnRateSliderValue = spawnRateSliderValue;
        }
    }

    /// <summary>
    /// 将当前滑块值写入角色存档。
    /// </summary>
    /// <remarks>
    /// 仅持久化滑块值；倍率由运行时映射函数动态计算。
    /// </remarks>
    public override void SaveData(TagCompound tag)
    {
        tag[nameof(SpawnRateSliderValue)] = SpawnRateSliderValue;
    }
}
