using ImproveGame.Common.ModPlayers;

namespace ImproveGame.Common.GlobalNPCs
{
    public class BattlerSpawnTweakNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
            if (!player.TryGetModPlayer<BattlerPlayer>(out var battlerPlayer)) {
                return;
            }

            if (battlerPlayer.GetShouldDisableSpawns) {
                maxSpawns = 0;
                spawnRate = 114514;
                return;
            }

            float rate = BattlerPlayer.RemapSliderValueToPowerValue(battlerPlayer.SpawnRateSliderValue);
            spawnRate = (int)(spawnRate / rate);
            maxSpawns = (int)(maxSpawns * rate);
        }
    }
}
