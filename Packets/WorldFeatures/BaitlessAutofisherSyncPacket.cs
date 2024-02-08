using ImproveGame.Content;
using Terraria.DataStructures;

namespace ImproveGame.Packets.WorldFeatures;

public class BaitlessAutofisherSyncPacket : NetModule
{
    private List<Point16> _positions;
    
    public static void Sync(List<Point16> positions)
    {
        if (Main.netMode is NetmodeID.MultiplayerClient)
            return;

        var module = NetModuleLoader.Get<BaitlessAutofisherSyncPacket>();
        module._positions = positions;
        module.Send(runLocally: true); // 单人支持
    }

    public override void Send(ModPacket p)
    {
        p.Write(_positions);
    }

    public override void Read(BinaryReader r)
    {
        _positions = r.ReadPoint16List().ToList();
    }

    public override void Receive()
    {
        StructureDatas.BaitlessAutofisherPositions = _positions;
    }
}