namespace ImproveGame.Common.GlobalItems
{
    public class GlobalItemData : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool ShouldHaveInvGlowForBanner = false;
        public bool InventoryGlow;
    }
}
