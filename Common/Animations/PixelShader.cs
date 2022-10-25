namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
        public static readonly Texture2D texture = GetTexture("255").Value; // √

        public static void DrawTest(Matrix matrix, Vector2 position, Vector2 size, float round, Color backgroundColor, float border, Color borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            Effect effect = ModAssets.RoundRectangle.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, effect, matrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, matrix);
        }

        public static void DrawRound(Texture2D texture, Vector2 center, float scale, Vector4 background, float border, Vector4 borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Effect effect = ModAssets.BorderRound.Value;
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor"].SetValue(borderColor);
            effect.Parameters["background1"].SetValue(new Vector4(1f, 1f, 1f, 0.5f));
            effect.Parameters["imageSize"].SetValue(new Vector2(scale));

            sb.Begin(effect, Main.UIScaleMatrix);
            sb.Draw(texture, center, null, Color.White, 0, texture.Size() / 2f, scale, 0, 1f);
            sb.Begin(null, Main.UIScaleMatrix);
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
