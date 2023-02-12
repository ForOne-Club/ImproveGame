namespace ImproveGame.Assets;

internal class ShaderAssets : ModSystem
{
    public static EffectPass SpriteEffectPass { get; private set; }
    public static Effect SDFGraphic { get; private set; }
    public static Effect SDFRectangle { get; private set; }

    public static Asset<Effect> LiquidSurface;
    public static Asset<Effect> Transform;
    public static Asset<Effect> Bloom;
    public static Asset<Texture2D> Perlin;

    public override void Load()
    {
        if (Main.dedServ) return;

        SDFGraphic = GetEffect("SDFGraphic").Value;
        SDFRectangle = GetEffect("RoundRectangle").Value;

        SpriteEffectPass = Main.spriteBatch.spriteEffectPass;

        LiquidSurface = GetEffect("LiquidSurface");
        Transform = GetEffect("Transform");
        Bloom = GetEffect("Bloom");
        Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
    }

    public override void Unload()
    {
        if (Main.dedServ) return;

        SDFGraphic = SDFRectangle = null;
        LiquidSurface = Transform = Bloom = null;
        Perlin = null;

        SpriteEffectPass = null;
    }
}