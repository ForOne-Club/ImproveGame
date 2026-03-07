using ImproveGame.Modules.InfiniteBuff;

namespace ImproveGame.Packets.Items;

public class BuffDataPacket : NetModule
{
    [AutoSync]
    private byte _whoAmI;
    private readonly List<Item> _buffItems = [];
    private readonly List<ushort> _flags = [];

    public static BuffDataPacket GetInstance(int whoAmI, List<Item> buffItems, List<ushort> flags)
    {
        var module = NetModuleLoader.Get<BuffDataPacket>();
        module._whoAmI = (byte)whoAmI;
        module._buffItems.Clear();
        module._buffItems.AddRange(buffItems);
        module._flags.Clear();
        module._flags.AddRange(flags);
        return module;
    }

    public override void Send(ModPacket p)
    {
        p.Write((ushort)_buffItems.Count);
        for (var i = 0; i < _buffItems.Count; i++)
        {
            // 为了减少传输包大小，只传输这俩
            p.Write((ushort)_buffItems[i].stack);
            p.Write((ushort)_buffItems[i].type);
        }

        p.Write((ushort)_flags.Count);
        for (var i = 0; i < _flags.Count; i++)
        {
            p.Write(_flags[i]);
        }
    }

    public override void Read(BinaryReader r)
    {
        var listCount = r.ReadUInt16();
        _buffItems.Clear();
        for (var i = 0; i < listCount; i++)
        {
            int stack = r.ReadUInt16();
            int type = r.ReadUInt16();
            _buffItems.Add(new Item(type, stack));
        }

        var flagsCount = r.ReadUInt16();
        _flags.Clear();
        for (var i = 0; i < flagsCount; i++)
        {
            _flags.Add(r.ReadUInt16());
        }
    }

    public override void Receive()
    {
        if (!Main.player[_whoAmI].TryGetModPlayer<InfiniteBuffModPlayer>(out var infinite)) return;

        infinite.PlayerBuffItems.Clear();
        infinite.PlayerBuffItems.AddRange(_buffItems);

        // 服务器转发到其他客户端
        if (Main.netMode is NetmodeID.Server)
        {
            GetInstance(_whoAmI, _buffItems, _flags).Send(-1, _whoAmI, false);
        }
    }
}