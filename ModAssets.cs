using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        internal static Asset<Effect> ItemEffect;
        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Texture2D> Perlin;

        public override void Load() {
            if (!Main.dedServ) {
                ItemEffect = MyUtils.GetEffect("item");
                LiquidSurface = MyUtils.GetEffect("LiquidSurface");
                Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
            }
        }

        public override void Unload() {
            if (!Main.dedServ) {
                ItemEffect = null;
                LiquidSurface = null;
                Perlin = null;
            }
        }
    }
}
