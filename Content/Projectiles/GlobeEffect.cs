using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ImproveGame.Content.Projectiles
{
    public class GlobeEffect : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = -1;
            Projectile.scale = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
        }
        RenderTarget2D render = null;
        RenderTarget2D render2 = null;
        public override bool PreDraw(ref Color lightColor)
        {
            render ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            render2 ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            var spriteBatch = Main.spriteBatch;
            var graphicsDevice = Main.instance.GraphicsDevice;
            var screenTarget = Main.screenTarget;
            var screenTargetSwap = Main.screenTargetSwap;
            var tex = TextureAssets.Projectile[Type].Value;

            graphicsDevice.SetRenderTarget(screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);

            graphicsDevice.SetRenderTarget(render);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            for (int i = (int)Main.screenPosition.X / 16 * 16; i < Main.screenPosition.X + Main.screenWidth; i += 16)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(i - Main.screenPosition.X, 0), new Rectangle(0, 0, 1, 1), new Color(0, 120, 0), 1.57f, Vector2.Zero, new Vector2(Main.screenHeight, 2), SpriteEffects.None, 0);
            for (int i = (int)Main.screenPosition.Y / 16 * 16; i < Main.screenPosition.Y + Main.screenHeight; i += 16)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(0, i - Main.screenPosition.Y), new Rectangle(0, 0, 1, 1), new Color(0, 120, 0), 0, Vector2.Zero, new Vector2(Main.screenWidth, 2), SpriteEffects.None, 0);

            graphicsDevice.SetRenderTarget(render2);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, 0, tex.Size() / 2f, (90 - Projectile.timeLeft) / 1.2f, SpriteEffects.None, 0);

            graphicsDevice.SetRenderTarget(screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            graphicsDevice.Textures[1] = render;
            graphicsDevice.Textures[2] = render2;
            ModAsset.Mk2.Value.CurrentTechnique.Passes["Mask"].Apply();
            spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);

            // 复原
            graphicsDevice.Textures[0] = null;
            graphicsDevice.Textures[1] = null;
            graphicsDevice.Textures[2] = null;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            render = null;
            render2 = null;
        }
    }
}
