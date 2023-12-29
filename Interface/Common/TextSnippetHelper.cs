global using ImproveGame.Interface.Common;
using ReLogic.Graphics;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.Common;

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
            snippets.Add(new TextSnippet(input[inputIndex..], baseColor));
        }

        return snippets;
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
}
