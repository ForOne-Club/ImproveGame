using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class UIFork : UIElement
    {
        public float size;
        public float radius;
        public float border;
        public AnimationTimer hoverTimer;

        public UIFork(float size)
        {
            hoverTimer = new();
            radius = 3.7f;
            border = 2;
            this.size = size;
            Width.Pixels = size + 20;
            Height.Pixels = size + 10;
        }

        public override void Update(GameTime gameTime)
        {
            hoverTimer.Update();
            base.Update(gameTime);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            hoverTimer.Open();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            hoverTimer.Close();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Color background = Color.Lerp(UIColor.Default.TitleBackground * 0.5f, UIColor.Default.TitleBackground * 0.75f, hoverTimer.Schedule);
            Color fork = Color.Lerp(Color.Transparent, UIColor.Default.CloseBackground, hoverTimer.Schedule);

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRectangle(pos, size, 8f, background, 0, Color.Transparent);
            PixelShader.DrawFork(pos + size / 2 - new Vector2(this.size / 2), this.size, radius,
                fork, border, UIColor.Default.CloseBorder);
        }
    }
}
