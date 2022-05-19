using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

namespace ImproveGame.Content.Projectiles
{
    public class PlaceWall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
            DrawOriginOffsetX = 3;
        }

        public List<Point> Walls = new();

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

        public int WP
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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.timeLeft++;
            if (Timer % 2 == 0)
            {
                if (WP < Walls.Count)
                {
                    Item item = Utils.GetFirstWall(player);
                    if (item.type == ItemID.None)
                    {
                        CombatText.NewText(Projectile.getRect(), new Color(225, 0, 0),
                            Language.GetTextValue($"Mods.ImproveGame.CombatText_Projectile.PlaceWall_Lack"));
                        Projectile.Kill();
                    }
                    if (item.consumable || ItemLoader.ConsumeItem(item, player))
                    {
                        item.stack--;
                    }
                    Projectile proj = Projectile.NewProjectileDirect(null, Projectile.position, (Walls[WP].ToVector2() * 16f - Projectile.Center) / 10f,
                        ModContent.ProjectileType<PlaceWall2>(), 0, 0, Projectile.owner);
                    proj.ai[1] = item.createWall;
                    proj.Center = Projectile.Center + new Vector2(DrawOriginOffsetX, DrawOriginOffsetY);
                    proj.rotation = proj.velocity.ToRotation();
                    ((PlaceWall2)proj.ModProjectile).WallPosition = Walls[WP];
                    Projectile.rotation = proj.rotation + MathF.PI;
                    // WorldGen.PlaceWall(Walls[WP].X, Walls[WP].Y, 1, false);
                    WP++;
                }
                if (!(WP < Walls.Count))
                {
                    Projectile.Kill();
                }
            }
            Timer++;
        }
    }
}
