using ImproveGame.Common;
using ReLogic.Graphics;
using ReLogic.Text;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.Common;

public static class TextSnippetHelper
{
    /// <summary>
    /// 将文本转换为文本片段 <br/>
    /// 根据格式: [tag/options:text] 拆分
    /// </summary>
    public static List<TextSnippet> ParseMessage(string input, Color baseColor)
    {
        // 删除文本中回车 (怎么会有回车捏?)
        input = input.Replace("\r", "");

        // 创建正则列表
        MatchCollection matchCollection = ChatManager.Regexes.Format.Matches(input);

        // 文字片段列表
        List<TextSnippet> snippets = [];
        int inputIndex = 0;

        // 遍历匹配到的正则之间的文本
        foreach (Match match in matchCollection.Cast<Match>())
        {
            // match.Index 是该匹配到的正则在原文中的其实位置
            // match.Length 是该正则在原文中的长度

            // 如果有, 添加两正则之间的文本.
            if (match.Index > inputIndex)
            {
                // AddTextSnippetWithCursorCheck(input[inputIndex..match.Index], snippets, baseColor);
                snippets.Add(new TextSnippet(input[inputIndex..match.Index], baseColor));
            }

            // 移动下标至当前正则后的第一个字符
            inputIndex = match.Index + match.Length;

            // 获取指定文本
            string tag = match.Groups["tag"].Value;
            string text = match.Groups["text"].Value;
            string options = match.Groups["options"].Value;

            ITagHandler handler = ChatManager.GetHandler(tag);

            // 没有, 插入文本
            if (handler is null)
            {
                snippets.Add(new TextSnippet(text, baseColor));
            }
            // 如果有, 按照处理程序设定插入
            else
            {
                TextSnippet snippet = handler.Parse(text, baseColor, options);
                snippet.TextOriginal = match.ToString();
                snippets.Add(snippet);
            }
        }

        // 如果有, 添加最后一个正则后的文本.
        if (input.Length > inputIndex)
        {
            // AddTextSnippetWithCursorCheck(input[inputIndex..], snippets, baseColor);
            snippets.Add(new TextSnippet(input[inputIndex..], baseColor));
        }

        return snippets;
    }

    public static List<TextSnippet> ParseMessageWithCursorCheck(string input, Color baseColor)
    {
        string pattern = @"%\$#\?\?>\s*(.*?)\s*%\$#\?\?>";

        Match match = Regex.Match(input, pattern);
        if (!match.Success)
            return ParseMessage(input, baseColor);

        var snippets = new List<TextSnippet>();
        string result = match.Groups[1].Value;
        string textBefore = input[..match.Index];
        string textAfter = input[(match.Index + match.Length)..];

        // 消除插入文本不同字符间的kerning的影响，避免鼠标闪烁时文字位置不稳定
        float leftSize = FontAssets.MouseText.Value.MeasureString(textBefore).X;
        float rightSize = FontAssets.MouseText.Value.MeasureString(textAfter).X;
        float togetherSize = FontAssets.MouseText.Value.MeasureString(textBefore + textAfter).X;
        int kerning = (int)(togetherSize - (leftSize + rightSize));

        if (textBefore is {Length: > 0 })
            snippets.AddRange(ParseMessage(textBefore, baseColor));

        var cursorSnippet = CursorSnippet.Parse(result, kerning);
        if (result == "transparent")
            cursorSnippet.Color = Color.Transparent;
        snippets.Add(cursorSnippet);

        if (textAfter is {Length: > 0 })
            snippets.AddRange(ParseMessage(textAfter, baseColor));

        return snippets;
    }

    public static void AddTextSnippetWithCursorCheck(string text, List<TextSnippet> snippets, Color baseColor)
    {
        // 匹配所有被 %$#??> 包裹的文本
        // 比如 %$#??>你的文本%$#??>
        // 用来标记指针位置
        string pattern = @"%\$#\?\?>\s*(.*?)\s*%\$#\?\?>";

        Match match = Regex.Match(text, pattern);
        if (match.Success)
        {
            string result = match.Groups[1].Value;
            string textBefore = text[..match.Index];
            string textAfter = text[(match.Index + match.Length)..];

            // 消除插入文本不同字符间的kerning的影响，避免鼠标闪烁时文字位置不稳定
            float leftSize = FontAssets.MouseText.Value.MeasureString(textBefore).X;
            float rightSize = FontAssets.MouseText.Value.MeasureString(textAfter).X;
            float togetherSize = FontAssets.MouseText.Value.MeasureString(textBefore + textAfter).X;
            int kerning = (int)(togetherSize - (leftSize + rightSize));

            if (textBefore is {Length: > 0 })
                snippets.Add(new TextSnippet(textBefore, baseColor));

            Main.NewText(kerning);

            snippets.Add(CursorSnippet.Parse(result, kerning));

            if (textAfter is {Length: > 0 })
                snippets.Add(new TextSnippet(textAfter, baseColor));
            return;
        }

        snippets.Add(new TextSnippet(text, baseColor));
    }

    /// <summary>
    /// 把 TextSnippet 转换为 PlainSnippet, 因为 PlainSnippet 文字颜色不会闪烁.<br/>
    /// 不会修改传入的, 请使用返回值.
    /// </summary>
    public static List<TextSnippet> ConvertNormalSnippets(List<TextSnippet> originalSnippets)
    {
        List<TextSnippet> finalSnippets = [];

        for (int i = 0; i < originalSnippets.Count; i++)
        {
            TextSnippet snippet = originalSnippets[i];

            if (originalSnippets[i].GetType() == typeof(TextSnippet))
            {
                snippet = new PlainTagHandler.PlainSnippet(snippet.Text, snippet.Color, snippet.Scale);
            }

            finalSnippets.Add(snippet);
        }

        return finalSnippets;
    }

    /// <summary>
    /// 针对textSnippet特殊文本的换行
    /// </summary>
    public static TextSnippet[] WordwrapString(TextSnippet[] originalSnippets, Color textColor, DynamicSpriteFont font,
        int maxWidth, out float lastLineLength, int maxCharacterCount = 19, int maxLines = -1)
    {
        int lineCount = 1; // 行数
        float workingLineLength = 0f; // 当前行长度
        List<TextSnippet> finalSnippets = new() {new TextSnippet()};

        foreach (var snippet in originalSnippets)
        {
            if (snippet is PlainTagHandler.PlainSnippet)
            {
                string cacheString = ""; // 缓存字符串 - 准备输入的字符
                for (int i = 0; i < snippet.Text.Length; i++)
                {
                    GlyphMetrics characterMetrics = font.GetCharacterMetrics(snippet.Text[i]);
                    workingLineLength += font.CharacterSpacing + characterMetrics.KernedWidth;

                    if (workingLineLength > maxWidth && !char.IsWhiteSpace(snippet.Text[i]))
                    {
                        // 如果第一个字符是空格，单词长度小于19（实际上是18因为第一个字符为空格），可以空格换行
                        bool canWrapWord = cacheString.Length > 1 && cacheString.Length < maxCharacterCount;

                        // 找不到空格，或者拆腻子，则强制换行
                        if (!canWrapWord || (i > 0 && CanBreakBetween(snippet.Text[i - 1], snippet.Text[i])))
                        {
                            finalSnippets.Add(new PlainTagHandler.PlainSnippet(cacheString, snippet.Color));
                            finalSnippets.Add(new PlainTagHandler.PlainSnippet("\n"));
                            workingLineLength = characterMetrics.KernedWidthOnNewLine;
                            cacheString = "";
                            lineCount++;
                        }
                        // 空格换行
                        else
                        {
                            // 由于下面那一段“将CJK字符与非CJK字符分割”可能会导致空格换行后的第一个字符不是空格，所以这里手动加一个空格
                            // 就不改下面的cacheString[1..]了
                            if (cacheString[0] != ' ')
                                cacheString = " " + cacheString;
                            finalSnippets.Add(new PlainTagHandler.PlainSnippet("\n"));
                            finalSnippets.Add(new PlainTagHandler.PlainSnippet(cacheString[1..], snippet.Color));
                            workingLineLength = font.MeasureString(cacheString).X;
                            cacheString = "";
                            lineCount++;
                        }
                    }

                    // 这么做可以分割单词，并且使自然分割单词（即不因换行过长强制分割的单词）第一个字符总是空格
                    // 或者是将CJK字符与非CJK字符分割
                    if (cacheString != string.Empty && (char.IsWhiteSpace(snippet.Text[i]) ||
                                                        IsCjk(cacheString[^1]) != IsCjk(snippet.Text[i])))
                    {
                        finalSnippets.Add(new PlainTagHandler.PlainSnippet(cacheString, snippet.Color));
                        cacheString = "";
                    }

                    // 原有换行则将当前行长度重置
                    if (snippet.Text[i] is '\n')
                    {
                        workingLineLength = 0;
                        lineCount++;
                    }

                    cacheString += snippet.Text[i];
                }

                finalSnippets.Add(new PlainTagHandler.PlainSnippet(cacheString, snippet.Color));
            }
            else
            {
                float length = snippet.GetStringLength(font);
                workingLineLength += length;
                // 超了 - 换行再添加，注意起始长度
                if (workingLineLength > maxWidth)
                {
                    workingLineLength = length;
                    finalSnippets.Add(new PlainTagHandler.PlainSnippet("\n"));
                    lineCount++;
                }

                finalSnippets.Add(snippet);
            }

            if (lineCount >= maxLines + 1 && maxLines != -1)
            {
                // 一直向前移除到最后一个换行，并把最后一个换行也移除
                int linesToBeRemoved = lineCount - maxLines;
                for (int i = 0; i < linesToBeRemoved; i++)
                {
                    while (finalSnippets.Count > 1 && finalSnippets[^1].Text != "\n")
                    {
                        finalSnippets.RemoveAt(finalSnippets.Count - 1);
                    }

                    finalSnippets.RemoveAt(finalSnippets.Count - 1);
                }

                lastLineLength = 0;
                return finalSnippets.ToArray();
            }
        }

        lastLineLength = workingLineLength;
        return finalSnippets.ToArray();
    }

    /// <summary>
    /// 针对textSnippet特殊文本的换行
    /// </summary>
    public static TextSnippet[] WordwrapString(string text, Color textColor, DynamicSpriteFont font, int maxWidth,
        out float lastLineLength, int maxCharacterCount = 19, int maxLines = -1)
    {
        TextSnippet[] originalSnippets = ChatManager.ParseMessage(text, textColor).ToArray();
        ChatManager.ConvertNormalSnippets(originalSnippets);
        return WordwrapString(originalSnippets, textColor, font, maxWidth, out lastLineLength, maxCharacterCount,
            maxLines);
    }

    // https://unicode-table.com/cn/blocks/cjk-unified-ideographs/ 中日韩统一表意文字
    // https://unicode-table.com/cn/blocks/cjk-symbols-and-punctuation/ 中日韩符号和标点
    public static bool IsCjk(char a)
    {
        return (a >= 0x4E00 && a <= 0x9FFF) || (a >= 0x3000 && a <= 0x303F);
    }

    internal static bool CanBreakBetween(char previousChar, char nextChar)
    {
        if (IsCjk(previousChar) || IsCjk(nextChar))
            return true;

        return false;
    }

    /* 有Bug，不支持英文空格换行
    public static List<List<TextSnippet>> WordwrapString(string input, Color color, DynamicSpriteFont font, float maxWidth)
    {
        List<TextSnippet> originalSnippets = ConvertNormalSnippets(ParseMessage(input, color));

        List<List<TextSnippet>> firstSnippets = [];
        List<TextSnippet> cacheSnippets = [];

        #region 处理原文中的换行
        foreach (TextSnippet textSnippet in originalSnippets)
        {
            // 以换行分隔开
            string[] strings = textSnippet.Text.Split('\n');

            // length - 1:
            // 最后一行开头前分行
            for (int i = 0; i < strings.Length - 1; i++)
            {
                cacheSnippets.Add(textSnippet.CopyMorph(strings[i]));
                firstSnippets.Add(cacheSnippets);
                cacheSnippets = [];
            }

            // 最后一行开头
            cacheSnippets.Add(textSnippet.CopyMorph(strings[^1]));
        }

        firstSnippets.Add(cacheSnippets);
        cacheSnippets = [];
        #endregion

        #region 根据限制宽度添加换行
        // 最终列表
        List<List<TextSnippet>> finalSnippets = [];

        if (maxWidth > 0)
        {
            foreach (List<TextSnippet> snippets in firstSnippets)
            {
                float cacheWidth = 0f;

                foreach (TextSnippet snippet in snippets)
                {
                    // 简单片段
                    if (snippet is PlainTagHandler.PlainSnippet)
                    {
                        float width = snippet.GetStringLength(font);

                        // 越界, 计算在哪个字符位置断开换行
                        if (cacheWidth + width > maxWidth)
                        {
                            // 缓存字符串
                            string cacheString = "";

                            // 遍历当前字符串
                            foreach (char cacheChar in snippet.Text)
                            {
                                // 此字符宽度
                                float kernedWidth = font.GetCharacterMetrics(cacheChar).KernedWidth;

                                // 缓存宽度 + 间距 + 此字符宽度超行从此处断开
                                // cacheString 加入 cacheSnippets, cacheSnippets 加入 finalSnippets, cacheChar 加入 cacheString
                                // cacheWidth 等于 kernedWidth
                                if (cacheWidth + font.CharacterSpacing + kernedWidth > maxWidth)
                                {
                                    if (!string.IsNullOrEmpty(cacheString))
                                    {
                                        cacheSnippets.Add(snippet.CopyMorph(cacheString));
                                    }

                                    finalSnippets.Add(cacheSnippets);
                                    cacheSnippets = [];

                                    cacheString = cacheChar.ToString();
                                    cacheWidth = kernedWidth;
                                }
                                // 不越界, cacheWidth 加上 字间距和字符宽度
                                else
                                {
                                    cacheString += cacheChar;
                                    cacheWidth += font.CharacterSpacing + kernedWidth;
                                }
                            }

                            // 遍历完, 如果有, 剩下的 cacheString 加入到 cacheSnippets
                            if (!string.IsNullOrEmpty(cacheString))
                            {
                                cacheSnippets.Add(snippet.CopyMorph(cacheString));
                            }
                        }
                        // 未越界, 添加到缓存行
                        else
                        {
                            cacheSnippets.Add(snippet);
                            cacheWidth += width;
                        }
                    }
                    // 非简单片段
                    // 如: [centeritem/stack:type]
                    else
                    {
                        float width = snippet.GetStringLength(font);

                        // 特殊 Snippet 超过宽度限制
                        if (cacheWidth + width > maxWidth)
                        {
                            // 之前的行添加到最终列表
                            finalSnippets.Add(cacheSnippets);
                            cacheSnippets = [snippet];
                            cacheWidth = width;
                        }
                        // 未越界, 添加到缓存行
                        else
                        {
                            cacheSnippets.Add(snippet);
                            cacheWidth += width;
                        }
                    }
                }

                // 遍历完, 如果有, 剩下的 cacheSnippets 加入到 finalSnippets
                if (cacheSnippets.Count > 0)
                {
                    finalSnippets.Add(cacheSnippets);
                    cacheSnippets = [];
                }
            }
        }
        #endregion

        return finalSnippets;
    }
    */
}