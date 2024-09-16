using ImproveGame.Common.ModSystems;

namespace ImproveGame.Packets;

/// <summary>
/// 告诉某个客户端他已经被认证了
/// </summary>
public class AuthorizedPacket : NetModule
{
    public static void Send(int toClient) => ((NetModule) NetModuleLoader.Get<AuthorizedPacket>()).Send(toClient);

    /// <summary>
    /// 只有多人模式客户端会收到这个包
    /// </summary>
    public override void Receive()
    {
        NetPasswordSystem.LocalPlayerRegistered = true;
    }
}