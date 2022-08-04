using ImproveGame.Common.ConstructCore;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using System.Threading;

namespace ImproveGame.Content.Items
{
    public class ConstructWand : SelectorItem
    {
        public override bool ModifySelectedTiles(Player player, int i, int j) => true;

        public override void PostModifyTiles(Player player, int minI, int minJ, int maxI, int maxJ)
        {
            var rectangle = new Rectangle(minI, minJ, maxI - minI, maxJ - minJ);
            var saveTileThread = new Thread(() => FileOperator.SaveAsFile(rectangle));
            saveTileThread.Start();
        }

        public override void SetItemDefaults()
        {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.mana = 20;

            SelectRange = new(40, 30);
        }

        public override bool StartUseItem(Player player)
        {
            if (player.altFunctionUse == 0)
            {
                ItemRotation(player);
            }
            else if (player.altFunctionUse == 2)
            {
                if (StructureGUI.Visible)
                    UISystem.Instance.StructureGUI.Close();
                else
                    UISystem.Instance.StructureGUI.Open();
                return false;
            }

            return base.StartUseItem(player);
        }

        public override bool IsNeedKill() => !Main.mouseLeft;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseSelector(Player player)
        {
            return WandSystem.ConstructMode is WandSystem.Construct.Save;
        }
    }
}
