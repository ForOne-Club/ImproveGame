using ImproveGame.Content.Functions;

namespace ImproveGame.Packets
{
    internal class RefreshShopPacket : NetModule
    {
        public static RefreshShopPacket Get() => NetModuleLoader.Get<RefreshShopPacket>();

        public override void Send(ModPacket p)
        {
            if (Main.netMode is NetmodeID.Server)
            {
                for (int i = 0; i < 40; i++)
                {
                    p.Write((short)Main.travelShop[i]);
                }
            }
            else if (!RefreshTravelShopSystem.Refreshing)
            {
                RefreshTravelShopSystem.Refreshing = true;
            }
        }

        public override void Read(BinaryReader r)
        {
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 40; i++)
                {
                    Main.travelShop[i] = r.ReadInt16();
                }
                RefreshTravelShopSystem.Refreshing = false;
            }
        }

        /// <summary>
        /// 只有单人模式和服务器有必要运行
        /// </summary>
        public override void Receive()
        {
            if (Main.netMode is NetmodeID.SinglePlayer)
            {
                Chest.SetupTravelShop();
                SoundEngine.PlaySound(SoundID.Chat);
            }
            if (Main.netMode is NetmodeID.Server)
            {
                Chest.SetupTravelShop();
                Send(runLocally: false); // 发回给全体玩家
            }
        }
    }
}
