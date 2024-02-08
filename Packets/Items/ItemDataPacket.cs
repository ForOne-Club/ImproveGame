using Terraria.DataStructures;

namespace ImproveGame.Packets.Items;

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
    // 0: 检验存在，若true，则只有物品存在时才设置物品，否则不进行操作（可以避免复制物品）
    // 1: 是否设置item.favorited
    // 2: 是否在物品槽已存在物品时生成物品
    private byte _options;
    private byte _playerIndex;
    private sbyte _slot;

    public static InventoryItemDataPacket Get(byte playerIndex, int slot, BitsByte options)
    {
        var packet = ModContent.GetInstance<InventoryItemDataPacket>();
        packet._item = Main.player[playerIndex].inventory[slot];
        packet._options = options;
        packet._playerIndex = playerIndex;
        packet._slot = (sbyte)slot;
        return packet;
    }

    public static InventoryItemDataPacket Get(byte playerIndex, int slot, bool ensureExistence,
        bool spawnItemIfWasFilled)
    {
        var packet = Get(playerIndex, slot, ensureExistence);
        packet._options = new BitsByte(ensureExistence, false, spawnItemIfWasFilled);
        return packet;
    }
    
    public static InventoryItemDataPacket Get(byte playerIndex, int slot, bool ensureExistence) =>
        Get(new ItemPosition(playerIndex, slot), ensureExistence);
    
    public static InventoryItemDataPacket Get(ItemPosition itemID, bool ensureExistence)
    {
        var packet = ModContent.GetInstance<InventoryItemDataPacket>();
        packet._item = Main.player[itemID.player].inventory[itemID.slot];
        packet._options = new BitsByte(ensureExistence, false);
        packet._playerIndex = itemID.player;
        packet._slot = (sbyte)itemID.slot;
        return packet;
    }

    public override void Receive()
    {
        if (!Main.player.IndexInRange(_playerIndex) || !Main.player[_playerIndex].inventory.IndexInRange(_slot))
            return;

        var player = Main.player[_playerIndex];
        var options = (BitsByte)_options;
        bool ensureExistence = options[0];
        bool setFavorite = options[1];
        bool spawnItemIfWasFilled = options[2];
        if (ensureExistence && player.inventory[_slot].IsAir)
            return;

        if (spawnItemIfWasFilled && !player.inventory[_slot].IsAir && !_item.IsAir)
        {
            player.QuickSpawnItem(new EntitySource_Sync("SyncFromServer"), _item, _item.stack);
        }
        
        if (setFavorite)
        {
            player.inventory[_slot] = _item;
        }
        else
        {
            bool oldFavorited = player.inventory[_slot].favorited;
            player.inventory[_slot] = _item;
            player.inventory[_slot].favorited = oldFavorited;
        }

        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            Recipe.FindRecipes();
        }
    }
}