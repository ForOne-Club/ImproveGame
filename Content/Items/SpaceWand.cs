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
            Item.value = Item.sellPrice(0, 1, 0, 0);

            Item.mana = 50;
        }

        Point StartPosition = Point.Zero;
        Point EndPosition = Point.Zero;

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
            player.itemAnimation = player.itemAnimationMax / 2;
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
            UseItem_Update(player);
            return false;
        }

        /// <summary>
        /// 左键点击
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_LeftMouseDown(Player player)
        {
            StartPosition = Main.MouseWorld.ToTileCoordinates();
            _allowPlacePlatform = true;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_Update(Player player)
        {
            // 旋转物品
            Vector2 rotaion = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
            player.direction = Main.MouseWorld.X < player.Center.X ? -1 : 1;
            player.itemRotation = MathF.Atan2(rotaion.Y * player.direction, rotaion.X * player.direction);

            // 指定结束位置
            EndPosition = Main.MouseWorld.ToTileCoordinates();

            // 平台数量
            int platfromCount = 0;
            if (!MyUtils.GetPlatformCount(player.inventory, ref platfromCount))
            {
                platfromCount = 500;
            }
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

                Rectangle rect = new();
                rect.X = (int)MathF.Min(StartPosition.X, EndPosition.X);
                if (rect.X < StartPosition.X - MathF.Min(499, platfromCount - 1))
                {
                    rect.X = StartPosition.X - (int)MathF.Min(499, platfromCount - 1);
                }
                rect.Y = StartPosition.Y;
                rect.Width = (int)MathF.Min(MathF.Abs(StartPosition.X - EndPosition.X) + 1, MathF.Min(500, platfromCount));
                rect.Height = 1;

                TileDraw.tileColor = color;
                TileDraw.tileRect = rect;
                TileDraw.allowDrawBorderRect = true;
            }
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
                int minI = TileDraw.tileRect.X;
                int maxI = TileDraw.tileRect.X + TileDraw.tileRect.Width - 1;
                int minJ = TileDraw.tileRect.Y;
                int maxJ = TileDraw.tileRect.Y + TileDraw.tileRect.Height - 1;
                // 处理图块
                for (int i = minI; i <= maxI; i++)
                {
                    for (int j = minJ; j <= maxJ; j++)
                    {
                        // 找到背包第一个平台
                        Item PlatformItem = MyUtils.GetFirstPlatform(player);
                        Item BestPickaxe = player.GetBestPickaxe();
                        // 如果有找到平台结束执行
                        if (PlatformItem.type == ItemID.None)
                        {
                            CombatText.NewText(player.getRect(), new Color(255, 0, 0, 255), Language.GetTextValue($"Mods.ImproveGame.CombatText_Item.SpaceWand_Lack"));
                            return;
                        }
                        // 破坏物块
                        if (!Main.tileHammer[Main.tile[i, j].TileType] && player.TileReplacementEnabled // 物块交换
                            && MyUtils.IsSameTile(i, j, PlatformItem.createTile, PlatformItem.placeStyle))
                        {
                            HitTile.HitTileObject hitTileObject = player.hitTile.data[player.hitTile.HitObject(i, j, 1)];
                            int damage = hitTileObject.damage;
                            int type = Main.tile[i, j].TileType;
                            player.PickTile(i, j, BestPickaxe != null ? BestPickaxe.pick : 1);
                            if (hitTileObject.damage > damage || type != Main.tile[i, j].TileType)
                            {
                                WorldGen.KillTile(i, j);
                            }
                        }
                        // 放置成功，是消耗品，可以被消耗 扣除一个
                        if (WorldGen.PlaceTile(i, j, PlatformItem.createTile, false, false, player.whoAmI, PlatformItem.placeStyle)
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
    }
}
