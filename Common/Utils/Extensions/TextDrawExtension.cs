using ImproveGame.Common.RenderTargetContents;
using ReLogic.Graphics;
using Terraria.DataStructures;

namespace ImproveGame.Common.Utils.Extensions;

public static class TextDrawExtension
{
    public static void DrawItemStackByRenderTarget(this SpriteBatch sb, int stack, Vector2 position, float scale)
    {
        if (stack < 0)
        {
            DrawItemStackString(sb, stack.ToString(), position, scale);
            return;
        }

        DynamicSpriteFont font = FontAssets.ItemStack.Value;
        var text = stack.ToString();
        foreach (char c in text)
        {
            var numberTarget = RenderTargetContentSystem.StackNumberRenderTargets[c - '0'];
            numberTarget.Request();
            if (!numberTarget.IsReady) continue;

            RenderTarget2D target = numberTarget.GetTarget();
            sb.Draw(target, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            position.X += scale * (target.Width - 2);
        }
    }

    /// <summary>
    /// 专门用于绘制物品栏物品的堆叠，直接调用字体的 InternalDraw 相比 TrUtils.DrawBorderString 性能消耗更小
    /// 且这个的描边大小经过调节，看起来更舒服
    /// </summary>
    public static void DrawItemStackString(this SpriteBatch sb, string text, Vector2 position, float scale)
    {
        DynamicSpriteFont font = FontAssets.ItemStack.Value;
        Color color = Color.Black * 0.8f;
        Vector2 zero = Vector2.Zero;
        float x = position.X;
        float y = position.Y;
        float spread = 1.5f * scale;
        for (int index = 0; index <= 4; ++index)
        {
            switch (index)
            {
                case 0:
                    zero.X = x - spread;
                    zero.Y = y;
                    break;
                case 1:
                    zero.X = x + spread;
                    zero.Y = y;
                    break;
                case 2:
                    zero.X = x;
                    zero.Y = y - spread;
                    break;
                case 3:
                    zero.X = x;
                    zero.Y = y + spread;
                    break;
                default:
                    zero.X = x;
                    zero.Y = y;
                    color = Color.White;
                    break;
            }

            sb.DrawString(font, text, zero, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }
    }
}