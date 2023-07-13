using ImproveGame.Core;

namespace ImproveGame.Common.ModSystems
{
    public class CoroutineSystem : ModSystem
    {
        internal static CoroutineRunner TileRunner = new();
        internal static CoroutineRunner GenerateRunner = new();

        public override void PreUpdateTime()
        {
            TileRunner.Update(1);
            GenerateRunner.Update(1);
        }
    }
}
