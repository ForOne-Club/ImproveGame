namespace ImproveGame.Content.Projectiles
{
    public class PlaceWall2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 8;
        }

        public int Timer
        {
            get
            {
                return (int)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = value;
            }
        }

        public int WallType
        {
            get
            {
                return (int)Projectile.ai[1];
            }
            set
            {
                Projectile.ai[1] = value;
            }
        }

        public Point WallPosition = Point.Zero;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer == 9)
            {
                if (Main.tile[WallPosition.X, WallPosition.Y].WallType > 0)
                {
                    WorldGen.KillWall(WallPosition.X, WallPosition.Y);
                }
                WorldGen.PlaceWall(WallPosition.X, WallPosition.Y, WallType, false);
                NetMessage.SendTileSquare(Projectile.owner, WallPosition.X, WallPosition.Y);
                Projectile.Kill();
            }
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            int length = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            Texture2D t2d = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = Projectile.Size * 0.5f;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 pos = Projectile.oldPos[i] + origin - Main.screenPosition;
                sb.Draw(t2d, pos, null, lightColor * ((length - i) / (float)length), Projectile.oldRot[i], origin, Projectile.scale, 0, 0f);
            }
            return base.PreDraw(ref lightColor);
        }
    }
}
