namespace ImproveGame.Common.Systems
{
    public class NetPasswordSystem : ModSystem
    {
        /// <summary>
        /// 允许调节设置的密码，只会在服务器有值
        /// </summary>
        internal static string ConfigPassword;
        internal static bool[] Registered = new bool[255];

        public override void Load() {
            On.Terraria.Netplay.StartServer += SendPassword;
            On.Terraria.Main.Update += CheckConnected;
        }

        // 关掉掉线玩家的认证
        private void CheckConnected(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime) {
            orig.Invoke(self, gameTime);
            if (Main.netMode != NetmodeID.Server)
                return;
            for (int i = 0; i < 255; i++) {
                if (Registered[i] && !Netplay.Clients[i].Socket.IsConnected()) {
                    Registered[i] = false;
                }
            }
        }

        private void SendPassword(On.Terraria.Netplay.orig_StartServer orig) {
            orig.Invoke();

            if (!Config.OnlyHostByPassword)
                return;

            for (int i = 0; i < 255; i++) {
                Registered[i] = false;
            }

            for (int i = 0; i < 8; i++)
                ConfigPassword += Main.rand.Next(10).ToString();

            ImproveGame.Instance.Logger.Info(GetTextWith("Config.OnlyHostByPassword.ServerPasswordLog", new {
                Password = ConfigPassword
            }));

            Console.WriteLine(GetTextWith("Config.OnlyHostByPassword.ServerPassword", new {
                Password = ConfigPassword
            }));
        }
    }
}
