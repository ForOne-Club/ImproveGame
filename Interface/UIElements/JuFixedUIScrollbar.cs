using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ImproveGame.Interface.UIElements
{
    public class JuFixedUIScrollbar : UIScrollbar
    {
        protected override void DrawSelf(SpriteBatch spriteBatch) {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = UISystem.JuBigVaultInterface;
            base.DrawSelf(spriteBatch);
            UserInterface.ActiveInstance = temp;
        }

        public override void MouseDown(UIMouseEvent evt) {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = UISystem.JuBigVaultInterface;
            base.MouseDown(evt);
            UserInterface.ActiveInstance = temp;
            ScrollWheelValue = -120;
        }

        private float ScrollWheelValue = -120;
        public void SetViewPosition(int ScrollWheelValue) {
            this.ScrollWheelValue = ViewPosition - ScrollWheelValue;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (ScrollWheelValue > -120) {
                ViewPosition += (ScrollWheelValue - ViewPosition) / 5f;
            }
        }
    }
}
