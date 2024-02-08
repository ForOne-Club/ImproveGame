using ImproveGame.Common.ModPlayers;
using System.Diagnostics;

namespace ImproveGame.Packets.NetStorager;

[AutoSync]
public class OpenStateUpdatePacket : NetModule
{
    private ushort _openState;

    public static void SendClose()
    {
        Send(-1);
    }

    public static void Send(int openState)
    {
        Debug.Assert(Main.netMode is not NetmodeID.Server, "Packet cannot be sent by server");
        var packet = ModContent.GetInstance<OpenStateUpdatePacket>();
        packet._openState = (ushort)openState;
        packet.Send();
    }

    public override void Receive()
    {
        if (!Main.player[Sender].TryGetModPlayer<ExtremeStoragePlayer>(out var modPlayer))
            return;

        modPlayer.UsingStorage = _openState;
    }
}