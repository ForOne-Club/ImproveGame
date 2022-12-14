using ImproveGame.Common.Animations;

namespace ImproveGame.Content.Projectiles
{
    public class WallRobot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 100000;
            // DrawOriginOffsetX = 3;
        }

        public List<Point> Walls = new();

        public int Index
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

        public Player Player
        {
            get
            {
                return Main.player[Projectile.owner];
            }
        }

        public override void AI()
        {
            Item item = FirstWall(Player);
            Lighting.AddLight(Projectile.Center, 45 / 510f, 206 / 510f, 255 / 510f);

            if (item is null)
            {
                CombatText.NewText(Projectile.getRect(), new Color(225, 0, 0), GetText("CombatText_Projectile.PlaceWall_Lack"));
                Projectile.Kill();
                return;
            }

            if (Index < Walls.Count)
            {
                Point wall = Walls[Index++];

                BongBong(wall.ToVector2() * 16, 16, 16);

                if (item.consumable || ItemLoader.ConsumeItem(item, Player))
                    item.stack--;

                if (Main.tile[wall.X, wall.Y].WallType > 0 && Main.tile[wall.X, wall.Y].WallType != item.createWall)
                    WorldGen.KillWall(wall.X, wall.Y);

                WorldGen.PlaceWall(wall.X, wall.Y, item.createWall);

                NetMessage.SendTileSquare(Projectile.owner, wall.X, wall.Y);
                Projectile.rotation = (wall.ToVector2() * 16f + new Vector2(8) - Projectile.Center).ToRotation() + MathF.PI;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public Color background = new(45, 206, 255);
        public Color border = new(66, 117, 186);

        public override bool PreDraw(ref Color lightColor)
        {
            if (Index < Walls.Count)
            {
                Vector2 center = Projectile.Center - Main.screenPosition;
                Vector2 target = Walls[Index - 1].ToVector2() * 16f + new Vector2(8) - Main.screenPosition;
                PixelShader.DrawLine(Main.GameViewMatrix.TransformationMatrix, center, target, 2f, background, 1f, border);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 10; i++)
                BongBong(Projectile.position, Projectile.width, Projectile.height);
        }
    }
}
