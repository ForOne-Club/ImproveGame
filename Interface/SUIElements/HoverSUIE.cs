using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 大背包按钮背景上的效果，需要直接继承此类即可。
    /// </summary>
    public class HoverSUIE : UIElement
    {
        public AnimationTimer hoverTimer;
        public Color hoverColor1;
        public Color hoverColor2;
        public float hoverWidth1;
        public float hoverWidth2;

        public HoverSUIE()
        {
            hoverTimer = new(3);
            hoverColor1 = new Color(0, 0, 0, 0);
            hoverColor2 = new Color(0, 0, 0, 0.5f);
            hoverWidth1 = 0f;
            hoverWidth2 = 4f;
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

            Vector2 shadow = Vector2.Lerp(new(hoverWidth1), new(hoverWidth2), hoverTimer.Schedule);
            Color color = Color.Lerp(hoverColor1, hoverColor2, hoverTimer.Schedule);
            PixelShader.DrawRoundRect(pos - shadow, size + shadow * 2, 10 + shadow.X, color);
        }
    }
}
