using ImproveGame.Content.Items.Placeable;
using ImproveGame.Packets.NetChest;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ImproveGame.Packets.NetStorager;

/// <summary>
/// 向 <see cref="ExtremeStorage"/> 增加物品的操作
/// </summary>
public class AddItemOperationPacket : NetModule
{
    private Item _item;
    private short _chestIndex;
    private byte _slotIndex;
    [AutoSync] private int _count;

    /// <summary>
    /// 把 <see cref="Main.mouseItem"/> 添加到 <paramref name="chestIndex"/> 箱子的 <paramref
    /// name="slotIndex"/><br/> 注：若count为default，则全部放入
    /// </summary>
    /// <param name="chestIndex">箱子下标</param>
    /// <param name="slotIndex">物品所占格子</param>
    /// <param name="count">存放的数量</param>
    public static void Send(int chestIndex, int slotIndex, int count = default)
    {
        Debug.Assert(!Main.mouseItem.IsAir && chestIndex != -1 && slotIndex is >= 0 and < Chest.maxItems,
            "Invalid data, Check your code");
        Debug.Assert(count < Main.mouseItem.stack, "Count must be less than mouseItem.stack!");
        var packet = ModContent.GetInstance<AddItemOperationPacket>();
        packet._item = Main.mouseItem;
        packet._chestIndex = (short)chestIndex;
        packet._slotIndex = (byte)slotIndex;
        packet._count = count;
        packet.Send();
    }

    /// <summary>
    /// 服务器收到客户端发送的信息
    /// </summary>
    /// <param name="r"></param>
    public override void Read(BinaryReader r)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            _item = r.ReadItem();
            //记录应该存储的数量
            if (_count != default)
            {
                // 直接将读取的_item的stack设置为_count
                _item.stack = _count;
            }
            else
            {
                _count = _item.stack;
            }

            _chestIndex = r.ReadInt16();
            _slotIndex = r.ReadByte();
        }
    }

    /// <summary>
    /// 客户端向服务器发送信息
    /// </summary>
    /// <param name="p"></param>
    public override void Send(ModPacket p)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            p.Write(_item);
            p.Write(_chestIndex);
            p.Write(_slotIndex);
        }
    }

    public override void Receive()
    {
        switch (Main.netMode)
        {
            case NetmodeID.Server:
                if (Chest.UsingChest(_chestIndex) is -1 && TryAddItem(out var realSlotIndex))
                {
                    // 回复存储成功
                    Send(Sender);
                    // 同步物品
                    ChestItemOperation.SendItem(_chestIndex, realSlotIndex);
                }

                break;

            case NetmodeID.MultiplayerClient:
                // 存储成功，得到剩余物品数量
                // 此时_count为存入的数量，给mouseItem减去存入的数量
                if (Main.mouseItem.stack == _count)
                {
                    Main.mouseItem.TurnToAir();
                }
                else
                {
                    Main.mouseItem.stack -= _count;
                }

                break;
        }
    }

    private bool TryAddItem(out int realSlotIndex)
    {
        // 初始化 & 检查
        ref var slotItem = ref Main.chest[_chestIndex].item[realSlotIndex = _slotIndex];
        Debug.Assert(slotItem != null, "Chest not sync!"); // 理论上已经同步的Chest中的Item不会为null

        BeginAdd:
        if (slotItem.IsAir)
        {
            slotItem = _item;
            return true;
        }

        if (slotItem.type == _item.type)
        {
            // 不成功就不能return
            if (slotItem.stack + _item.stack <= slotItem.maxStack)
            {
                slotItem.stack += _item.stack;
                return true;
            }

            _item.stack -= slotItem.maxStack - slotItem.stack;
            slotItem.stack = slotItem.maxStack;
            // 同步物品
            ChestItemOperation.SendItem(_chestIndex, realSlotIndex, -1, Sender);
        }

        // 存储失败，尝试存到其他地方（必要性存疑
        slotItem = ref Unsafe.Add(ref slotItem, 1);
        if (++realSlotIndex >= Chest.maxItems) // 用maxItems而不是Item[]::Length，应该没问题（？
        {
            // 得到存进去的的数量
            if (_count != _item.stack)
            {
                // 此后_count表示存进去的数量
                _count -= _item.stack;
                // 返回消息
                Send(Sender);
            }

            return false;
        }

        // 重新尝试Add，不想用递归和while
        goto BeginAdd;
    }
}