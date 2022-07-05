using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ImproveGame.Interface.UIElements
{
    public class JuFixedUIScrollbar : UIScrollbar
    {
        protected override void DrawSelf(SpriteBatch spriteBatch) {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = UISystem.BigBagInterface;
            base.DrawSelf(spriteBatch);
            UserInterface.ActiveInstance = temp;
        }

        public override void MouseDown(UIMouseEvent evt) {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = UISystem.BigBagInterface;
            base.MouseDown(evt);
            UserInterface.ActiveInstance = temp;
            ScrollWheelValue = 0;
        }

        private float ScrollWheelValue;
        public void SetViewPosition(int ScrollWheelValue) {
            this.ScrollWheelValue -= ScrollWheelValue;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (ScrollWheelValue != 0) {
                ViewPosition += ScrollWheelValue * 0.2f;
                ScrollWheelValue *= 0.8f;
                if (MathF.Abs(ScrollWheelValue) < 0.001f) {
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
