using ImproveGame.Content.Functions.VeinMiner;

namespace ImproveGame.Packets.Tiles;

[AutoSync]
public class VeinMiningPacket : NetModule
{
    private Point _center;
    private ushort _tileType;

    public static void Run(Point center, int tileType)
    {
        Get(center, tileType).Send(runLocally: true);
    }

    public static VeinMiningPacket Get(Point center, int tileType)
    {
        var module = NetModuleLoader.Get<VeinMiningPacket>();
        module._center = center;
        module._tileType = (ushort) tileType;
        return module;
    }

    public override void Receive()
    {
        if (Main.netMode is NetmodeID.MultiplayerClient)
            return;

        VeinMinerSystem.MinerIndex = Sender;
        VeinMinerSystem.DoVeinMiningAt(_center, _tileType);
        VeinMinerSystem.MinerIndex = -1;
    }
}