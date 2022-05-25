using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Player;

namespace ImproveGame.Content.Items
{
    public class SpaceWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return Utils.GetConfig().LoadModItems;
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

        private bool BeginDown = true;
        private bool RightDown = false;
        Point StartPosition = Point.Zero;
        Point EndPosition = Point.Zero;

        public override void UseItemFrame(Player player)
        {
            CompositeArmStretchAmount stretch = CompositeArmStretchAmount.Full;
            float rotation = player.itemRotation - (float)Math.PI / 2f * (float)player.direction;
            player.SetCompositeArmFront(enabled: true, stretch, rotation);
        }

        public override void HoldItem(Player player)
        {
            if (!Main.mouseLeft && !BeginDown)
            {
                BeginDown = true;
            }
            if (Main.mouseRight && !RightDown && !BeginDown)
            {
                CombatText.NewText(player.getRect(), new Color(255, 165, 255, 255), Language.GetTextValue($"Mods.ImproveGame.CombatText_Item.SpaceWand_Cancel"));
                RightDown = true;
            }
            if (BeginDown && !Main.mouseRight)
            {
                RightDown = false;
            }
        }

        public override bool CanUseItem(Player player)
        {
            int count = 0;
            Utils.GetPlatformCount(player.inventory, ref count);
            if (count < 1)
            {
                return false;
            }
            return base.CanUseItem(player);
        }

        public override bool? UseItem(Player player)
        {
            // 旋转物品
            Vector2 rotaion = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
            player.direction = Main.MouseWorld.X < player.Center.X ? -1 : 1;
            player.itemRotation = MathF.Atan2(rotaion.Y * player.direction, rotaion.X * player.direction);
            // 首次按下
            if (BeginDown && Main.mouseLeft)
            {
                BeginDown = false;
                StartPosition = Main.MouseWorld.ToTileCoordinates();
            }
            // 首次之后按下
            if (!BeginDown && Main.mouseLeft)
            {
                player.itemAnimation = player.itemAnimationMax / 2;
                EndPosition = Main.MouseWorld.ToTileCoordinates();
            }
            // 绘制层参数，X、Y 
            int platfromCount = 0;
            bool consumable = Utils.GetPlatformCount(player.inventory, ref platfromCount);
            if (!consumable)
            {
                platfromCount = 500;
            }
            TileDraw.MagiskTilesRec.X = (int)MathF.Min(StartPosition.X, EndPosition.X);
            if (TileDraw.MagiskTilesRec.X < StartPosition.X - MathF.Min(499, platfromCount - 1))
            {
                TileDraw.MagiskTilesRec.X = StartPosition.X - (int)MathF.Min(499, platfromCount - 1);
            }
            TileDraw.MagiskTilesRec.Y = StartPosition.Y;
            TileDraw.MagiskTilesRec.Width = (int)MathF.Min(MathF.Abs(StartPosition.X - EndPosition.X) + 1, MathF.Min(500, platfromCount));
            TileDraw.MagiskTilesRec.Height = 1;
            if (Main.mouseLeft)
            {
                TileDraw.MagiskTileColor = new Color(0, 165, 255, 255);
            }
            if (RightDown)
            {
                TileDraw.MagiskTileColor = new Color(255, 0, 0, 255);
            }
            player.GetModPlayer<Common.ModPlayers.UpdatePlayer>().MagiskKillTiles = true;
            int minI = TileDraw.MagiskTilesRec.X;
            int maxI = TileDraw.MagiskTilesRec.X + TileDraw.MagiskTilesRec.Width - 1;
            int minJ = TileDraw.MagiskTilesRec.Y;
            int maxJ = TileDraw.MagiskTilesRec.Y + TileDraw.MagiskTilesRec.Height - 1;
            if (player.whoAmI == Main.myPlayer)
            {
                // 没有按下
                if (!BeginDown && !Main.mouseLeft && !RightDown)
                {
                    // 处理图块
                    for (int i = minI; i <= maxI; i++)
                    {
                        for (int j = minJ; j <= maxJ; j++)
                        {
                            // 找到背包第一个平台
                            Item PlatformItem = Utils.GetFirstPlatform(player);
                            Item BestPickaxe = player.GetBestPickaxe();
                            // 如果有找到平台结束执行
                            if (PlatformItem.type == ItemID.None)
                            {
                                CombatText.NewText(player.getRect(), new Color(255, 0, 0, 255), Language.GetTextValue($"Mods.ImproveGame.CombatText_Item.SpaceWand_Lack"));
                                return false;
                            }
                            // 破坏物块
                            if (!Main.tileHammer[Main.tile[i, j].TileType] && BestPickaxe.type != ItemID.None
                                && player.TileReplacementEnabled // 物块交换
                                && Utils.IsSameTile(i, j, PlatformItem.createTile, PlatformItem.placeStyle))
                            {
                                HitTile.HitTileObject hitTileObject = player.hitTile.data[player.hitTile.HitObject(i, j, 1)];
                                int damage = hitTileObject.damage;
                                int type = Main.tile[i, j].TileType;
                                player.PickTile(i, j, BestPickaxe.pick);
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
            return false;
        }

        public override void AddRecipes()
        {

        }
    }
}
