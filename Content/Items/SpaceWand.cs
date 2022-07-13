using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.GUI;
using Terraria.GameContent.Creative;
using static ImproveGame.MyUtils;

namespace ImproveGame.Content.Items
{
    public class SpaceWand : ModItem
    {
        public enum PlaceType
        {
            platform,
            soild,
            rope
        }

        public PlaceType placeType;

        // 准备加上一个纵向的
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems;
        public override bool AltFunctionUse(Player player) => true;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.channel = true;

            Item.mana = 20;
        }

        private Point start;
        private Point end;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (SpaceWandGUI.Visible)
                    UISystem.Instance.SpaceWandGUI.Close();
                else
                    UISystem.Instance.SpaceWandGUI.Open(this);
                return false;
            }
            GetPlatformCount(player.inventory, out int count);
            if (count < 1)
            {
                return false;
            }
            ItemRotation(player);
            start = Main.MouseWorld.ToTileCoordinates();
            CanPlace = true;
            return true;
        }

        /// <summary>
        /// 能否放置平台
        /// </summary>
        private bool CanPlace;
        public override bool? UseItem(Player player)
        {
            UseItem_PreUpdate(player);
            if (!Main.mouseLeft)
            {
                player.itemAnimation = 0;
                UseItem_LeftMouseUp(player);
                return true;
            }
            UseItem_PostUpdate(player);
            player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation - player.direction * MathHelper.ToRadians(90f));
            return false;
        }

        /// <summary>
        /// 前更新
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_PreUpdate(Player player)
        {
            player.itemAnimation = player.itemAnimationMax;
            ItemRotation(player);
            end = Main.MouseWorld.ToTileCoordinates();
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
                if (Main.mouseRight && CanPlace)
                {
                    CanPlace = false;
                    CombatText.NewText(player.getRect(), new Color(250, 40, 80), GetText("CombatText_Item.SpaceWand_Cancel"));
                }
                Color color;
                if (CanPlace)
                    color = new Color(135, 0, 180);
                else
                    color = new Color(250, 40, 80);
                int box = Box.NewBox(GetPlatfromRect(player), color * 0.35f, color);
                DrawSystem.boxs[box].ShowWidth = true;
            }
        }

        /// <summary>
        /// 左键释放
        /// </summary>
        /// <param name="player"></param>
        public void UseItem_LeftMouseUp(Player player)
        {
            // 放置平台
            if (CanPlace && player.whoAmI == Main.myPlayer && !Main.dedServ)
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
            if (!GetPlatformCount(player.inventory, out int platfromCount) || platfromCount > maxWidth)
            {
                platfromCount = maxWidth;
            }
            if (MathF.Abs(start.X - end.X) > MathF.Abs(start.Y - end.Y))
            {
                end = LimitRect(start, end, platfromCount, 1);
            }
            else
            {
                end = LimitRect(start, end, 1, platfromCount);
            }
            Rectangle rect = new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y),
                 (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1);
            return rect;
        }

        private static bool JudgePlatform(Item item) => item.createTile > -1 && TileID.Sets.Platforms[item.createTile];
    }
}
