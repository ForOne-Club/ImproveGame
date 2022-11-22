namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
        public static readonly Texture2D texture = new(Main.graphics.GraphicsDevice, 1, 1);

        public static void DrawFork(Vector2 position, float size, float round, Color backgroundColor, float border, Color borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            Effect effect = ModAssets.Fork.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.Begin(SpriteSortMode.Immediate, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
            effect.CurrentTechnique.Passes["Fork"].Apply();
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }

        // DrawRoundRect 现在有两个 .fx 文件，一个带边框的，一个不带的，也许能节省性能？
        public static void DrawRoundRect(Vector2 position, Vector2 size, float round, Color backgroundColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            Effect effect = ModAssets.RoundRectNoBorder.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }

        public static void DrawRoundRect(Vector2 position, Vector2 size, float round, Color backgroundColor, float border, Color borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            Effect effect = ModAssets.RoundRect.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }

        public static void DrawBox(Matrix matrix, Vector2 position, Vector2 size, float radius, float border, Color borderColor, Color background)
        {
            DrawBox(matrix, position, size, radius, border, borderColor, borderColor, background, background);
        }

        public static void DrawBox(Matrix matrix, Vector2 position, Vector2 size, float radius, float border, Color borderColor1, Color borderColor2, Color background1, Color background2)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.BoxShader.Value;

            effect.Parameters["size"].SetValue(size);
            effect.Parameters["radius"].SetValue(radius);
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor1"].SetValue(borderColor1.ToVector4());
            effect.Parameters["borderColor2"].SetValue(borderColor2.ToVector4());
            effect.Parameters["background1"].SetValue(background1.ToVector4());
            effect.Parameters["background2"].SetValue(background2.ToVector4());

            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, effect, matrix);

            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);

            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, matrix);
        }
    }
}
