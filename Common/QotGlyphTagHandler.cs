using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Common;

public class QotGlyphTagHandler : ITagHandler
{
    private class QotGlyphSnippet : GlyphTagHandler.GlyphSnippet
    {
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
            Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
        {
            if (CenteredItemTagHandler.ModernConfigDrawing)
            {
                position.X -= 4f;
                position.Y -= 18f;
            }

            return base.UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);
        }

        public QotGlyphSnippet(int index) : base(index)
        {
        }
    }

    TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
    {
        if (!int.TryParse(text, out var result) || result >= 26)
            return new TextSnippet(text);

        return new QotGlyphSnippet(result)
        {
            DeleteWhole = true,
            Text = "[qotglyph:" + result + "]"
        };
    }
}