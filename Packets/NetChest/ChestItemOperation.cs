namespace ImproveGame.Packets.NetChest;

public static class ChestItemOperation
{
    /// <summary>
    /// 由客户端向服务器发送同步箱子物品的请求
    /// </summary>
    /// <param name="chestIndex">箱子索引</param>
    /// <param name="slot">要同步的槽位，为 -1 则同步所有槽位</param>
    /// <exception cref="UsageException">不能在服务器端调用</exception>
    public static void RequestChestItem(int chestIndex, int slot = -1)
    {
        if (Main.netMode is NetmodeID.Server) throw new UsageException("This method can only be called on the client.");
        RequestForChestItem.Get(chestIndex, slot).Send();
    }
    
    /// <inheritdoc cref="SendItemWithSync"/>
    public static void SendAllItemsWithSync(int chestIndex)
    {
        if (chestIndex is -1 || Main.chest[chestIndex] is null) return;
        for (var k = 0; k < Chest.maxItems; k++)
        {
            SendItemWithSync(chestIndex, k);
        }
    }
    
    /// <summary>
    /// 发送带转发到全客户端的包，只应在客户端使用
    /// </summary>
    /// <exception cref="UsageException">不能在服务器端调用</exception>
    public static void SendItemWithSync(int chestIndex, int slot)
    {
        if (Main.netMode is NetmodeID.Server) throw new UsageException("This method can only be called on the client.");
        SendItem(chestIndex, slot);
        ChestOperationWithResend.Get(chestIndex, slot).Send();
    }
    
    public static void SendItem(int chestIndex, int slot, int toClient = -1, int ignoreClient = -1)
    {
        NetMessage.TrySendData(MessageID.SyncChestItem, toClient, ignoreClient, null, chestIndex, slot);
    }

    public static void SendAllItems(int chestIndex, int toClient = -1, int ignoreClient = -1)
    {
        if (chestIndex is -1 || Main.chest[chestIndex] is null) return;
        for (var k = 0; k < Chest.maxItems; k++)
        {
            SendItem(chestIndex, k, toClient, ignoreClient);
        }
    }
    
    /// <inheritdoc cref="TellServerToForwardItem"/>
    public static void TellServerToForwardAllItems(int chestIndex)
    {
        if (chestIndex is -1 || Main.chest[chestIndex] is null) return;
        for (var k = 0; k < Chest.maxItems; k++)
        {
            TellServerToForwardItem(chestIndex, k);
        }
    }
    
    /// <summary>
    /// 发包告诉服务器某个箱子的物品要同步，只应在客户端使用
    /// </summary>
    /// <exception cref="UsageException">不能在服务器端调用</exception>
    public static void TellServerToForwardItem(int chestIndex, int slot)
    {
        if (Main.netMode is NetmodeID.Server) throw new UsageException("This method can only be called on the client.");
        ChestOperationWithResend.Get(chestIndex, slot).Send();
    }
    
    [AutoSync]
    private class ChestOperationWithResend : NetModule
    {
        private short _chestIndex;
        private byte _slotIndex;
        
        public static ChestOperationWithResend Get(int chestIndex, int slotIndex)
        {
            var packet = ModContent.GetInstance<ChestOperationWithResend>();
            packet._chestIndex = (short)chestIndex;
            packet._slotIndex = (byte)slotIndex;
            return packet;
        }

        public override void Receive()
        {
            SendItem(_chestIndex, _slotIndex, -1, Sender);
        }
    }
    
    [AutoSync]
    private class RequestForChestItem : NetModule
    {
        private short _chestIndex;
        private sbyte _slotIndex;
        
        public static RequestForChestItem Get(int chestIndex, int slotIndex)
        {
            var packet = ModContent.GetInstance<RequestForChestItem>();
            packet._chestIndex = (short)chestIndex;
            packet._slotIndex = (sbyte)slotIndex;
            return packet;
        }

        public override void Receive()
        {
            if (_slotIndex is -1)
                SendAllItems(_chestIndex, Sender);
            else
                SendItem(_chestIndex, _slotIndex, Sender);
        }
    }
}