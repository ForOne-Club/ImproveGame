namespace ImproveGame.IndependentModules.InfiniteBuff;

public class BattlerSpawnRateGlobalNPC : GlobalNPC
{
    public override void Load()
    {
        // SlimeRain 史莱姆雨
        On_NPC.SlimeRainSpawns += (orig, plr) =>
        {
            if (!Main.player[plr].TryGetModPlayer<BattlerModPlayer>(out var battler) ||
                !battler.MeetsActivationConditions(HideBuffSystem.HideFlags))
            {
                orig.Invoke(plr);
                return;
            }

            var rate = InfiniteBuffHelper.RemapSliderToSpawnRate(battler.SpawnRateSliderValue);
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

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (!player.TryGetModPlayer<BattlerModPlayer>(out var battler)) return;
        if (!battler.MeetsActivationConditions(HideBuffSystem.HideFlags)) return;

        if (battler.SpawnRateSliderValue == 0f)
        {
            spawnRate += 114514;
            maxSpawns = 0;
            return;
        }

        var rate = InfiniteBuffHelper.RemapSliderToSpawnRate(battler.SpawnRateSliderValue);
        spawnRate = (int)(spawnRate / rate);
        maxSpawns = (int)(maxSpawns * rate);
    }
}
