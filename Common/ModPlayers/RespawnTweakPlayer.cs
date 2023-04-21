namespace ImproveGame.Common.ModPlayers
{
    public class RespawnTweakPlayer : ModPlayer
    {
        public int respawnFullHPTimer = 0;

        public override void OnRespawn() {
            if (Config.RespawnWithFullHP) {
                respawnFullHPTimer = 1;
            }
        }

        // 为了让满血复活获得Buff效果，在PostUpdate中隔一帧执行
        public override void PostUpdate() {
            if (Config.RespawnWithFullHP && respawnFullHPTimer == 0) {
                respawnFullHPTimer = -1;
                Player.statLife = Player.statLifeMax2;
                Player.statMana = Player.statManaMax2;
            }
            respawnFullHPTimer--;
        }
    }
}
