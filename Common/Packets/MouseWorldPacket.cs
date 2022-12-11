using ImproveGame.Common.Players;

namespace ImproveGame.Common.Packets;

[AutoSync]
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

    public override void Receive()
    {
        Main.player[player].GetModPlayer<ImprovePlayer>().MouseWorld = mouseWorld;
        if (Main.netMode == NetmodeID.Server)
        {
            Send(-1, player, false);
        }
    }
}