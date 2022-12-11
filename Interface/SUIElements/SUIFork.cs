using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUIFork : UIElement
    {
        public float forkSize;
        public float radius;
        public float border;
        public AnimationTimer hoverTimer = new(3);

        public SUIFork(float forkSize)
        {
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
            SoundEngine.PlaySound(SoundID.MenuTick);
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
            PixelShader.DrawRoundRect(pos, size, 10f, background, 3f, UIColor.Default.PanelBorder);
            Vector2 forkPos = pos + size / 2 - new Vector2(forkSize / 2);
            PixelShader.DrawFork(forkPos, forkSize, radius, fork, border, UIColor.Default.PanelBorder);
        }
    }
}
