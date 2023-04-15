using ImproveGame.Common.Configs;
using ImproveGame.Common.Players;
using ImproveGame.Interface.BannerChest;
using ImproveGame.Interface.ExtremeStorage;
using ImproveGame.Interface.GUI;
using ImproveGame.Interface.PlayerInfo;

namespace ImproveGame.Interface.Common
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIPlayer : ModPlayer
    {
        internal static Vector2 HugeInventoryUIPosition;

        // 函数在玩家进入地图时候调用, 不会在服务器调用, 用来加载 UI, 可以避免一些因 HJson 未加载产生的问题.
        public override void OnEnterWorld()
        {
            UISystem uiSystem = UISystem.Instance;
            DataPlayer dataPlayer = Player.GetModPlayer<DataPlayer>();

            // 玩家信息
            uiSystem.PlayerInfoGUI = new PlayerInfoGUI();
            uiSystem.PlayerInfoTrigger.SetCarrier(uiSystem.PlayerInfoGUI);
            PlayerInfoGUI.Visible = true;

            // 旗帜盒
            uiSystem.PackageGUI = new PackageGUI();
            uiSystem.PackageTrigger.SetCarrier(uiSystem.PackageGUI);

            // 大背包
            uiSystem.BigBagGUI = new BigBagGUI();
            uiSystem.BigBagTrigger.SetCarrier(uiSystem.BigBagGUI);

            uiSystem.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
            if (HugeInventoryUIPosition.X <= 0 && HugeInventoryUIPosition.X >= Main.screenWidth)
                HugeInventoryUIPosition.X = 150;
            if (HugeInventoryUIPosition.Y <= 0 && HugeInventoryUIPosition.Y >= Main.screenHeight)
                HugeInventoryUIPosition.Y = 340;
            uiSystem.BigBagGUI.MainPanel.SetPos(HugeInventoryUIPosition).Recalculate();
            
            // 左侧栏
            uiSystem.ExtremeStorageGUI = new ExtremeStorageGUI();
            uiSystem.AutofisherGUI = new AutofisherGUI();
            SidedEventTrigger.Clear();
            SidedEventTrigger.RegisterViewBody(uiSystem.ExtremeStorageGUI);
            SidedEventTrigger.RegisterViewBody(uiSystem.AutofisherGUI);
        }
    }
}