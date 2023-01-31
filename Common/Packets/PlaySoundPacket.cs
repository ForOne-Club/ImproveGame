namespace ImproveGame.Common.Packets;

/// <summary>
/// 没错，专门搞一个包，就只为了飘洋过海播放一个声音。用的是数字ID，多亏了AssemblyPublicizer。
/// </summary>
[AutoSync]
public class PlaySoundPacket : NetModule
{
    private sbyte _soundID;
        
    public static PlaySoundPacket Get(int soundID)
    {
        var packet = ModContent.GetInstance<PlaySoundPacket>();
        packet._soundID = (sbyte)soundID;
        return packet;
    }

    public override void Receive()
    {
        if (Main.netMode is NetmodeID.Server) return;
        SoundEngine.PlaySound(_soundID);
    }
}