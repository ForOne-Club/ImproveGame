namespace ImproveGame.UI.DeathSpectating;

[AutoSync]
public class SpectatingPlayerPacket : NetModule
{
    public int SpectatingTarget;

    public static void SynsSpectatingTarget(int spectatingTarget)
    {
        var sp = NetModuleLoader.Get<SpectatingPlayerPacket>();
        sp.SpectatingTarget = spectatingTarget;
        sp.Send();
    }

    public override void Receive()
    {
        if (Main.player.IndexInRange(Sender) && Main.player[Sender].TryGetModPlayer(out SpectatingPlayer sp))
        {
            sp.SpectatingTarget = SpectatingTarget;
        }
    }
}
