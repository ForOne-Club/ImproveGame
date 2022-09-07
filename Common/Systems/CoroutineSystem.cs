using Terraria.ObjectData;

namespace ImproveGame.Common.Systems
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
