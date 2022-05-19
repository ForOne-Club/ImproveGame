using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ImproveGame.Content
{
    public class TileDraw : ModSystem
    {
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer,
                (Effect)null, Main.Transform);
            // Main.spriteBatch.Begin();
            DrawTiles();
            Main.spriteBatch.End();
        }

        public static Rectangle MagiskTilesRec = new Rectangle();
        public static Color MagiskTileColor;

        private static void DrawTiles()
        {
            if (Main.LocalPlayer.GetModPlayer<Common.ModPlayers.UpdatePlayer>().MagiskKillTiles)
            {
                Texture2D texture = TextureAssets.MagicPixel.Value;
                Vector2 UpperLeft = new Vector2(MagiskTilesRec.X, MagiskTilesRec.Y);
                Vector2 UpperLeftScreen = UpperLeft * 16f - Main.screenPosition;
                Vector2 BrushSize = new Vector2(MagiskTilesRec.Width, MagiskTilesRec.Height);
                // Color yellow = new Color(1f, 0.9f, 0.1f, 1f);
                Main.spriteBatch.Draw(
                    texture,
                    UpperLeftScreen,
                    new(0, 0, 1, 1),
                    MagiskTileColor * 0.35f,
                    0f,
                    Vector2.Zero,
                    16f * BrushSize,
                    SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(
                    texture,
                    UpperLeftScreen + Vector2.UnitX * -2f,
                    new(0, 0, 1, 1),
                    MagiskTileColor, 0f, Vector2.Zero,
                    new Vector2(2f, 16f * BrushSize.Y),
                    SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture,
                    UpperLeftScreen + Vector2.UnitX * 16f * BrushSize.X,
                    new(0, 0, 1, 1),
                    MagiskTileColor, 0f, Vector2.Zero,
                    new Vector2(2f, 16f * BrushSize.Y), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture,
                    UpperLeftScreen + Vector2.UnitY * -2f,
                    new(0, 0, 1, 1),
                    MagiskTileColor, 0f, Vector2.Zero,
                    new Vector2(16f * BrushSize.X, 2f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture,
                    UpperLeftScreen + Vector2.UnitY * 16f * BrushSize.Y,
                    new(0, 0, 1, 1),
                    MagiskTileColor, 0f, Vector2.Zero,
                    new Vector2(16f * BrushSize.X, 2f), SpriteEffects.None, 0f);
            }
        }
    }
}
