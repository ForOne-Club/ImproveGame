namespace ImproveGame.Common.ModSystems;

public class NetPasswordSystem : ModSystem
{
    /// <summary>
    /// 允许调节设置的密码，只会在服务器有值
    /// </summary>
    internal static string ConfigPassword;
    internal static bool[] Registered = new bool[Main.maxPlayers];

    public override void Load() {
        On_Netplay.StartServer += SendPassword;
    }

    // 关掉掉线玩家的认证
    public override void PreUpdateWorld()
    {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Registered[i] && !Netplay.Clients[i].Socket.IsConnected()) {
                Registered[i] = false;
            }
        }
    }
        
    private void SendPassword(On_Netplay.orig_StartServer orig) {
        orig.Invoke();

        if (!Config.OnlyHostByPassword)
            return;

        for (int i = 0; i < Main.maxPlayers; i++) {
            Registered[i] = false;
        }
            
        for (int i = 0; i < 4; i++)
            ConfigPassword += (char)Main.rand.Next('A', 'Z' + 1);

        ImproveGame.Instance.Logger.Info(GetTextWith("Configs.ImproveConfigs.OnlyHostByPassword.ServerPasswordLog", new {
            Password = ConfigPassword
        }));
            
        Console.WriteLine(GetTextWith("Configs.ImproveConfigs.OnlyHostByPassword.ServerPassword", new {
            Password = ConfigPassword
        }));
    }

    public static bool Permitted(int whoAmI)
    {
        if (Main.netMode is not NetmodeID.Server)
            return true;
        if (Config.OnlyHostByPassword && !Registered[whoAmI])
            return false;
        if (Config.OnlyHost && !Netplay.Clients[whoAmI].Socket.GetRemoteAddress().IsLocalHost())
            return false;
        return true;
    }
}