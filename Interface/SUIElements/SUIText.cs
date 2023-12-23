using ImproveGame.Common.Configs;
using ReLogic.Graphics;
using ReLogic.Text;
using System.Collections.Generic;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.SUIElements;

public class SUIText : TimerView
{
    /// <summary>
    /// 使用的字体
    /// </summary>
    public DynamicSpriteFont Font => _isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
    public string Text => _keyMode ? _textOrKey : Language.GetTextValue(_textOrKey);

    #region 常规可控属性

    /// <summary>
    /// 可以作为 HJson 的 Key，也可直接作为文本。
    /// <br/>
    /// 如果要作为 Key 请设置 KeyMode = true。
    /// </summary>
    public string TextOrKey
    {
        get => _keyMode ? _textOrKey : Language.GetTextValue(_textOrKey);
        set
        {
            if (_textOrKey != value)
            {
                _textOrKey = value;
                RecalculateText();
            }
        }
    }
    protected string _textOrKey = "";

    /// <summary>
    /// 使 TextOrKey 作为 HJson 的 Key 使用
    /// </summary>
    public bool KeyMode
    {
        get => _keyMode;
        set
        {
            if (_keyMode != value)
            {
                _keyMode = value;
                RecalculateText();
            }
        }
    }
    protected bool _keyMode = false;

    /// <summary>
    /// 使用放大版的字体
    /// </summary>
    public bool IsLarge
    {
        get => _isLarge;
        set
        {
            if (_isLarge != value)
            {
                _isLarge = value;
                RecalculateText();
            }
        }
    }
    protected bool _isLarge;

    /// <summary>
    /// 使文本不会越界 (横向)
    /// </summary>
    public bool IsWrapped
    {
        get => _isWrapped;
        set
        {
            if (_isWrapped != value)
            {
                _isWrapped = value;
                RecalculateText();
            }
        }
    }
    protected bool _isWrapped;

    /// <summary>
    /// 文字缩放比例
    /// </summary>
    public float TextScale = 1f;

    /// <summary>
    /// 文字颜色
    /// </summary>
    public Color TextColor = Color.White;

    /// <summary>
    /// 文字边框颜色
    /// </summary>
    public Color TextBorderColor = Color.Black;

    public Vector2 TextOffset = Vector2.Zero;
    public Vector2 TextOrigin = Vector2.Zero;
    public Vector2 TextAlign = Vector2.Zero;
    public Vector2 TextPercentOffset = Vector2.Zero;
    #endregion

    #region 非常规可控属性
    /// <summary>
    /// 展示文本
    /// </summary>
    public TextSnippet[] VisibleTextSnippets { get; protected set; }

    /// <summary>
    /// 最后一次的 inner 宽度
    /// </summary>
    public float LastInnerWidth { get; protected set; }

    /// <summary>
    /// 最后一次的文本
    /// </summary>
    public string LastText { get; protected set; }

    /// <summary>
    /// 文字大小
    /// </summary>
    public Vector2 TextSize { get; protected set; } = Vector2.Zero;
    #endregion

    /// <summary>
    /// 修改后会影响 “文本大小” 的属性
    /// <br/>
    /// - keyMode
    /// <br/>
    /// - text
    /// <br/>
    /// - isLarge
    /// <br/>
    /// 当开启 isWrapped 的时候，除了以上属性变动，在宽度限制改变时也需要刷新
    /// </summary>
    public void RecalculateText()
    {
        LastText = Text;

        if (_isWrapped)
        {
            List<TextSnippet> finalSnippets = [];
            List<List<TextSnippet>> firstSnippets = TextSnippetHelper.WordwrapString(LastText, TextColor, Font, GetInnerDimensions().Width / TextScale);

            foreach (List<TextSnippet> snippets in firstSnippets)
            {
                finalSnippets.AddRange(snippets);
                finalSnippets.Add(new TextSnippet("\n"));
            }

            if (finalSnippets.Count > 0 && finalSnippets[^1].Text == "\n")
            {
                finalSnippets.RemoveAt(finalSnippets.Count - 1);
            }

            VisibleTextSnippets = [.. finalSnippets];
        }
        else
        {
            VisibleTextSnippets = [.. TextSnippetHelper.ConvertNormalSnippets(TextSnippetHelper.ParseMessage(LastText, TextColor))];
        }

        TextSize = ChatManager.GetStringSize(Font, VisibleTextSnippets, new Vector2(1f));
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var inner = GetInnerDimensions();

        if (LastText != Text || (_isWrapped && LastInnerWidth != inner.Width))
        {
            LastInnerWidth = inner.Width;
            RecalculateText();
        }

        Vector2 innerSize = inner.Size();
        Vector2 innerPos = inner.Position();

        Vector2 textSize = TextSize;
        Vector2 textPos = innerPos + TextOffset + TextPercentOffset * innerSize + (innerSize - textSize * TextScale) * TextAlign;
        textPos.Y += TextScale * (_isLarge ? UIConfigs.Instance.BigFontOffsetY : UIConfigs.Instance.GeneralFontOffsetY);
        textPos -= TextOrigin * TextSize * TextScale;

        ChatManager.DrawColorCodedStringShadow(spriteBatch, Font, VisibleTextSnippets,
            textPos, TextBorderColor, 0f, Vector2.Zero, new Vector2(TextScale), -1f, 1.5f);

        ChatManager.DrawColorCodedString(spriteBatch, Font, VisibleTextSnippets,
            textPos, Color.White, 0f, Vector2.Zero, new Vector2(TextScale), out var _, -1f);

        /*DrawBorderString(spriteBatch, Font, textPos, VisibleText,
            TextColor, TextBorderColor, TextOrigin * TextSize, TextScale, 2f);*/
    }

    /// <summary>
    /// 绘制带有边框的文字
    /// </summary>
    public static void DrawBorderString(SpriteBatch spriteBatch, DynamicSpriteFont font, Vector2 position,
        string text, Color textColor, Color borderColor, Vector2 origin, float textScale, float border = 2f)
    {
        border *= textScale;

        float x = position.X;
        float y = position.Y;
        Color color = borderColor;

        if (borderColor == Color.Transparent)
        {
            spriteBatch.DrawString(font, text, position, textColor, 0f, origin, textScale, 0, 0f);
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    position.X = x - border;
                    position.Y = y;
                    break;
                case 1:
                    position.X = x + border;
                    position.Y = y;
                    break;
                case 2:
                    position.X = x;
                    position.Y = y - border;
                    break;
                case 3:
                    position.X = x;
                    position.Y = y + border;
                    break;
                default:
                    position.X = x;
                    position.Y = y;
                    color = textColor;
                    break;
            }

            spriteBatch.DrawString(font, text, position, color, 0f, origin, textScale, 0, 0f);
        }
    }
}
