using ImproveGame.Modules.InfiniteBuff;

namespace ImproveGame.Packets;

public class InfiniteBuffPacket : NetModule
{
    private int _whoAmI;
    private readonly List<Item> _items = [];
    private readonly List<short> _flags = [];

    public static InfiniteBuffPacket GetInstance(int whoAmI, List<Item> buffItems, ReadOnlySpan<bool> flags)
    {
        var list = new List<short>(flags.Length);
        for (int i = 0; i < flags.Length; i++)
            if (flags[i]) list.Add((short)i);

        return GetInstance(whoAmI, buffItems, list);
    }

    private static InfiniteBuffPacket GetInstance(int whoAmI, List<Item> buffItems, List<short> flags)
    {
        var module = ModContent.GetInstance<InfiniteBuffPacket>();
        module._whoAmI = whoAmI;

        module._items.Clear();
        module._items.AddRange(buffItems);

        module._flags.Clear();
        module._flags.AddRange(flags);
        return module;
    }

    public override void Send(ModPacket p)
    {
        p.Write(_whoAmI);

        p.Write(_items.Count);
        for (var i = 0; i < _items.Count; i++)
        {
            p.Write((short)_items[i].type);
            p.Write((short)_items[i].stack);
        }

        p.Write(_flags.Count);
        for (var i = 0; i < _flags.Count; i++)
        {
            p.Write(_flags[i]);
        }
    }

    public override void Read(BinaryReader r)
    {
        _whoAmI = r.ReadInt32();

        _items.Clear();
        var itemCount = r.ReadInt32();
        for (var i = 0; i < itemCount; i++)
        {
            int type = r.ReadInt16();
            int stack = r.ReadInt16();
            _items.Add(new Item(type, stack));
        }

        _flags.Clear();
        var flagCount = r.ReadInt32();
        for (var i = 0; i < flagCount; i++)
        {
            _flags.Add(r.ReadInt16());
        }
    }

    public override void Receive()
    {
        if (!Main.player[_whoAmI].TryGetModPlayer<InfiniteBuffModPlayer>(out var infinite)) return;

        infinite.PlayerBuffItems.Clear();
        infinite.PlayerBuffItems.AddRange(_items);

        foreach (var i in _flags)
        {
            infinite.ActivationFlags[i] = true;
        }

        // 调试代码
        //if (Main.netMode == NetmodeID.MultiplayerClient)
        //{
        //    Main.NewText(string.Join(", ", _items.Select(i => i.Name)));
        //}

        // 服务器转发到其他客户端
        if (Main.netMode != NetmodeID.Server) return;

        // NetModule 是单例的
        // 这次错误是因为 List.Clear => List.AddRange(self) 实际是清空了
        //GetInstance(_whoAmI, _items, _flags).Send(-1, _whoAmI);
        Send(-1, _whoAmI);
    }
}