using ImproveGame.Common.Players;
using static ImproveGame.Interface.Common.UISystem;

namespace ImproveGame.Interface.Common
{
    public class UIPlayer : ModPlayer
    {
        internal static Vector2 HugeInventoryUIPosition = new();

        // 函数在玩家进入地图时候调用, 不会在服务器调用, 用来加载 UI, 可以避免一些 HJson 文字未加载导致的问题.
        public override void OnEnterWorld(Player player)
        {
            DataPlayer dataPlayer = player.GetModPlayer<DataPlayer>();

            // 旗帜盒
            PackageInterface.SetState(Instance.PackageGUI = new());

            // 大背包
            BigBagInterface.SetState(Instance.BigBagGUI = new());
            Instance.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
            if (HugeInventoryUIPosition == Vector2.Zero)
                HugeInventoryUIPosition = new(150, 340);
            Instance.BigBagGUI.MainPanel.SetPos(HugeInventoryUIPosition).Recalculate();
        }
    }
}
