namespace ImproveGame.Common.Packets.NetChest;

/// <summary>
/// 用于寻找其他服务器中对应的物品
/// </summary>
public record ItemPosition(byte player, int slot)
{
    /// <summary>
    /// 该物品所属的玩家的 <see cref="Player.whoAmI"/>
    /// </summary>
    public byte player = player;

    /// <summary>
    /// 该物品所在的栏位
    /// </summary>
    public int slot = slot;
}

/// <summary>
/// 原版的 SyncEquipment 不支持服务器向玩家传输，因此自行写一个
/// </summary>
[AutoSync]
public class InventoryItemDataPacket : NetModule
{
    [ItemSync(syncFavorite: true)] private Item _item;
    private bool _ensureExistence;
    private byte _playerIndex;
    private int _slot;
        
    public static InventoryItemDataPacket Get(byte playerIndex, int slot, bool ensureExistence)
    {
        var packet = ModContent.GetInstance<InventoryItemDataPacket>();
        packet._item = Main.player[playerIndex].inventory[slot];
        packet._ensureExistence = ensureExistence;
        packet._playerIndex = playerIndex;
        packet._slot = slot;
        return packet;
    }
    
    public static InventoryItemDataPacket Get(ItemPosition itemID, bool ensureExistence)
    {
        var packet = ModContent.GetInstance<InventoryItemDataPacket>();
        packet._item = Main.player[itemID.player].inventory[itemID.slot];
        packet._ensureExistence = ensureExistence;
        packet._playerIndex = itemID.player;
        packet._slot = itemID.slot;
        return packet;
    }

    public override void Receive()
    {
        if (!Main.player.IndexInRange(_playerIndex) || !Main.player[_playerIndex].inventory.IndexInRange(_slot))
            return;
        
        if (_ensureExistence && Main.player[_playerIndex].inventory[_slot].IsAir)
            return;
        
        Main.player[_playerIndex].inventory[_slot] = _item;
    }
}