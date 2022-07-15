using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Terraria.GameContent.Creative;

namespace ImproveGame.Content.Items
{
    /// <summary>
    /// 选区物品做成一个基类，方便批量生产
    /// </summary>
    public abstract class SelectorItem : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => MyUtils.Config.LoadModItems;

        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        private Point Start;
        private Point End;
        protected Point SelectRange;
        protected Rectangle TileRect => new((int)MathF.Min(Start.X, End.X), (int)MathF.Min(Start.Y, End.Y), (int)MathF.Abs(Start.X - End.X) + 1, (int)MathF.Abs(Start.Y - End.Y) + 1);

        /// <summary>
        /// 封装了原来的SetDefaults()，现在用这个，在里面应该设置SelectRange
        /// </summary>
        public virtual void SetItemDefaults() { }
        public sealed override void SetDefaults() {
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

        public override bool CanUseItem(Player player) {
            bool flag = StartUseItem(player);
            if (flag && CanUseSelector(player)) {
                MyUtils.ItemRotation(player);
                _unCancelled = true;
                Start = Main.MouseWorld.ToTileCoordinates();
            }
            return flag;
        }

        /// <summary>
        /// 右键取消选区的实现
        /// </summary>
        private bool _unCancelled;

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
        public virtual Color ModifyColor(bool cancelled) {
            if (!cancelled)
                return new(255, 0, 0);
            else
                return Color.GreenYellow;
        }

        public override bool? UseItem(Player player) {
            if (CanUseSelector(player)&& Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer) {
                if (Main.mouseRight && _unCancelled) {
                    _unCancelled = false;
                }
                End = MyUtils.ModifySize(Start, Main.MouseWorld.ToTileCoordinates(), SelectRange.X, SelectRange.Y);
                Color color = ModifyColor(!_unCancelled);
                int boxIndex = Box.NewBox(Start, End, color * 0.35f, color);
                if (boxIndex is not -1) {
                    Box box = DrawSystem.boxs[boxIndex];
                    box.ShowWidth = true;
                    box.ShowHeight = true;
                }
                if (Main.mouseLeft) {
                    player.itemAnimation = 8;
                    MyUtils.ItemRotation(player);
                }
                else {
                    player.itemAnimation = 0;
                    if (_unCancelled) {
                        CoroutineSystem.TileRunner.Run(ModifyTiles(player));
                    }
                }
            }
            player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation - player.direction * MathHelper.ToRadians(90f));
            return base.UseItem(player);
        }

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
                    if (!ModifySelectedTiles(player, i, j))
                    {
                        PostModifyTiles(player, minI, minJ, i, j);
                        yield break;
                    }
                    if (countTiles >= 30) {
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
