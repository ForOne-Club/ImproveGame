using ImproveGame.Modules.InfiniteBuff;
using System.Runtime.InteropServices;

namespace ImproveGame.Packets.Items;

public class InfiniteBuffItemPacket : NetModule
{
    [AutoSync]
    private byte _whoAmI;
    private readonly List<Item> _items = [];

    public static InfiniteBuffItemPacket GetInstance(int whoAmI, List<Item> availableItems)
    {
        var module = NetModuleLoader.Get<InfiniteBuffItemPacket>();
        module._whoAmI = (byte)whoAmI;
        module._items.Clear();
        module._items.AddRange(availableItems);
        return module;
    }

    public override void Send(ModPacket p)
    {
        p.Write((ushort)_items.Count);
        for (var i = 0; i < _items.Count; i++)
        {
            // 为了减少传输包大小，只传输这俩
            p.Write((ushort)_items[i].stack);
            p.Write((ushort)_items[i].type);
        }
    }

    public override void Read(BinaryReader r)
    {
        var listCount = r.ReadUInt16();
        _items.Clear();
        for (var i = 0; i < listCount; i++)
        {
            int stack = r.ReadUInt16();
            int type = r.ReadUInt16();
            _items.Add(new Item(type, stack));
        }
    }

    public override void Receive()
    {
        if (!Main.player[_whoAmI].TryGetModPlayer<InfiniteBuffPlayer>(out var mp)) return;

        mp.PlayerAvailableItems.Clear();
        mp.PlayerAvailableItems.AddRange(_items);

        // 服务器转发到其他客户端
        if (Main.netMode is NetmodeID.Server)
        {
            GetInstance(mp.Player.whoAmI, mp.PlayerAvailableItems).Send(-1, _whoAmI, false);
        }
    }
}