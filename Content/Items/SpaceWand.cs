using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static ImproveGame.MyUtils;

namespace ImproveGame.Content.Items
{
    public class SpaceWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems;
        public override void SetStaticDefaults() => Item.staff[Type] = true;

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

        private Point start = Point.Zero;
        private Point end = Point.Zero;

        public override bool CanUseItem(Player player)
        {
            // 有平台的时候才允许使用
            int count = 0;
            GetPlatformCount(player.inventory, ref count);
            if (count < 1)
            {
                return false;
            }
            ItemRotation(player);
            return base.CanUseItem(player);
        }

        /// <summary>
        /// 待起名
        /// </summary>
        private bool MouseLeftPress;
        /// <summary>
        /// 能否放置平台
        /// </summary>
        private bool CanPlacePlatform;

        public override bool? UseItem(Player player)
        {
            UseItem_PreUpdate(player);
            if (!MouseLeftPress && Main.mouseLeft)
            {
                MouseLeftPress = true;
                UseItem_LeftMouseDown(player);
            }
            else if (MouseLeftPress && !Main.mouseLeft)
            {
                MouseLeftPress = false;
                player.itemAnimation = 0;
                UseItem_LeftMouseUp(player);
                return true;
            }
            UseItem_PostUpdate(player);
            return false;
        }

        /// <summary>
        /// 前更新
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_PreUpdate(Player player)
        {
            player.itemAnimation = player.itemAnimationMax / 2;

            ItemRotation(player);

            // 开启绘制
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                end = Main.MouseWorld.ToTileCoordinates();
            }
        }

        /// <summary>
        /// 后更新
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_PostUpdate(Player player)
        {
            // 开启绘制
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                if (Main.mouseRight && CanPlacePlatform)
                {
                    CanPlacePlatform = false;
                    CombatText.NewText(player.getRect(), new Color(250, 40, 80), GetText("CombatText_Item.SpaceWand_Cancel"));
                }
                Color color;
                if (CanPlacePlatform)
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
            CanPlacePlatform = true;
        }

        /// <summary>
        /// 左键释放
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_LeftMouseUp(Player player)
        {
            // 放置平台
            if (CanPlacePlatform && player.whoAmI == Main.myPlayer && !Main.dedServ)
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
                        int oneIndex = EnoughItem(player, JudgePlatform);
                        if (oneIndex > -1)
                        {
                            Item item = player.inventory[oneIndex];
                            if (player.TileReplacementEnabled && NotSameTile(i, j, item.createTile, item.placeStyle))
                            {
                                TryKillTile(i, j, player);
                            }
                            if (!Main.tile[i, j].HasTile)
                            {
                                SoundEngine.PlaySound(SoundID.Dig);
                                WorldGen.PlaceTile(i, j, item.createTile, true, true, player.whoAmI, item.placeStyle);
                                PickItemInInventory(player, JudgePlatform, true);
                            }
                        }
                    }
                }
                // 发送数据到服务器
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    int x = minI;
                    int y = minJ;
                    int width = maxI - minI + 1;
                    int height = maxJ - minJ + 1;
                    NetMessage.SendData(MessageID.TileSquare, player.whoAmI, -1, null, x, y, width, height);
                }
            }
        }

        public Rectangle GetPlatfromRect(Player player)
        {
            int maxWidth = Main.netMode == NetmodeID.MultiplayerClient ? 244 : 500;
            // 平台数量
            int platfromCount = 0;
            if (!GetPlatformCount(player.inventory, ref platfromCount) || platfromCount > maxWidth)
            {
                platfromCount = maxWidth;
            }
            end = LimitRect(start, end, platfromCount, 1);
            Rectangle rect = new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y),
                 (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1);
            return rect;
        }

        private static bool JudgePlatform(Item item) => item.createTile > -1 && TileID.Sets.Platforms[item.createTile];
    }
}
