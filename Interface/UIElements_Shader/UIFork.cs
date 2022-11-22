using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using System.Threading;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class UIFork : UIElement
    {
        public float forkSize;
        public float radius;
        public float border;
        public AnimationTimer hoverTimer;

        public UIFork(float forkSize)
        {
            hoverTimer = new();
            radius = 3.7f;
            border = 2;
            this.forkSize = forkSize;
            Width.Pixels = forkSize + 20;
            Height.Pixels = forkSize + 10;
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
            Color background = Color.Lerp(UIColor.Default.TitleBackground * 0.25f, UIColor.Default.TitleBackground * 0.75f, hoverTimer.Schedule);
            Color fork = Color.Lerp(Color.Transparent, UIColor.Default.CloseBackground, hoverTimer.Schedule);

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 10f, background, 3f, UIColor.Default.CloseBorder);
            Vector2 forkPos = pos + size / 2 - new Vector2(forkSize / 2);
            PixelShader.DrawFork(forkPos, forkSize, radius, fork, border, UIColor.Default.CloseBorder);
        }
    }
}
