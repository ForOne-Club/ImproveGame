using ImproveGame.Packets.NetChest;

namespace ImproveGame.Packets.NetStorager;

/// <summary>
/// 移出物品的操作
/// </summary>
public class RemoveItemOperationPacket : NetModule
{
    public enum RemovedItemDestination : byte
    {
        Mouse,
        Inventory,
        Trash
    }

    [AutoSync] private short _chestIndex;
    [AutoSync] private byte _slotIndex;
    [AutoSync] private int _count;
    [AutoSync] private byte _target;

    private RemovedItemDestination Target
    {
        get => (RemovedItemDestination)_target;
        set => _target = (byte)value;
    }

    /// <summary>
    /// 把 <paramref name="chestIndex"/> 箱子中的 <paramref name="slotIndex"/> 物品，移动 <paramref
    /// name="count"/> 个到 <paramref name="destination"/>
    /// </summary>
    /// <param name="chestIndex"></param>
    /// <param name="slotIndex"></param>
    /// <param name="destination"></param>
    /// <param name="count">若stack为null，则全部移出</param>
    public static void Send(int chestIndex, int slotIndex, RemovedItemDestination destination, int? count = null)
    {
        var packet = ModContent.GetInstance<RemoveItemOperationPacket>();
        packet._chestIndex = (short)chestIndex;
        packet._slotIndex = (byte)slotIndex;
        packet.Target = destination;
        packet._count = count ?? Main.chest[chestIndex].item[slotIndex].stack;
        packet.Send();
    }

    public override void Receive()
    {
        switch (Main.netMode)
        {
            case NetmodeID.Server:
                // 将realCount赋值给_count，然后发回客户端（所以我为什么不直接在函数里面修改_count）
                if (Chest.UsingChest(_chestIndex) is -1 && TryRemoveItem(out _count))
                {
                    // 回复存储成功
                    Send(Sender);
                    // 同步物品
                    ChestItemOperation.SendItem(_chestIndex, _slotIndex, -1, Sender);
                }

                break;
            case NetmodeID.MultiplayerClient:
                Item item;
                ref Item slotItem = ref Main.chest[_chestIndex].item[_slotIndex];
                if (slotItem.stack == _count)
                {
                    item = slotItem;
                    slotItem = new Item();
                }
                else
                {
                    item = slotItem.Clone();
                    item.stack = _count;
                    slotItem.stack -= _count;
                }

                // 应该会在物品消失前收到这个消息？
                switch (Target)
                {
                    case RemovedItemDestination.Mouse:
                        if (Main.mouseItem.type == item.type)
                        {
                            Main.mouseItem.stack += _count;
                        }
                        else
                        {
                            Main.mouseItem = item;
                        }

                        break;
                    case RemovedItemDestination.Inventory:
                        Main.LocalPlayer.GetItem(Sender, item,
                            GetItemSettings.InventoryEntityToPlayerInventorySettings);
                        break;
                    case RemovedItemDestination.Trash:
                        Main.LocalPlayer.trashItem = item;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
        }
    }

    private bool TryRemoveItem(out int realCount)
    {
        var item = Main.chest[_chestIndex].item[_slotIndex];
        if (item.IsAir)
        {
            realCount = 0;
            return false;
        }

        if (item.stack > _count)
        {
            item.stack -= realCount = _count;
        }
        else
        {
            realCount = item.stack;
            item.TurnToAir();
        }

        return true;
    }
}