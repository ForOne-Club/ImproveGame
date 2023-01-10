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
            DataPlayer dataPlayer = player.GetModPlayer<DataPlayer>();

            // 玩家信息
            UISystem.Instance.PlayerInfoGUI = new PlayerInfoGUI();
            UISystem.PlayerInfoTrigger.SetState(UISystem.Instance.PlayerInfoGUI);
            PlayerInfoGUI.Visible = true;

            // 旗帜盒
            UISystem.Instance.PackageGUI = new PackageGUI();
            UISystem.Instance.PackageTrigger.SetState(UISystem.Instance.PackageGUI);

            // 大背包
            UISystem.Instance.BigBagGUI = new BigBagGUI();
            UISystem.BigBagTrigger.SetState(UISystem.Instance.BigBagGUI);

            UISystem.Instance.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
            if (HugeInventoryUIPosition == Vector2.Zero)
            {
                HugeInventoryUIPosition = new Vector2(150, 340);
            }

            UISystem.Instance.BigBagGUI.MainPanel.SetPos(HugeInventoryUIPosition).Recalculate();
        }
    }
}