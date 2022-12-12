using static Terraria.ModLoader.PlayerDrawLayer;

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

        public static void DrawShadow(Vector2 position, Vector2 size, float round, Color background, float shadowWidth)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRectShadow.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            effect.Parameters[nameof(shadowWidth)].SetValue(shadowWidth);
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

        public static void DrawLine(Matrix matrix, Vector2 pos, Vector2 start, Vector2 end, float width, Color background)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            start += -pos;
            end += -pos;


            if (start.X > end.X)
            {
                pos.X -= start.X - end.X;
                start.X += size.X - width;
                end.X += size.X - width;
            }
            else
            {
                start.X += width;
                end.X += width;
            }

            if (start.Y > end.Y)
            {
                pos.Y -= start.Y - end.Y;
                start.Y += size.Y - width;
                end.Y += size.Y - width;
            }
            else
            {
                start.Y += width;
                end.Y += width;
            }

            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Line.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters["start"].SetValue(start);
            effect.Parameters["end"].SetValue(end);
            effect.Parameters["width"].SetValue(width);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            sb.ReBegin(effect, matrix);
            sb.Draw(texture, pos - new Vector2(width), null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, matrix);
        }
    }
}
