using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseUIEs;

namespace ImproveGame.Interface.PlayerInfo.UIElements
{
    public class PlyInfoSwitch : View
    {
        public Func<bool> Opened;
        public Texture2D open;
        public float round;
        public AnimationTimer hoverTimer;
        public PlyInfoSwitch()
        {
            hoverTimer = new AnimationTimer(3);
            Height.Pixels = 40f;
            open = GetTexture("UI/PlayerInfo/Open").Value;

            Relative = RelativeMode.Horizontal;
            Wrap = true;
            round = 10f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            hoverTimer.Update();
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

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            Color background = new Color(0, 0, 25, (int)MathHelper.Lerp(64, 128, hoverTimer.Schedule));
            Color border = new Color(0, 0, 25, (int)MathHelper.Lerp(192, 255, hoverTimer.Schedule));
            PixelShader.DrawRoundRect(pos, size, round, new Color(0, 0, 0, 0.5f), new Color(0, 0, 0, 0.25f), 3, new Color(0, 0, 0, 0.75f), hoverTimer.Schedule, mode: 3);

            if (Opened is not null)
            {
                float rotation;
                if (Opened())
                {
                    rotation = MathF.PI;
                }
                else
                {
                    rotation = 0;
                }
                sb.Draw(open, pos + size / 2, null, Color.White, rotation, open.Size() / 2f, 1f, 0, 0f);
            }
        }
    }
}
