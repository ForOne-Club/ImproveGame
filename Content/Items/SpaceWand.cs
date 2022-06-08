using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImproveGame.Content.Items
{
    public class SpaceWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return MyUtils.Config().LoadModItems;
        }

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.value = Item.sellPrice(0, 0, 50, 0);

            Item.mana = 20;
        }

        Point start = Point.Zero;
        Point end = Point.Zero;

        public override bool CanUseItem(Player player)
        {
            int count = 0;
            MyUtils.GetPlatformCount(player.inventory, ref count);
            if (count < 1)
            {
                return false;
            }
            return base.CanUseItem(player);
        }
        /// <summary>
        /// 待起名
        /// </summary>
        public bool _flag = true;
        /// <summary>
        /// 能否放置平台
        /// </summary>
        public bool _allowPlacePlatform;

        public override bool? UseItem(Player player)
        {
            UseItem_PreUpdate(player);
            if (_flag && Main.mouseLeft)
            {
                _flag = false;
                UseItem_LeftMouseDown(player);
            }
            else if (!_flag && !Main.mouseLeft)
            {
                _flag = true;
                player.itemAnimation = 0;
                UseItem_LeftMouseUp(player);
            }
            UseItem_PostUpdate(player);
            return false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_PreUpdate(Player player)
        {
            player.itemAnimation = player.itemAnimationMax / 2;

            MyUtils.ItemRotation(player);

            // 开启绘制
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                end = Main.MouseWorld.ToTileCoordinates();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_PostUpdate(Player player)
        {
            // 开启绘制
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                if (Main.mouseRight && _allowPlacePlatform)
                {
                    _allowPlacePlatform = false;
                    CombatText.NewText(player.getRect(), new Color(250, 40, 80), Language.GetTextValue($"Mods.ImproveGame.CombatText_Item.SpaceWand_Cancel"));
                }
                Color color;
                if (_allowPlacePlatform)
                    color = new Color(135, 0, 180);
                else
                    color = new Color(250, 40, 80);
                int box = Box.NewBox(GetPlatfromRect(player), color * 0.35f, color);
                DrawSystem.boxs[box].ShowWidth = true;
            }
        }

        /// <summary>
        /// 左键点击
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_LeftMouseDown(Player player)
        {
            start = Main.MouseWorld.ToTileCoordinates();
            _allowPlacePlatform = true;
        }

        /// <summary>
        /// 左键释放
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_LeftMouseUp(Player player)
        {
            // 放置平台
            if (_allowPlacePlatform && player.whoAmI == Main.myPlayer)
            {
                Rectangle platfromRect = GetPlatfromRect(player);
                int minI = platfromRect.X;
                int maxI = platfromRect.X + platfromRect.Width - 1;
                int minJ = platfromRect.Y;
                int maxJ = platfromRect.Y + platfromRect.Height - 1;
                // 处理图块
                for (int i = minI; i <= maxI; i++)
                {
                    for (int j = minJ; j <= maxJ; j++)
                    {
                        // 找到背包第一个平台
                        Item PlatformItem = MyUtils.GetFirstPlatform(player);
                        // 如果有找到平台结束执行
                        if (PlatformItem.type == ItemID.None)
                        {
                            CombatText.NewText(player.getRect(), new Color(255, 0, 0, 255), Language.GetTextValue($"Mods.ImproveGame.CombatText_Item.SpaceWand_Lack"));
                            return;
                        }
                        // 破坏物块
                        if (player.TileReplacementEnabled && MyUtils.IsSameTile(i, j, PlatformItem.createTile, PlatformItem.placeStyle))
                            MyUtils.TryKillTile(i, j, player, player.GetBestPickaxe());
                        // 放置成功，是消耗品，可以被消耗 扣除一个
                        if (WorldGen.PlaceTile(i, j, PlatformItem.createTile, false, Main.tile[i, j].TileType == 0 ? true : false, player.whoAmI, PlatformItem.placeStyle)
                            && ItemLoader.ConsumeItem(PlatformItem, player) && PlatformItem.consumable)
                        {
                            PlatformItem.stack--;
                        }
                    }
                }
                // 发送数据到服务器
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendTileSquare(player.whoAmI, minI, minJ, maxI - minI + 1, maxJ - minJ + 1);
            }
        }

        public Rectangle GetPlatfromRect(Player player)
        {
            // 平台数量
            int platfromCount = 0;
            if (!MyUtils.GetPlatformCount(player.inventory, ref platfromCount) || platfromCount > 500)
            {
                platfromCount = 500;
            }
            end = MyUtils.LimitRect(start, end, platfromCount, 1);
            Rectangle rect = new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y),
                 (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1);
            return rect;
        }
    }
}
