namespace ImproveGame.Content.Projectiles
{
    public class HomeEffect : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
        }
        public override string Texture => "ImproveGame/Content/Projectiles/KillProj";
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item6, player.position);

            for (int l = 0; l < 70; l++)
            {
                Dust.NewDust(Projectile.position, player.width, player.height, DustID.MagicMirror, player.velocity.X * 0.5f,
                    player.velocity.Y * 0.5f, 150, default, 1.5f);
            }

            for (int m = 0; m < 70; m++)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
            }
        }
    }
}
