using ImproveGame.Content.Functions.PortableBuff;

namespace ImproveGame.Common.ModPlayers
{
    public class BattlerPlayer : ModPlayer
    {
        internal const float SliderDefaultValue = 0.5f;


        public event EventHandler<float> SpawnRateSliderValueChanged;
        private void OnSpawnRateSliderValueChanged(float value) => SpawnRateSliderValueChanged?.Invoke(this, value);

        public float SpawnRateSliderValue
        {
            get; set
            {
                if (field == value) return;
                field = value;
                OnSpawnRateSliderValueChanged(field);
            }
        } = SliderDefaultValue;

        public bool GetShouldDisableSpawns => SpawnRateSliderValue == 0f;

        public override void OnEnterWorld()
        {
            SpawnRateSliderValue = SliderDefaultValue;
            // UISystem.Instance.BuffTrackerGUI.BuffTrackerBattler.ResetDataForNewPlayer(Player.whoAmI);
            // 被移到UIPlayer了
        }

        private readonly List<int> BattlerRequiredBuffs = new() {
            BuffID.Sunflower,
            BuffID.Calm,
            BuffID.PeaceCandle,
            BuffID.WaterCandle,
            BuffID.Battle
        };

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

        public static float RemapSliderValueToPowerValue(float sliderValue)
        {
            float rateMax = Config.SpawnRateMaxValue;
            float rateMin = Config.SpawnRateMinValue;
            if (rateMin > rateMax) rateMin = rateMax;
            float rateMid;
            if (rateMin < 1)
                rateMid = 1;
            else if (rateMax < 10)
                rateMid = rateMin > 1 ? (rateMax + rateMin) * .5f : 1;
            else
                rateMid = 10;
            float remappedValue = TrUtils.Remap(sliderValue, 0.5f, 1f, rateMid, rateMax);
            remappedValue = (float)Math.Round(remappedValue); // 取整
            if (sliderValue < 0.5f)
            {
                remappedValue = TrUtils.Remap(sliderValue, 0f, 0.5f, rateMin, rateMid);
                remappedValue = (float)Math.Round(remappedValue * 20f) / 20f; // 0.5显示，不然强迫症了
            }
            return remappedValue;
        }

        public override void Load()
        {
            On_NPC.SlimeRainSpawns += TweakSlimeRain;
        }

        private void TweakSlimeRain(On_NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (!Main.player[plr].TryGetModPlayer<BattlerPlayer>(out var battlerPlayer) || battlerPlayer.GetShouldDisableSpawns)
            {
                orig.Invoke(plr);
                return;
            }
            float rate = RemapSliderValueToPowerValue(battlerPlayer.SpawnRateSliderValue);
            if (rate >= 1f)
            { // 我直接多运行几次
                for (int i = 0; i < rate; i++)
                {
                    orig.Invoke(plr);
                }
            }
            else if (Main.rand.NextFloat() <= rate)
            {
                orig.Invoke(plr);
            }
        }
    }
}
