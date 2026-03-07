using SilkyUIFramework;
using Terraria.ModLoader.IO;

namespace ImproveGame.Modules.InfiniteBuff;

public class SpawnRateSliderValueModPlayer : ModPlayer
{
    public event EventHandler<float> SpawnRateSliderValueChanged;

    /// <summary>
    /// 刷怪倍率滑块值, 范围 [0,1]
    /// </summary>
    public float SpawnRateSliderValue
    {
        get; set
        {
            // 先投影到实际倍率区间，再映射回滑块空间，得到稳定值。
            value = InfiniteBuffHelper.RemapSliderToSpawnRate(value);
            value = InfiniteBuffHelper.RemapSpawnRateToSlider(value);

            if (SMath.NearlyEqual(field, value)) return;

            field = value;
            OnSpawnRateSliderValueChanged(field);
        }
    }
    private void OnSpawnRateSliderValueChanged(float sliderValue) =>
        SpawnRateSliderValueChanged?.Invoke(this, sliderValue);

    /// <summary>
    /// 从角色存档读取滑块值。
    /// </summary>
    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet<float>(nameof(SpawnRateSliderValue), out var spawnRateSliderValue))
        {
            SpawnRateSliderValue = spawnRateSliderValue;
        }
        else SpawnRateSliderValue = 0.5f;
    }

    /// <summary>
    /// 将当前滑块值写入角色存档。
    /// </summary>
    public override void SaveData(TagCompound tag)
    {
        tag[nameof(SpawnRateSliderValue)] = SpawnRateSliderValue;
    }
}
