using Terraria.DataStructures;

namespace ImproveGame.Common.Packets.NetChest;

[AutoSync]
public class ChestNamePacket : NetModule
{
    private ushort x;
    private ushort y;
    private string name;

    public static ChestNamePacket Get(ushort x, ushort y, string name)
    {
        var module = NetModuleLoader.Get<ChestNamePacket>();
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
    }
}