using ImproveGame.Modules.InfiniteBuff;

namespace ImproveGame.Packets;

public class InfiniteBuffPacket : NetModule
{
    private int WhoAmI { get; set; }
    private readonly List<Item> Items = [];
    private readonly List<short> Flags = [];

    public static InfiniteBuffPacket GetInstance(int whoAmI, List<Item> buffItems, List<short> flags)
    {
        var module = ModContent.GetInstance<InfiniteBuffPacket>();
        module.WhoAmI = whoAmI;

        module.Items.Clear();
        module.Items.AddRange(buffItems);

        module.Flags.Clear();
        module.Flags.AddRange(flags);
        return module;
    }

    public override void Send(ModPacket p)
    {
        p.Write(WhoAmI);

        p.Write(Items.Count);
        for (var i = 0; i < Items.Count; i++)
        {
            p.Write((short)Items[i].type);
            p.Write((short)Items[i].stack);
        }

        p.Write(Flags.Count);
        for (var i = 0; i < Flags.Count; i++)
        {
            p.Write(Flags[i]);
        }
    }

    public override void Read(BinaryReader r)
    {
        WhoAmI = r.ReadInt32();

        Items.Clear();
        var itemCount = r.ReadInt32();
        for (var i = 0; i < itemCount; i++)
        {
            int type = r.ReadInt16();
            int stack = r.ReadInt16();
            Items.Add(new Item(type, stack));
        }

        Flags.Clear();
        var flagCount = r.ReadInt32();
        for (var i = 0; i < flagCount; i++)
        {
            Flags.Add(r.ReadInt16());
        }
    }

    public override void Receive()
    {
        if (!Main.player[WhoAmI].TryGetModPlayer<InfiniteBuffModPlayer>(out var infinite)) return;

        infinite.PlayerBuffItems.Clear();
        infinite.PlayerBuffItems.AddRange(Items);

        foreach (var i in Flags)
        {
            infinite.ActivationFlags[i] = true;
        }

        // 服务器转发到其他客户端
        if (Main.netMode is NetmodeID.Server)
        {
            GetInstance(WhoAmI, Items, Flags).Send(-1, WhoAmI, false);
        }
    }
}