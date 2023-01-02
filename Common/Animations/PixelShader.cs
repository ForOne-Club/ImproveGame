namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
        public static readonly Texture2D texture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);

        /// <summary>
        /// 绘制叉号
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="round"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="border"></param>
        /// <param name="borderColor"></param>
        /// <param name="ui"></param>
        public static void DrawCross(Vector2 position, float size, float round, Color backgroundColor, float border, Color borderColor, bool ui = true)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.Cross.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(border)].SetValue(border);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.ReBegin(effect, ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(texture, position, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(null, ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// 圆角矩形，无边框
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="round"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="ui"></param>
        public static void DrawRoundRect(Vector2 pos, Vector2 size, float round, Color backgroundColor, bool ui = true)
        {
            pos -= new Vector2(1);
            size += new Vector2(2);
            round = MathHelper.Clamp(round + 1, 0, MathF.Min(size.X, size.Y) / 2);
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            sb.Draw(texture, pos, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// 圆角矩形，有边框，支持单独设置四个圆角
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="round4"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="ui"></param>
        public static void DrawRoundRect(Vector2 pos, Vector2 size, Vector4 round4, Color backgroundColor, bool ui = true)
        {
            pos -= new Vector2(1);
            size += new Vector2(2);
            round4 = Vector4.Max(new Vector4(1), round4) + new Vector4(1);
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round4)].SetValue(round4);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["NoBorderR4"].Apply();
            sb.Draw(texture, pos, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// 圆角矩形，有边框
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="round"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="border"></param>
        /// <param name="borderColor"></param>
        /// <param name="ui"></param>
        public static void DrawRoundRect(Vector2 pos, Vector2 size, float round, Color backgroundColor, float border, Color borderColor, bool ui = true)
        {
            pos -= new Vector2(1);
            size += new Vector2(2);
            round = MathHelper.Clamp(round + 1, 0, MathF.Min(size.X, size.Y) / 2);
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(border)].SetValue(border - 1);
            effect.Parameters[nameof(backgroundColor)].SetValue(backgroundColor.ToVector4());
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            effect.CurrentTechnique.Passes["HasBorder"].Apply();
            sb.Draw(texture, pos, null, Color.White, 0, new(0), size, 0, 1f);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// 进度条，有边框
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="round"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="border"></param>
        /// <param name="borderColor">[0, 1]</param>
        /// <param name="ui"></param>
        public static void DrawRoundRect(Vector2 pos, Vector2 size, float round, Color bgColor1, Color bgColor2, float border, Color borderColor, float progress, bool ui = true, int mode = 0)
        {
            pos -= new Vector2(1);
            size += new Vector2(2);
            round = MathHelper.Clamp(round + 1, 0, MathF.Min(size.X, size.Y) / 2);
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ModAssets.RoundRect2.Value;
            effect.Parameters[nameof(size)].SetValue(size);
            effect.Parameters[nameof(round)].SetValue(round);
            effect.Parameters[nameof(border)].SetValue(border - 1);
            effect.Parameters[nameof(bgColor1)].SetValue(bgColor1.ToVector4());
            effect.Parameters[nameof(bgColor2)].SetValue(bgColor2.ToVector4());
            effect.Parameters[nameof(borderColor)].SetValue(borderColor.ToVector4());
            effect.Parameters[nameof(progress)].SetValue(progress);
            sb.ReBegin(ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
            switch (mode)
            {
                default:
                    effect.CurrentTechnique.Passes["HasBorderProgressBarLeftRight"].Apply();
                    break;
                case 1:
                    effect.CurrentTechnique.Passes["HasBorderProgressBarLeftRight2"].Apply();
                    break;
                case 2:
                    effect.CurrentTechnique.Passes["HasBorderProgressBarTopBottom"].Apply();
                    break;
                case 3:
                    effect.CurrentTechnique.Passes["HasBorderProgressBarTopBottom2"].Apply();
                    break;
            }
            sb.Draw(texture, pos, null, Color.White, 0, new(0), size, 0, 1f);
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
