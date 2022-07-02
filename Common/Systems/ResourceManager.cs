using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    internal class ResourceManager : ModSystem
    {
        internal const string EffectBase = "ImproveGame/Assets/Effect/";

        internal static Asset<Effect> LiquidSurface;
        internal static Asset<Texture2D> Perlin;

        public override void PostSetupContent() {
            if (!Main.dedServ) {
                LiquidSurface = ModContent.Request<Effect>($"{EffectBase}LiquidSurface");
                Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
            }
            base.PostSetupContent();
        }

        public override void Unload() {
            if (!Main.dedServ) {
                LiquidSurface = null;
                Perlin = null;
            }
        }
    }
}
