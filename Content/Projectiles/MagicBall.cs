using ImproveGame.Common.Animations;

namespace ImproveGame.Content.Projectiles
{
    public class MagicBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Vector2 vel = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 25f;
            Projectile.velocity = (vel + Projectile.velocity * 9f) / 10f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i + 1] == Vector2.Zero)
                    continue;
                Vector2 newPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Vector2 oldPos = Projectile.oldPos[i + 1] + Projectile.Size / 2 - Main.screenPosition;
                PixelShader.DrawLine(newPos, oldPos, 4, Color.White * (1f - (i / 8f)), false);
            }
            return false;
        }
    }
}
