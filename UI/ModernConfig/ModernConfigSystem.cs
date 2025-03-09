using ImproveGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Chat;

namespace ImproveGame.UI.ModernConfig
{
    public class ModernConfigSystem:ModSystem
    {
        public override void Load()
        {
            On_ItemTagHandler.ItemSnippet.UniqueDraw += CenteredDrawFix;
            base.Load();
        }

        private bool CenteredDrawFix(On_ItemTagHandler.ItemSnippet.orig_UniqueDraw orig, Terraria.UI.Chat.TextSnippet self, bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
        {
            if (CenteredItemTagHandler.ModernConfigDrawing)
                position += Vector2.UnitY * -14;
            return orig(self, justCheckingString, out size, spriteBatch, position, color, scale);
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            //TooltipPanel.SetOption(null);
            base.PostDrawInterface(spriteBatch);
        }
    }
}
