using ImproveGame.Common.Configs;

namespace ImproveGame.Common.GlobalItems
{
    public class FasterExtractinatorItem : GlobalItem
    {
        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if (ImproveConfigs.Instance.FasterExtractinator)
            {
                Main.LocalPlayer.itemAnimation = 1;
                Main.LocalPlayer.itemTime = 1;
                Main.LocalPlayer.itemTimeMax = 1;
            }
        }
    }
}
