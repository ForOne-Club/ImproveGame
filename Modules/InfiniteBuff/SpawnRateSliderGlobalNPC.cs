namespace ImproveGame.Modules.InfiniteBuff;

public class SpawnRateSliderGlobalNPC : GlobalNPC
{
    public override void Load()
    {
        // SlimeRain 史莱姆雨
        On_NPC.Spawner.SlimeRainSpawns += (orig, plr) =>
        {
            if (!plr.TryGetModPlayer<SpawnRateSliderModPlayer>(out var battler) ||
                !plr.TryGetModPlayer<InfiniteBuffModPlayer>(out var infinitePlayer) ||
                !infinitePlayer.MeetsBattlerCombination())
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
        if (!player.TryGetModPlayer<SpawnRateSliderModPlayer>(out var battler)) return;
        if (!player.TryGetModPlayer<InfiniteBuffModPlayer>(out var infinitePlayer)) return;
        if (!infinitePlayer.MeetsBattlerCombination()) return;

        var rate = InfiniteBuffHelper.RemapSliderToSpawnRate(battler.SpawnRateSliderValue);

        if (rate == 0f)
        {
            spawnRate += 114514;
            maxSpawns = 0;
            return;
        }

        spawnRate = (int)Math.Round(spawnRate / rate);
        maxSpawns = (int)Math.Round(maxSpawns * rate);
    }
}
