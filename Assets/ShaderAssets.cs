using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.Assets;

internal class ShaderAssets : ModSystem
{
    public static Asset<Texture2D> Perlin;

    public override void Load()
    {
        if (Main.dedServ) return;

        SDFRectangle.Load();
        Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
    }

    public override void Unload()
    {
        if (Main.dedServ) return;

        SDFRectangle.Unload();
        Perlin = null;
    }
}