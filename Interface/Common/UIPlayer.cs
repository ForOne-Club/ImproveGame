using ImproveGame.Common.Players;
using static ImproveGame.Interface.Common.UISystem;

namespace ImproveGame.Interface.Common
{
    public class UIPlayer : ModPlayer
    {
        // 用来加载 UI, 这个是玩家进入地图的时候可以避免一些 HJson 文字未加载导致的问题.
        public override void OnEnterWorld(Player player)
        {
            DataPlayer dataPlayer = player.GetModPlayer<DataPlayer>();
            PackageInterface.SetState(Instance.PackageGUI = new());
            BigBagInterface.SetState(Instance.BigBagGUI = new());
            Instance.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
            Instance.BigBagGUI.MainPanel.SetPos(dataPlayer.SuperVaultPos).Recalculate();
        }
    }
}
