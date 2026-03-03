using ImproveGame.Content.Functions.PortableBuff;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ModPlayers;

public class BattlerPlayer : ModPlayer
{
    public const float SliderDefaultValue = 0.5f;

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
            value = RemapSliderToSpawnRate(value);
            value = RemapSpawnRateToSlider(value);

            field = value;
            OnSpawnRateSliderValueChanged(field);
        }
    } = SliderDefaultValue;

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

    public bool GetShouldDisableSpawns => SpawnRateSliderValue == 0f;

    //public override void OnEnterWorld()
    //{
    //    SpawnRateSliderValue = SliderDefaultValue;
    //    UISystem.Instance.BuffTrackerGUI.BuffTrackerBattler.ResetDataForNewPlayer(Player.whoAmI);
    //    // 被移到UIPlayer了
    //}

    private readonly List<int> BattlerRequiredBuffs = [
        BuffID.Sunflower,
        BuffID.Calm,
        BuffID.PeaceCandle,
        BuffID.WaterCandle,
        BuffID.Battle
    ];

    public bool HasRequiredBuffs()
    {
        int buffsCount = 0;
        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            if (HideBuffSystem.BuffTypesShouldHide[i] && BattlerRequiredBuffs.Contains(i))
            {
                buffsCount++;
            }
        }
        return buffsCount == BattlerRequiredBuffs.Count;
    }

    /// <summary>
    /// 重映射滑块至生成率
    /// </summary>
    public static float RemapSliderToSpawnRate(float value)
    {
        var min = Config.SpawnRateMinValue;
        var max = Config.SpawnRateMaxValue;

        // 直接返回
        if (min >= max) return max;

        value = Math.Clamp(value, 0f, 1f);

        if (min < 1)
        {
            value = value * 2f;

            if (value < 1f)
            {
                return MathF.Round(MathHelper.Lerp(min, 1, value), 2);
            }

            value -= 1f;
            return MathF.Round(MathHelper.Lerp(1, max, value));
        }

        return MathF.Round(MathHelper.Lerp(min, max, value));
    }

    /// <summary>
    /// 将生成率重新映射到滑块
    /// </summary>
    public static float RemapSpawnRateToSlider(float rate)
    {
        var min = Config.SpawnRateMinValue;
        var max = Config.SpawnRateMaxValue;

        // 直接返回
        if (min >= max) return 0.5f;
        if (max < 1) return 0.5f;

        rate = Math.Clamp(rate, min, max);

        // 最小值小于 1
        if (min < 1)
        {
            if (rate < 1)
            {
                rate = MathF.Round(rate, 2);

                return Math.Clamp((rate - min) / (1 - min) / 2f, 0f, 0.5f);
            }

            rate = MathF.Round(rate);

            return Math.Clamp(((rate - 1) / (max - 1) + 1f) / 2f, 0.5f, 1f);
        }

        rate = MathF.Round(rate, 2);

        return Math.Clamp((rate - min) / (max - min), 0f, 1f);
    }

    public override void Load()
    {
        // TweakSlimeRain
        On_NPC.SlimeRainSpawns += (orig, plr) =>
        {
            if (!Main.player[plr].TryGetModPlayer<BattlerPlayer>(out var battlerPlayer) || battlerPlayer.GetShouldDisableSpawns)
            {
                orig.Invoke(plr);
                return;
            }
            float rate = RemapSliderToSpawnRate(battlerPlayer.SpawnRateSliderValue);
            if (rate >= 1f)
            {
                // 我直接多运行几次
                for (int i = 0; i < rate; i++)
                {
                    orig.Invoke(plr);
                }
            }
            else if (Main.rand.NextFloat() <= rate)
            {
                orig.Invoke(plr);
            }
        };
    }
}
