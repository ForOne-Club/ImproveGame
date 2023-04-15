namespace ImproveGame.Common.GlobalItems
{
    public class FasterExtractinatorItem : GlobalItem
    {
        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if (Config.FasterExtractinator)
            {
                Main.LocalPlayer.itemAnimation = 1;
                Main.LocalPlayer.itemTime = 1;
                Main.LocalPlayer.itemTimeMax = 1;
            }
        }
    }
}
