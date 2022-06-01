using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ImproveGame.Content
{
    public class TileDraw : ModSystem
    {
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer,
                null, Main.Transform);
            DrawTiles();
            Main.spriteBatch.End();
        }

        public static bool allowDrawBorderRect;
        public static Rectangle tileRect = new Rectangle();
        public static Color tileColor;

        private static void DrawTiles()
        {
            if (allowDrawBorderRect)
            {
                MyUtils.DrawBorderRect(tileRect, tileColor * 0.35f, tileColor);
                allowDrawBorderRect = false;
            }
        }
    }
}
