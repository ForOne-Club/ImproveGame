namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> RoundRectangle;
        internal static Asset<Effect> BoxShader;
        internal static Asset<Effect> BorderRound;
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Texture2D> Perlin;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RoundRectangle = GetEffect(nameof(RoundRectangle));
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
                RoundRectangle = null;
                BoxShader = null;
                BorderRound = null;
                ItemEffect = null;
                LiquidSurface = null;
                Perlin = null;
            }
        }
    }
}
