using ImproveGame.Packets.NetChest;
using System.Diagnostics;

namespace ImproveGame.Packets.NetStorager;

public class SwapItemOperationPacket : NetModule
{
    [AutoSync] private Item _item;
    [AutoSync] private short _chestIndex;
    [AutoSync] private byte _slotIndex;

    /// <summary>
    /// 交换手上物品和箱子中的物品
    /// </summary>
    /// <param name="chestIndex"></param>
    /// <param name="slotIndex"></param>
    public static void Send(int chestIndex, int slotIndex)
    {
        Debug.Assert(!Main.mouseItem.IsAir && chestIndex != -1 && slotIndex is >= 0 and < Chest.maxItems,
            "Invalid data, Check your code");
        var packet = ModContent.GetInstance<SwapItemOperationPacket>();
        packet._item = Main.mouseItem;
        packet._chestIndex = (short)chestIndex;
        packet._slotIndex = (byte)slotIndex;
        packet.Send();
    }


    public override void Receive()
    {
        Debug.Assert(Main.chest.IndexInRange(_chestIndex) && _slotIndex is >= 0 and < Chest.maxItems,
            "Invalid data, Check your code");
        ref var slotItem = ref Main.chest[_chestIndex].item[_slotIndex];
        switch (Main.netMode)
        {
            case NetmodeID.Server:
                if (Chest.UsingChest(_chestIndex) is -1)
                {
                    var originalItem = slotItem.Clone();
                    slotItem = _item.Clone();
                    _item = originalItem;
                    // 回复交换成功
                    Send(Sender);
                    // 同步物品
                    ChestItemOperation.SendItem(_chestIndex, _slotIndex, -1, Sender);
                }

                break;

            case NetmodeID.MultiplayerClient:
                // 交换成功
                slotItem = _item;
                (Main.mouseItem, slotItem) = (slotItem, Main.mouseItem);
                break;
        }
    }
}