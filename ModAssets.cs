namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> Fork;
        internal static Asset<Effect> RoundRectangle;
        internal static Asset<Effect> BoxShader;
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Texture2D> Perlin;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Fork = GetEffect(nameof(Fork));
                RoundRectangle = GetEffect(nameof(RoundRectangle));
                BoxShader = GetEffect("Box");
                ItemEffect = GetEffect("item");
                LiquidSurface = GetEffect("LiquidSurface");
                Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                Fork = null;
                RoundRectangle = null;
                BoxShader = null;
                ItemEffect = null;
                LiquidSurface = null;
                Perlin = null;
            }
        }
    }
}
