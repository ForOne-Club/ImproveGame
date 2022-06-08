using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item item = MyUtils.GetFirstWall(Player);
            // 背包中没有墙结束弹幕
            if (item.type == ItemID.None)
            {
                CombatText.NewText(Projectile.getRect(), new Color(225, 0, 0),
                    MyUtils.GetText("CombatText_Projectile.PlaceWall_Lack"));
                Projectile.Kill();
            }
            Projectile.timeLeft++;
            if (Index < Walls.Count)
            {
                if (item.consumable || ItemLoader.ConsumeItem(item, Player))
                {
                    item.stack--;
                }
                Projectile proj = Projectile.NewProjectileDirect(null,
                    Projectile.position, (Walls[Index].ToVector2() * 16f - Projectile.Center) / 10f,
                    ModContent.ProjectileType<PlaceWall2>(), 0, 0, Projectile.owner);
                proj.ai[1] = item.createWall;
                proj.Center = Projectile.Center + new Vector2(DrawOriginOffsetX, DrawOriginOffsetY);
                proj.rotation = proj.velocity.ToRotation();
                ((PlaceWall2)proj.ModProjectile).WallPosition = Walls[Index];
                Projectile.rotation = proj.rotation + MathF.PI;
                Index++;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t2d = TextureAssets.Projectile[ModContent.ProjectileType<PlaceWall2>()].Value;
            return base.PreDraw(ref lightColor);
        }
    }
}
