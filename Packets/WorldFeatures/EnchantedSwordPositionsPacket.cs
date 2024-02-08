using ImproveGame.Content;
using Terraria.DataStructures;

namespace ImproveGame.Packets.WorldFeatures;

public class EnchantedSwordPositionsPacket : NetModule
{
    private List<Point16> _positions;
    
    public static void Sync()
    {
        if (Main.netMode is NetmodeID.MultiplayerClient)
            return;

        var module = NetModuleLoader.Get<EnchantedSwordPositionsPacket>();
        module._positions = StructureDatas.EnchantedSwordPositions;
        module.Send();
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
        StructureDatas.EnchantedSwordPositions = _positions;
    }
}