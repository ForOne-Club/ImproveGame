using Microsoft.Xna.Framework.Graphics;

namespace ImproveGame
{
    public class ModAssets : ModSystem
    {
        public static Effect ItemEffect = MyUtils.GetEffect("item").Value;

        public override void Load() {
            ItemEffect = MyUtils.GetEffect("item").Value;
        }

        public override void Unload() {
            ItemEffect = null;
        }
    }
}
