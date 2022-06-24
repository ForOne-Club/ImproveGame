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
            UserInterface.ActiveInstance = UISystem.JuBigVaultInterface;
            base.DrawSelf(spriteBatch);
            UserInterface.ActiveInstance = temp;
        }

        public override void MouseDown(UIMouseEvent evt) {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = UISystem.JuBigVaultInterface;
            base.MouseDown(evt);
            UserInterface.ActiveInstance = temp;
        }
    }
}
