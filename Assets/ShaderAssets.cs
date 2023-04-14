using ImproveGame.Common.Animations;

namespace ImproveGame.Assets;

internal class ShaderAssets : ModSystem
{
    public static Effect SDFGraphic { get; private set; }

    public static Asset<Effect> LiquidSurface;
    public static Asset<Effect> Transform;
    public static Asset<Effect> Bloom;
    public static Asset<Texture2D> Perlin;

    public override void Load()
    {
        if (Main.dedServ) return;

        SDFRectangle.Laod();

        SDFGraphic = GetEffect("SDFGraphic").Value;

        LiquidSurface = GetEffect("LiquidSurface");
        Transform = GetEffect("Transform");
        Bloom = GetEffect("Bloom");
        Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
    }

    public override void Unload()
    {
        if (Main.dedServ) return;

        SDFRectangle.Unload();
        LiquidSurface = Transform = Bloom = null;
        Perlin = null;
    }
}