using ImproveGame.Common.Animations;
using Microsoft.VisualBasic;
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
            Item item = GetFirstWall(Player);
            // 背包中没有墙结束弹幕
            if (item is null)
            {
                CombatText.NewText(Projectile.getRect(), new Color(225, 0, 0),
                    GetText("CombatText_Projectile.PlaceWall_Lack"));
                Projectile.Kill();
            }
            if (Index < Walls.Count)
            {
                Point wall = Walls[Index++];

                BongBong(wall.ToVector2() * 16, 16, 16);

                if (Main.tile[wall.X, wall.Y].WallType == item.createWall)
                    return;

                if (item.consumable || ItemLoader.ConsumeItem(item, Player))
                    item.stack--;
                WorldGen.PlaceWall(wall.X, wall.Y, item.createWall);

                if (Main.tile[wall.X, wall.Y].WallType > 0)
                    WorldGen.KillWall(wall.X, wall.Y);

                NetMessage.SendTileSquare(Projectile.owner, wall.X, wall.Y);
                Projectile.rotation = (wall.ToVector2() * 16f + new Vector2(8) - Projectile.Center).ToRotation() + MathF.PI;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Index < Walls.Count)
            {
                Vector2 center = Projectile.Center - Main.screenPosition;
                PixelShader.DrawLine(Main.GameViewMatrix.EffectMatrix, center, center,
                    Walls[Index - 1].ToVector2() * 16f + new Vector2(8) - Main.screenPosition, 2, new Color(0, 155, 255));
            }
            return true;
        }
    }
}
