using ImproveGame.Content.Items;
using ImproveGame.Packets.Items;
using Terraria.ObjectData;

namespace ImproveGame.Packets.NetChest;

/// <summary>
/// 客户端向服务器询问拿取
/// </summary>
public class AskTakePacket : NetModule
{
    private ItemPosition itemID;
    [AutoSync] private Point coord;

    public static AskTakePacket Get(ItemPosition itemID, Point coord)
    {
        var packet = ModContent.GetInstance<AskTakePacket>();
        packet.itemID = itemID;
        packet.coord = coord;
        return packet;
    }

    // 仅在单人或者服务器端调用
    public override void Receive()
    {
        var tile = Main.tile[coord.X, coord.Y];

        // 拿走箱子
        if (!tile.HasTile || !TileID.Sets.BasicChest[tile.TileType])
        {
            return;
        }

        // 获取基本属性
        string modChestName = null;
        if (tile.TileType >= TileID.Count)
        {
            modChestName = ModContent.GetModTile(tile.TileType).FullName;
        }

        int style = TileObjectData.GetTileStyle(tile);

        // 坐标修正到箱子左上角
        coord -= new Point((tile.TileFrameX / 18) - (style * 2), tile.TileFrameY / 18);
        
        for (int i = 0; i < Main.chest.Length; i++)
        {
            var chest = Main.chest[i];
            if (chest == null)
            {
                continue;
            }

            if (chest.x != coord.X || chest.y != coord.Y)
            {
                continue;
            }

            // 复制到物品
            var player = Main.player[itemID.player];
            var item = itemID.slot >= 0 ? player.inventory[itemID.slot] : null;
            if (item?.ModItem is MoveChest move)
            {
                move.SetChest(chest.item, tile.TileType, chest.name, style, modChestName);
                InventoryItemDataPacket.Get(itemID, true).Send();
            }
            else
            {
                Mod.Logger.Error("Unexpected Item Not Found Error");
                return;
            }

            // 清除物块
            Main.tile[coord.X, coord.Y].ClearTile();
            Main.tile[coord.X + 1, coord.Y].ClearTile();
            Main.tile[coord.X, coord.Y + 1].ClearTile();
            Main.tile[coord.X + 1, coord.Y + 1].ClearTile();

            // 清空箱子
            Chest.DestroyChestDirect(coord.X, coord.Y, i);

            if (Main.netMode is NetmodeID.Server)
                NetMessage.SendTileSquare(-1, coord.X, coord.Y, 2);

            break;
        }
    }

    public override void Read(BinaryReader r)
    {
        itemID = new ItemPosition(r.ReadByte(), r.ReadInt32());
    }

    public override void Send(ModPacket p)
    {
        p.Write(itemID.player);
        p.Write(itemID.slot);
    }
}