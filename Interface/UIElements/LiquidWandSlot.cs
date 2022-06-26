using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.UI;

namespace ImproveGame.Interface.UIElements
{
    public class LiquidWandSlot : ModImageButton
    {
        public LiquidWandSlot(Color hoverColor, Color normalColor) : base(null, normalColor, hoverColor) { }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);
        }
    }
}
