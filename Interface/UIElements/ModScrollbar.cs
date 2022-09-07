using Microsoft.Xna.Framework.Graphics;

namespace ImproveGame.Interface.UIElements
{
    public class ModScrollbar : FixedUIScrollbar
    {
        public bool Visible = true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                base.DrawSelf(spriteBatch);
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            ScrollWheelValue = 0;
        }

        private float ScrollWheelValue;

        public ModScrollbar(UserInterface userInterface) : base(userInterface)
        {

        }

        public void SetViewPosition(int ScrollWheelValue)
        {
            this.ScrollWheelValue -= ScrollWheelValue;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ScrollWheelValue != 0)
            {
                ViewPosition += ScrollWheelValue * 0.2f;
                ScrollWheelValue *= 0.8f;
                if (MathF.Abs(ScrollWheelValue) < 0.001f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 3);
                    ScrollWheelValue = 0;
                }
            }
        }

        /*private float ScrollWheelValue = -120;
        public void SetViewPosition(int ScrollWheelValue) {
            this.ScrollWheelValue = ViewPosition - ScrollWheelValue;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (ScrollWheelValue > -120) {
                ViewPosition += (ScrollWheelValue - ViewPosition) / 5f;
            }
        }*/
    }
}
