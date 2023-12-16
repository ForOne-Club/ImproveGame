using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Common.GlobalProjectiles
{
    public class ImproveProjectile : GlobalProjectile
    {
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            // 如果开启了禁止爆炸物自伤，则视条件决定是否阻止造成伤害
            if (Config.BombsNotHurtPlayer && ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[projectile.type] && projectile.owner == target.whoAmI) return false;
            // 默认返回
            return true;
        }
    }
}
