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

        public Round(AnimationTimer timer, Color borderColor, Color background, float scale = 50f, float extraScale = 50f)
        {
            Vector2 vel = new(1);
            vel.ToRotation();
            this.timer = timer;
            this.borderColor = borderColor;
            this.background = background;
            this.scale = scale;
            this.extraScale = extraScale;

            texture = GetTexture("255").Value;
            border = 2f;
        }

        public void DrawSelf(SpriteBatch sb)
        {
            Vector2 center = Center?.Invoke() ?? TransformToUIPosition(Main.MouseScreen);
            sb.Draw(texture, center, null, Color.White, 0, texture.Size() / 2f, Scale, 0, 1f);
        }

        public void Draw()
        {
            SpriteBatch sb = Main.spriteBatch;

            Effect effect = ModAssets.BorderRound.Value;
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor"].SetValue(BorderColor);
            effect.Parameters["background"].SetValue(Background);
            effect.Parameters["imageSize"].SetValue(Size);

            sb.BeginEffect(effect, Main.UIScaleMatrix);
            DrawSelf(sb);
            sb.EndEffect(Main.UIScaleMatrix);
        }
    }
}
