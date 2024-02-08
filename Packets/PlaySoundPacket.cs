using ImproveGame.Common.Configs;
using Terraria.DataStructures;

namespace ImproveGame.Packets;

/// <summary>
/// 没错，专门搞一个包，就只为了飘洋过海播放一个声音。用的是数字ID，多亏了AssemblyPublicizer。
/// </summary>
[AutoSync]
public class PlaySoundPacket : NetModule
{
    private byte _soundID;
    private Point16 _position; // 存short物块坐标，减少包大小
    private byte _style;
    
    public static void PlaySound(int soundID, Vector2? position = null, int style = 1, bool playForSelf = true) =>
        Get(soundID, position, style).Send(runLocally: playForSelf);
        
    public static PlaySoundPacket Get(int soundID, Vector2? position = null, int style = 1)
    {
        var packet = ModContent.GetInstance<PlaySoundPacket>();
        packet._soundID = (byte)soundID;
        packet._position = (position ?? Vector2.Zero).ToTileCoordinates16();
        packet._style = (byte)style;
        return packet;
    }

    public override void Receive()
    {
        if (Main.netMode is NetmodeID.Server)
        {
            Send(-1, Sender);
            return;
        }
        
        if (!UIConfigs.Instance.ExplosionEffect && _soundID is LegacySoundIDs.Item && _style is 14)
            return;

        if (_position != Point16.Zero)
            SoundEngine.PlaySound(_soundID, _position.ToWorldCoordinates(), _style);
        else
            SoundEngine.PlaySound(_soundID, Style: _style);
    }
}