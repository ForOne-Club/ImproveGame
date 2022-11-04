using ImproveGame.Common.Players;

namespace ImproveGame.Common.Packets;

public class MouseWorldPacket : NetModule
{
    private Vector2 mouseWorld;
    private byte player;

    public static MouseWorldPacket Get(byte player, Vector2 mouseWorld)
    {
        var packet = NetModuleLoader.Get<MouseWorldPacket>();
        packet.player = player;
        packet.mouseWorld = mouseWorld;
        return packet;
    }
    public override void Read(BinaryReader r)
    {
        player = r.ReadByte();
        mouseWorld = r.ReadVector2();
    }

    public override void Receive()
    {
        Main.player[player].GetModPlayer<ImprovePlayer>().MouseWorld = mouseWorld;
        if (Main.netMode == NetmodeID.Server)
        {
            Send(-1, player, false);
        }
    }

    public override void Send(ModPacket p)
    {
        p.Write(player);
        p.WriteVector2(mouseWorld);
    }
}
