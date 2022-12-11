namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
        public static readonly Texture2D texture = new(Main.graphics.GraphicsDevice, 1, 1);

        // 绘制一个叉号
        public static void DrawFork(Vector2 position, float size, float round, Color backgroundColor, float border, Color borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Fork.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.ReBegin(effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, Main.UIScaleMatrix);
        }

        // DrawRoundRect 现在有两个 .fx 文件，一个带边框的，一个不带的，也许能节省性能？
        public static void DrawRoundRect(Vector2 position, Vector2 size, float round, Color background)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRectNoBorder2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            sb.ReBegin(effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, Main.UIScaleMatrix);
        }

        public static void DrawRoundRect(Vector2 position, Vector2 size, float round, Color backgroundColor, float border, Color borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(border)].SetValue(border - 1);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            sb.ReBegin(effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, Main.UIScaleMatrix);
        }

        public static void DrawShadow(Vector2 position, Vector2 size, float round, Color background, float shadowSize)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRectShadow.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round + shadowSize / 2);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            effect.Parameters[nameof(shadowSize)].SetValue(shadowSize);
            sb.ReBegin(effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, Main.UIScaleMatrix);
        }

        public static void DrawRound(Vector2 position, float size, Color background)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Round.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            sb.ReBegin(effect, Main.UIScaleMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, Main.UIScaleMatrix);
        }
    }
}
