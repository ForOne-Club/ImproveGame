using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using System.Collections;

namespace ImproveGame.Content.Items
{
    /// <summary>
    /// 选区物品做成一个基类，方便批量生产
    /// </summary>
    public abstract class SelectorItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            SacrificeTotal = 1;
        }

        private Point start;
        private Point end;
        protected Point SelectRange;
        protected Rectangle TileRect => new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y), (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1);

        /// <summary>
        /// 封装了原来的SetDefaults()，现在用这个，在里面应该设置SelectRange
        /// </summary>
        public virtual void SetItemDefaults() { }
        public sealed override void SetDefaults()
        {
            // 基本属性
            Item.width = 28;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.UseSound = SoundID.Item1;

            SetItemDefaults();
        }

        /// <summary>
        /// CanUseItem的替代
        /// </summary>
        public virtual bool StartUseItem(Player player) => true;

        /// <summary>
        /// 是否应该使用自由选区功能，比如说某个模式开了才能自由选区之类的
        /// </summary>
        public virtual bool CanUseSelector(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            bool flag = StartUseItem(player);
            if (!flag || !CanUseSelector(player))
            {
                return flag;
            }

            ItemRotation(player);
            unCancelled = true;
            start = Main.MouseWorld.ToTileCoordinates();
            return true;
        }

        /// <summary>
        /// 右键取消选区的实现
        /// </summary>
        private bool unCancelled;

        /// <summary>
        /// 在对框选物块执行操作后的方法，
        /// </summary>
        public virtual void PostModifyTiles(Player player, int minI, int minJ, int maxI, int maxJ) { }

        /// <summary>
        /// 对被框选的物块进行修改的方法，如果返回值为false，则会立刻终止后面的操作，默认为true
        /// </summary>
        public virtual bool ModifySelectedTiles(Player player, int i, int j) => true;

        /// <summary>
        /// 修改Box和文本的颜色
        /// </summary>
        /// <param name="cancelled">是否取消选中</param>
        /// <returns>颜色</returns>
        public virtual Color ModifyColor(bool cancelled)
        {
            return !cancelled ? new(255, 0, 0) : Color.GreenYellow;
        }

        public override bool? UseItem(Player player)
        {
            if (CanUseSelector(player) && Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer)
            {
                if (Main.mouseRight && unCancelled)
                {
                    unCancelled = false;
                }
                end = ModifySize(start, Main.MouseWorld.ToTileCoordinates(), SelectRange.X, SelectRange.Y);
                Color color = ModifyColor(!unCancelled);
                Box.NewBox(this, IsNeedKill, start, end, color * 0.35f, color, TextDisplayMode.All);
                if (Main.mouseLeft)
                {
                    player.itemAnimation = 8;
                    ItemRotation(player);
                }
                else
                {
                    player.itemAnimation = 0;
                    if (unCancelled)
                    {
                        CoroutineSystem.TileRunner.Run(ModifyTiles(player));
                    }
                }
            }
            player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation - player.direction * MathHelper.ToRadians(90f));
            return base.UseItem(player);
        }

        public virtual bool IsNeedKill() => true;

        IEnumerator ModifyTiles(Player player)
        {
            int countTiles = 0;
            Rectangle tileRect = TileRect;
            int minI = tileRect.X;
            int maxI = tileRect.X + tileRect.Width - 1;
            int minJ = tileRect.Y;
            int maxJ = tileRect.Y + tileRect.Height - 1;
            for (int j = minJ; j <= maxJ; j++)
            {
                for (int i = minI; i <= maxI; i++)
                {
                    countTiles++;
                    if (WorldGen.InWorld(i, j) && !ModifySelectedTiles(player, i, j))
                    {
                        PostModifyTiles(player, minI, minJ, i, j);
                        yield break;
                    }
                    if (countTiles >= 30)
                    {
                        countTiles = 0;
                        yield return 0;
                    }
                }
            }
            yield return 0;
            PostModifyTiles(player, minI, minJ, maxI, maxJ);
        }
    }
}
