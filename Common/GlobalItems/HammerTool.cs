namespace ImproveGame.Common.GlobalItems
{
    /// <summary>
    /// 锤子右键，一键选取锤取形状
    /// </summary>
    public class HammerTool : GlobalItem
    {
        public override bool AltFunctionUse(Item item, Player player) => item.hammer > 0;

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.hammer > 0)
            {
                if (player.altFunctionUse == 2)
                {
                    item.useStyle = ItemUseStyleID.Shoot;
                }
                else if (player.altFunctionUse == 0)
                {
                    item.useStyle = ItemUseStyleID.Swing;
                }
            }
            return true;
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (item.hammer > 0)
            {
                if (Main.mouseLeft)
                {
                    player.itemTime = item.useTime;
                    player.itemAnimation = item.useAnimation;
                    ItemRotation(player);
                }
                else
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                }
            }
            return base.UseItem(item, player);
        }
    }
}
