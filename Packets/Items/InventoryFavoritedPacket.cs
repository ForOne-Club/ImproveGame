using System.Collections;

namespace ImproveGame.Packets.Items;

/// <summary>
/// 直接传输玩家整个物品栏的favorited情况，只应从客户端发送给服务器
/// </summary>
public class InventoryFavoritedPacket : NetModule
{
    private BitArray _favoritedStates;
    
    public static void Send(Item[] inventory = null)
    {
        inventory ??= Main.LocalPlayer.inventory;

        bool[] flags = new bool[58];
        for (int i = 0; i < 58; i++)
        {
            flags[i] = inventory[i].favorited;
        }
        
        var packet = ModContent.GetInstance<InventoryFavoritedPacket>();
        packet._favoritedStates = new BitArray(flags);
        packet.Send(-1);
    }

    public override void Receive()
    {
        var player = Main.player[Sender];
        for (int i = 0; i < 58; i++)
        {
            player.inventory[i].favorited = _favoritedStates[i];
        }
    }

    public override void Send(ModPacket p)
    {
        byte[] bytes = new byte[(_favoritedStates.Length - 1) / 8 + 1]; // Calculation for correct length of the byte array
        _favoritedStates.CopyTo(bytes, 0);

        p.Write(bytes.Length);
        p.Write(bytes);
    }

    public override void Read(BinaryReader r)
    {
        int length = r.ReadInt32();
        byte[] bytes = r.ReadBytes(length);
        _favoritedStates = new BitArray(bytes);
    }
}