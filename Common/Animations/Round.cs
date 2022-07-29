namespace ImproveGame.Common.Animations
{
    public class Round
    {
        public AnimationTimer timer; // √
        public float border; // √
        public float scale; // √
        public float extraScale; // √
        public Texture2D texture; // √
        public Color borderColor; // √
        public Color background; // √

        public Func<Vector2> Center; // x

        public float Scale => scale + (1 - timer.Schedule) * extraScale;
        public Vector2 Size => new(Scale);
        public Vector4 BorderColor
        {
            get
            {
                Vector4 vector4 = borderColor.ToVector4();
                vector4.W = timer.Schedule;
                return vector4;
            }
        }
        public Vector4 Background
        {
            get
            {
                Vector4 vector4 = background.ToVector4();
                vector4.W = timer.Schedule / 2f;
                return vector4;
            }
        }

        public Round(AnimationTimer timer, float border, Color borderColor, Color background, float scale = 50f, float extraScale = 50f)
        {
            this.timer = timer;
            this.border = border;
            this.borderColor = borderColor;
            this.background = background;
            this.scale = scale;
            this.extraScale = extraScale;

            texture = GetTexture("255").Value;
        }

        public void Draw()
        {
            Vector2 center = Center?.Invoke() ?? TransformToUIPosition(Main.MouseScreen);
            DrawRound(texture, Center(), Scale, Background, border, BorderColor);
        }

        public static void DrawRound(Texture2D texture, Vector2 center, float scale, Vector4 background, float border, Vector4 borderColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Effect effect = ModAssets.BorderRound.Value;
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor"].SetValue(borderColor);
            effect.Parameters["background"].SetValue(background);
            effect.Parameters["imageSize"].SetValue(new Vector2(scale));

            sb.BeginEffect(effect, Main.UIScaleMatrix);
            sb.Draw(texture, center, null, Color.White, 0, texture.Size() / 2f, scale, 0, 1f);
            sb.EndEffect(Main.UIScaleMatrix);
        }
    }
}
