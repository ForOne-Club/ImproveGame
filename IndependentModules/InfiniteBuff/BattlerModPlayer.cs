using ImproveGame.Content.Functions.PortableBuff;
using Terraria.ModLoader.IO;

namespace ImproveGame.IndependentModules.InfiniteBuff;

public class BattlerModPlayer : ModPlayer
{
    public event EventHandler<float> SpawnRateSliderValueChanged;
    private void OnSpawnRateSliderValueChanged(float value) => SpawnRateSliderValueChanged?.Invoke(this, value);

    /// <summary>
    /// 生成速率滑杆值
    /// </summary>
    public float SpawnRateSliderValue
    {
        get; set
        {
            if (field == value) return;

            // 猜猜猜猜猜猜猜猜猜猜猜猜猜猜猜猜猜猜
            // 这段奇怪怪怪怪怪怪怪怪怪怪怪怪怪怪怪
            // 的的的的的的的的的的的的的的的的代码
            // 是干嘛嘛嘛嘛嘛嘛嘛嘛嘛嘛嘛嘛嘛嘛嘛的
            value = InfiniteBuffHelper.RemapSliderToSpawnRate(value);
            value = InfiniteBuffHelper.RemapSpawnRateToSlider(value);

            field = value;
            OnSpawnRateSliderValueChanged(field);
        }
    } = 0.5f;

    /// <summary>
    /// 激活组合
    /// </summary>
    public HashSet<int> ActivationCombination { get; } = [
        BuffID.Sunflower,
        BuffID.Calm,
        BuffID.PeaceCandle,
        BuffID.WaterCandle,
        BuffID.Battle
    ];

    /// <summary>
    /// 满足激活条件
    /// </summary>
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

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet<float>(nameof(SpawnRateSliderValue), out var spawnRateSliderValue))
        {
            SpawnRateSliderValue = spawnRateSliderValue;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(SpawnRateSliderValue)] = SpawnRateSliderValue;
    }
}