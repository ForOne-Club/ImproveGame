using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ReLogic.Graphics;
using System.Globalization;
using Terraria.GameContent.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Common;

/// <summary>
/// 绘制一个代码上没宽度的指针
/// </summary>
public class CursorSnippet : TextSnippet
{
    private int _kerningOffset; // 补偿指针前字符的kerning
    
    public static TextSnippet Parse(string text, int kerning)
    {
        CursorSnippet textSnippet = new()
        {
            _kerningOffset = kerning
        };
        if (!int.TryParse(text, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var result))
            return textSnippet;

        textSnippet.Color = new Color((result >> 16) & 0xFF, (result >> 8) & 0xFF, result & 0xFF);
        return textSnippet;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
        Vector2 position = default, Color color = default, float scale = 1f)
    {
        size = new Vector2(_kerningOffset, 0);

        // 这里两个color判断的大小写不一样，是故意的，一个是判断局部变量，一个是全局变量
        // 局部变量color好像没有alpha通道
        if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0) && Color != Color.Transparent)
        {
            var drawPos = position;
            drawPos.Y -= 2;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos, new Rectangle(0, 0, 1, 24), color, 0f,
                Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        return true;
    }

    public static string GenerateTag(Color color)
    {
        if (color == Color.Transparent)
            return "%$#??>transparent%$#??>";
        return $"%$#??>{color.Hex3()}%$#??>";
    }

    public override float GetStringLength(DynamicSpriteFont font) => 0f;
}