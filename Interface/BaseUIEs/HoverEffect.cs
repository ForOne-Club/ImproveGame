using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.BaseUIEs
{
    /// <summary>
    /// 大背包按钮背景上的效果，需要直接继承此类即可。(继承自 RelativeUIE)
    /// </summary>
    public class HoverEffect : RelativeElement
    {
        public bool border;
        public AnimationTimer hoverTimer;
        public Color startColor;
        public Color endColor;
        public float startWidth;
        public float endWidth;
        public float round;

        public HoverEffect()
        {
            hoverTimer = new(3);
            round = 10;
            startColor = new Color(0, 0, 0, 0);
            endColor = new Color(0, 0, 0, 0.5f);
            startWidth = 0f;
            endWidth = 4f;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            hoverTimer.Open();
            base.MouseOver(evt);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            hoverTimer.Close();
            base.MouseOut(evt);
        }

        public override void Update(GameTime gameTime)
        {
            hoverTimer.Update();
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            Vector2 shadow = Vector2.Lerp(new(startWidth), new(endWidth), hoverTimer.Schedule);
            Color color = Color.Lerp(startColor, endColor, hoverTimer.Schedule);

            if (border)
            {
                PixelShader.DrawRoundRect(pos - shadow, size + shadow * 2, round + shadow.X, Color.Transparent, 3f, color);
            }
            else
            {
                PixelShader.DrawRoundRect(pos - shadow, size + shadow * 2, round + shadow.X, color);
            }
        }
    }
}
