using ImproveGame.Content.Items;

namespace ImproveGame.Common.Packets.Chest;

/// <summary>
/// 客户端向服务器发送Item信息
/// </summary>
public class PlaceChestPacket : NetModule
{
    private Point chestCoord;
    private string chestName;
    private ushort chestType;
    private ItemPosition itemID;
    private Item[] items;
    private int style;
    public static PlaceChestPacket Get(Point coord, ushort chestType, int style, Item[] items, string chestName, ItemPosition itemID)
    {
        var packet = ModContent.GetInstance<PlaceChestPacket>();
        packet.chestCoord = coord;
        packet.chestType = chestType;
        packet.style = style;
        packet.items = items;
        packet.chestName = chestName;
        packet.itemID = itemID;
        return packet;
    }
    public override void Read(BinaryReader r)
    {
        chestCoord = r.ReadPoint();
        chestType = r.ReadUInt16();
        style = r.ReadInt32();
        items = r.ReadItemArray();
        chestName = r.ReadString();
        itemID = new ItemPosition(r.ReadByte(), r.ReadInt32());
    }

    public override void Receive()
    {
        // TODO 位置bug，箱子名称无法同步
        int index = WorldGen.PlaceChest(chestCoord.X, chestCoord.Y, chestType, false, style);
        if (index == -1)
        {
            Mod.Logger.Error("Unexpected Error: Unable to Place Chest - Server");
            return;
        }
        var chest = Main.chest[index];
        chest.item = items;
        chest.name = chestName;
        items = null;
        int x = chest.x, y = chest.y + 1;
        // 这原版怎么同步箱子都一堆魔法数
        switch (chestType)
        {
            case TileID.Containers:
                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 0, x, y, style, index);
                break;
            case TileID.Containers2:
                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 4, x, y, style, index);
                break;
            case TileID.Dressers:
                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 2, x, y, style, index);
                break;
            case ushort when TileID.Sets.BasicChest[chestType]:
                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 100, x, y, style, index, chestType, 0);
                break;
            case ushort when TileID.Sets.BasicDresser[chestType]:
                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 102, x, y, style, index, chestType, 0);
                break;
        }
        NetMessage.SendData(MessageID.ChestName, -1, -1, null, index, x, y);

        var item = itemID.slot >= 0 ? Main.player[itemID.player].inventory[itemID.slot] : null;
        if (item?.ModItem is MoveChest move)
        {
            move.hasChest = false;
            move.forceCoolDown = MoveChest.MaxForceCoolDown;
        }
        else
        {
            Mod.Logger.Error("Unexpected Item Not Found Error");
        }
    }

    public override void Send(ModPacket p)
    {
        p.Write(chestCoord);
        p.Write(chestType);
        p.Write(style);
        p.Write(items);
        p.Write(chestName);
        p.Write(itemID.player);
        p.Write(itemID.slot);
    }
}
