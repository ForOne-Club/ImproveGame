namespace ImproveGame.Assets;

// 在这里加一个 internal 就不用再给字段加了
internal class ShaderAssets : ModSystem
{
    public static EffectPass SpriteEffectPass { get; private set; }
    public static Effect Line { get; private set; }
    public static Effect RoundedRectangle { get; private set; }
    public static Effect Round { get; private set; }
    public static Effect Cross { get; private set; }

    public static Asset<Effect> LiquidSurface;
    public static Asset<Effect> Transform;
    public static Asset<Effect> Bloom;
    public static Asset<Texture2D> Perlin;

    public override void Unload()
    {
        if (Main.dedServ)
            return;

        RoundedRectangle = Round = Cross = Line = null;
        LiquidSurface = null;
        Transform = null;
        Bloom = null;
        Perlin = null;

        SpriteEffectPass = null;
    }

    public override void Load()
    {
        if (Main.dedServ)
            return;

        RoundedRectangle = GetEffect("RoundRectangle").Value;
        Round = GetEffect("Round").Value;
        Cross = GetEffect("Cross").Value;
        Line = GetEffect("Line").Value;

        SpriteEffectPass = Main.spriteBatch.spriteEffectPass;

        LiquidSurface = GetEffect("LiquidSurface");
        Transform = GetEffect("Transform");
        Bloom = GetEffect("Bloom");
        Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
    }
}