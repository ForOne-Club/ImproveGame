namespace ImproveGame.Packets.NetChest;

[AutoSync]
public class ChestNamePacketByPosition : NetModule
{
    private ushort x;
    private ushort y;
    private string name;

    public static ChestNamePacketByPosition Get(ushort x, ushort y, string name)
    {
        var module = NetModuleLoader.Get<ChestNamePacketByPosition>();
        module.x = x;
        module.y = y;
        module.name = name;
        return module;
    }

    public override void Receive()
    {
        var index = Chest.FindChest(x, y);
        if (index is -1) return;

        Main.chest[index].name = name;
        // 服务器转发
        if (Main.netMode is NetmodeID.Server)
        {
            Send();
        }
    }
}

[AutoSync]
public class ChestNamePacketByID : NetModule
{
    private ushort id;
    private string name;

    public static void Send(ushort id, string name)
    {
        var module = NetModuleLoader.Get<ChestNamePacketByID>();
        module.id = id;
        module.name = name;
        module.Send();
    }

    public override void Receive()
    {
        if (Main.chest[id] is null) return;

        Main.chest[id].name = name;
        // 服务器转发
        if (Main.netMode is NetmodeID.Server)
        {
            Send();
        }
    }
}