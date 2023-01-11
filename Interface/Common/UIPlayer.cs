using ImproveGame.Common.Players;
using ImproveGame.Interface.BannerChest;
using ImproveGame.Interface.GUI;
using ImproveGame.Interface.PlayerInfo;

namespace ImproveGame.Interface.Common
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIPlayer : ModPlayer
    {
        internal static Vector2 HugeInventoryUIPosition;

        // 函数在玩家进入地图时候调用, 不会在服务器调用, 用来加载 UI, 可以避免一些因 HJson 未加载产生的问题.
        public override void OnEnterWorld(Player player)
        {
            UISystem uiSystem = UISystem.Instance;
            DataPlayer dataPlayer = player.GetModPlayer<DataPlayer>();

            // 玩家信息
            uiSystem.PlayerInfoGUI = new PlayerInfoGUI();
            uiSystem.PlayerInfoTrigger.SetViewCarrier(UISystem.Instance.PlayerInfoGUI);
            PlayerInfoGUI.Visible = true;

            // 旗帜盒
            uiSystem.PackageGUI = new PackageGUI();
            uiSystem.PackageTrigger.SetViewCarrier(UISystem.Instance.PackageGUI);

            // 大背包
            uiSystem.BigBagGUI = new BigBagGUI();
            uiSystem.BigBagTrigger.SetViewCarrier(UISystem.Instance.BigBagGUI);

            uiSystem.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
            if (HugeInventoryUIPosition == Vector2.Zero)
                HugeInventoryUIPosition = new Vector2(150, 340);
            uiSystem.BigBagGUI.MainPanel.SetPos(HugeInventoryUIPosition).Recalculate();
        }
    }
}