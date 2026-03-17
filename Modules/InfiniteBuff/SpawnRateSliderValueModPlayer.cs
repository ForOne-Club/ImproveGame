using ImproveGame.Packets;
using SilkyUIFramework;
using Terraria.ModLoader.IO;

namespace ImproveGame.Modules.InfiniteBuff;

public class SpawnRateSliderValueModPlayer : ModPlayer
{
    public event EventHandler<bool> ShowSliderChanged;

    public bool ShowSlider
    {
        get; set
        {
            if (ShowSlider == value) return;

            field = value;
            ShowSliderChanged?.Invoke(this, field);
        }
    }

    public event EventHandler<float> SpawnRateSliderValueChanged;

    /// <summary>
    /// 刷怪倍率滑块值, 范围 [0,1]
    /// </summary>
    public float SpawnRateSliderValue
    {
        get; set
        {
            // 先投影到实际倍率区间，再映射回滑块空间，得到稳定值。
            value = MathF.Round(value / 0.01f) * 0.01f;
            if (SMath.NearlyEqual(field, value)) return;

            field = value;
            SpawnRateSliderValueChanged?.Invoke(this, field);
        }
    }

    public void SetSpawnRateSliderValue(float value)
    {
        SpawnRateSliderValue = value;
        SpawnRateSlider.Get(Player.whoAmI, SpawnRateSliderValue).Send();
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) => SpawnRateSlider.Get(Player.whoAmI, SpawnRateSliderValue).Send();

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet<float>(nameof(SpawnRateSliderValue), out var spawnRateSliderValue))
        {
            SpawnRateSliderValue = spawnRateSliderValue;
        }
        else SpawnRateSliderValue = 0.5f;

        if (tag.TryGet<bool>(nameof(ShowSlider), out var showSlider))
        {
            ShowSlider = showSlider;
        }
        else ShowSlider = true;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(SpawnRateSliderValue)] = SpawnRateSliderValue;
        tag[nameof(ShowSlider)] = ShowSlider;
    }
}
