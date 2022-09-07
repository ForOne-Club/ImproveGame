namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> BoxShader;
        internal static Asset<Effect> BorderRound;
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Texture2D> Perlin;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BoxShader = GetEffect("Box");
                BorderRound = GetEffect("BorderRound");
                ItemEffect = GetEffect("item");
                LiquidSurface = GetEffect("LiquidSurface");
                Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                BoxShader = null;
                BorderRound = null;
                ItemEffect = null;
                LiquidSurface = null;
                Perlin = null;
            }
        }
    }
}
