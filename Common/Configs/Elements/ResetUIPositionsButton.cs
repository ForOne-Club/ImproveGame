using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;

namespace ImproveGame.Common.Configs.Elements;

internal class ResetUIPositionsButton : LargerPanelElement
{
    public override void LeftClick(UIMouseEvent evt) {
        base.LeftClick(evt);

        // 重设UI的位置
        UIPlayer.HugeInventoryUIPosition = UIPlayer.HugeInventoryDefPosition;
        UIPlayer.BuffTrackerPosition = UIPlayer.BuffTrackerDefPosition;
        UIPlayer.WorldFeaturePosition = UIPlayer.WorldFeatureDefPosition;
        UIPlayer.ItemSearcherPosition = UIPlayer.ItemSearcherDefPosition;
        UIPlayer.OpenBagPosition = UIPlayer.OpenBagDefPosition;
        UIPlayer.PlayerInfoTogglePosition = UIPlayer.PlayerInfoToggleDefPosition;

        // 应用
        UISystem uiSystem = UISystem.Instance;
        uiSystem.BigBagGUI.MainPanel.SetPos(UIPlayer.HugeInventoryUIPosition).Recalculate();
        uiSystem.BuffTrackerGUI.MainPanel.SetPos(UIPlayer.BuffTrackerPosition).Recalculate();
        uiSystem.WorldFeatureGUI.MainPanel.SetPos(UIPlayer.WorldFeaturePosition).Recalculate();
        uiSystem.ItemSearcherGUI.MainPanel.SetPos(UIPlayer.ItemSearcherPosition).Recalculate();
        uiSystem.OpenBagGUI.MainPanel.SetPos(UIPlayer.OpenBagPosition).Recalculate();
        uiSystem.PlayerInfoGUI.ControllerSwitch.SetPosPixels(UIPlayer.PlayerInfoTogglePosition).Recalculate();
    }
}