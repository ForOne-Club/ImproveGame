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
    }
}
