using System.Reflection;

namespace ImproveGame.Assets
{
    // 在这里加一个 internal 就不用再给字段设置了
    internal class ShaderAssets : ModSystem
    {
        public static EffectPass SpriteEffectPass { get; private set; }
        public static Effect Line { get; private set; }
        public static Effect RoundedRectangle { get; private set; }
        public static Effect Round { get; private set; }
        public static Effect Cross { get; private set; }

        public static Asset<Effect> LiquidSurface;
        public static Asset<Effect> Transform;
        internal static Asset<Effect> Bloom;
        internal static Asset<Texture2D> Perlin;

        public static Texture2D Shader1, Shader2, Shader3;
        public static Effect Effect1;

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            // BetterFiltering = null;

            // Shader UI
            RoundedRectangle = Round = Cross = Line = null;
            LiquidSurface = null;
            Transform = null;
            Bloom = null;
            Perlin = null;

            Shader1 = Shader2 = Shader3 = null;
            Effect1 = null;

            SpriteEffectPass = null;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Shader1 = GetTexture("Shader_1").Value;
            Shader2 = GetTexture("Shader_2").Value;
            Shader3 = GetTexture("Shader_3").Value;
            Effect1 = GetEffect("Trail").Value;

            // Shader UI
            RoundedRectangle = GetEffect("RoundRectangle").Value;
            Round = GetEffect("Round").Value;
            Cross = GetEffect("Cross").Value;
            Line = GetEffect("Line").Value;

            SpriteEffectPass = Main.spriteBatch.spriteEffectPass;

            // BetterFiltering = GetEffect("BetterFiltering");
            LiquidSurface = GetEffect("LiquidSurface");
            Transform = GetEffect("Transform");
            Bloom = GetEffect("Bloom");
            Perlin = Main.Assets.Request<Texture2D>("Images/Misc/Perlin");
        }
    }
}