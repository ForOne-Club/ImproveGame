namespace ImproveGame.Common.Systems
{
    public class CoroutineSystem : ModSystem
    {
        internal static CoroutineRunner TileRunner = new();

        public override void PreUpdateTime()
        {
            TileRunner.Update(1);
        }
    }
}
