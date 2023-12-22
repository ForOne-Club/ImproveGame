using Terraria.DataStructures;
using static Humanizer.In;

namespace ImproveGame.Common.GlobalProjectiles
{
    public class ImproveProjectile : GlobalProjectile
    {
        public override bool? CanDamage(Projectile projectile)
        {
            // 禁止爆炸物伤害（我是超激打表大师）
            if (Config.BombsNotDamage is BombsNotDamageType.Item && projectile.type is 28 or 29 or 37 or 108 or 470 or 
                516 or 519 or 637 or 773 or 903 or 904 or 905 or 906 or 910 or 911)
                return false;

            // 禁止爆炸物+火箭伤害（进行一个表的打！）
            if (Config.BombsNotDamage is BombsNotDamageType.ItemAndRocket && projectile.type is 28 or 29 or 37 or 108 or
                136 or 137 or 138 or 142 or 143 or 144 or 470 or 516 or 519 or 637 or 773 or 780 or 781 or 782 or 783 or
                784 or 785 or 786 or 787 or 788 or 789 or 790 or 791 or 792 or 796 or 797 or 798 or 799 or 800 or 801 or 
                903 or 904 or 905 or 906 or 910 or 911)
                return false;

            // 默认返回
            return null;
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // 更强、更大的环境改造枪
            if (Config.ClentaminatorPlus && source is EntitySource_ItemUse_WithAmmo itemUse && itemUse.Item.type is ItemID.Clentaminator)
            {
                // 原版默认133帧，这里翻三倍，也就是400
                projectile.timeLeft = 400;
                projectile.extraUpdates += 4;
                // 重设大小没什么意义，只会影响粒子特效
                projectile.Resize(projectile.width * 16, projectile.height * 16);
            }
        }
        public override bool PreAI(Projectile projectile)
        {
            #region 原版环境改造枪弹幕写的是真离谱，我直接在PreAI魔改成我想要的样子得了
            if (Config.ClentaminatorPlus && projectile.aiStyle == 31)
            {
                bool flag11 = projectile.ai[1] == 1f;
                short num272 = 110;
                int num273 = 0;
                switch (projectile.type)
                {
                    default:
                        num272 = 110;
                        num273 = 0;
                        break;
                    case 147:
                        num272 = 112;
                        num273 = 1;
                        break;
                    case 146:
                        num272 = 111;
                        num273 = 2;
                        break;
                    case 148:
                        num272 = 113;
                        num273 = 3;
                        break;
                    case 149:
                        num272 = 114;
                        num273 = 4;
                        break;
                    case 1015:
                        num272 = 311;
                        num273 = 5;
                        break;
                    case 1016:
                        num272 = 312;
                        num273 = 6;
                        break;
                    case 1017:
                        num272 = 313;
                        num273 = 7;
                        break;
                }

                if (projectile.owner == Main.myPlayer)
                {
                    // 这里才是实现更大的环境改造枪的关键
                    int size = 4;
                    if (flag11)
                        size = 5;

                    Point point = projectile.Center.ToTileCoordinates();
                    WorldGen.Convert(point.X, point.Y, num273, size);
                }

                float num275 = 1f;

                int num276 = 0;
                if (flag11)
                {
                    num275 *= 1.2f;
                    num276 = (int)(12f * num275);
                }

                projectile.ai[0]++;

                int num278 = Dust.NewDust(new Vector2(projectile.position.X - 20 - num276, projectile.position.Y - 20 - num276), projectile.width + 20 + num276 * 2, projectile.height + 20 + num276 * 2, num272, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100);
                Main.dust[num278].noGravity = true;
                Dust dust2 = Main.dust[num278];
                dust2.scale *= 1.75f;
                Main.dust[num278].velocity.X *= 2f;
                Main.dust[num278].velocity.Y *= 2f;
                dust2 = Main.dust[num278];
                dust2.scale *= num275;

                projectile.rotation += 0.3f * projectile.direction;
                return false;
            }
            #endregion
            return true;
        }
    }
}
