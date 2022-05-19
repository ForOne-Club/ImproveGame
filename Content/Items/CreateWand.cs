using ImproveGame.Common.ModPlayers;
using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Content.Items
{
    public class CreateWand : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.width = 24;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override void HoldItem(Player player)
        {
            Point point = Main.MouseWorld.ToTileCoordinates(); // 鼠标位置
            point.X -= 5;
            point.Y -= 3;
            player.GetModPlayer<UpdatePlayer>().MagiskKillTiles = true;
            TileDraw.MagiskTilesRec = new Rectangle(point.X, point.Y, 11, 6);
            TileDraw.MagiskTileColor = new Color(0, 165, 255, 255);
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation == player.itemAnimationMax)
            {
                List<TileInfo> tileInfos = Utils.PrisonTiles(19, 0);
                List<WallInfo> wallInfos = Utils.PrisonWalls(4);
                Point point = Main.MouseWorld.ToTileCoordinates(); // 鼠标位置
                point.X -= 5;
                point.Y -= 3;
                for (int i = 0; i < wallInfos.Count; i++)
                {
                    WallInfo wallInfo = wallInfos[i];
                    WorldGen.PlaceWall(point.X + wallInfo.x + 1, point.Y + wallInfo.y + 1, wallInfo.type);
                }
                for (int i = 0; i < tileInfos.Count; i++)
                {
                    TileInfo tileInfo = tileInfos[i];
                    WorldGen.PlaceTile(point.X + tileInfo.x, point.Y + tileInfo.y, tileInfo.type, false, false, player.whoAmI);
                    if (tileInfo.x == 3 && tileInfo.y == 5)
                    {
                        Main.tile[point.X + tileInfo.x, point.Y + tileInfo.y].TileFrameX = 18;
                        Main.tile[point.X + tileInfo.x, point.Y + tileInfo.y - 1].TileFrameX = 18;
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient) // 如果是处在服务器的人物
                {
                    NetMessage.SendTileSquare(player.whoAmI, point.X, point.Y, 11, 6); // 发送数据到其他端
                }
            }
            return base.UseItem(player);
        }
    }
}
