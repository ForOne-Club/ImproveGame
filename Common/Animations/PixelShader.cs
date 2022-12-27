using static Terraria.ModLoader.PlayerDrawLayer;

namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
        public static readonly Texture2D texture = new(Main.graphics.GraphicsDevice, 1, 1);

        // 绘制一个叉号
        public static void DrawFork(Vector2 position, float size, float round, Color backgroundColor, float border, Color borderColor, bool ui = true)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Fork.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.ReBegin(effect, ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        // DrawRoundRect 现在有两个 .fx 文件，一个带边框的，一个不带的，也许能节省性能？
        public static void DrawRoundRect(Vector2 position, Vector2 size, float round, Color backgroundColor, bool ui = true)
        {
            position -= new Vector2(2);
            size += new Vector2(4);
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round + 2);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawRoundRect(Vector2 position, Vector2 size, float round, Color backgroundColor, float border, Color borderColor, bool ui = true)
        {
            position -= new Vector2(2);
            size += new Vector2(4);
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round + 2);
            effect.Parameters[nameof(border)].SetValue(border - 1);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["HasBorder"].Apply();
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawShadow(Vector2 position, Vector2 size, float round, Color backgroundColor, float shadow, bool ui = true)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            effect.Parameters[nameof(shadow)].SetValue(shadow);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["Shadow"].Apply();
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawRound(Vector2 position, float size, Color background, bool ui = true)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Round.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawRound(Vector2 position, float size, Color background, float border, Color borderColor, bool ui = true)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Round.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["HasBorder"].Apply();
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawLine(Vector2 start, Vector2 end, float width, Color background, bool ui = true)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            start += new Vector2(width) - min;
            end += new Vector2(width) - min;

            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Line.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters["start"].SetValue(start);
            effect.Parameters["end"].SetValue(end);
            effect.Parameters["width"].SetValue(width);
            effect.Parameters[nameof(background)].SetValue(background.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            sb.Draw(texture, min - new Vector2(width), null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, bool ui = true)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            start += new Vector2(width) - min;
            end += new Vector2(width) - min;

            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Line.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters["start"].SetValue(start);
            effect.Parameters["end"].SetValue(end);
            effect.Parameters["width"].SetValue(width);
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor"].SetValue(borderColor.ToVector4());
            effect.Parameters["background"].SetValue(background.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["HasBorder"].Apply();
            sb.Draw(texture, min - new Vector2(width), null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
