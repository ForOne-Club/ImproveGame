global using ImproveGame.Interface.Common;
using ReLogic.Graphics;
using System.Text.RegularExpressions;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.Common;

public static class TextUtils
{
    /// <summary>
    /// ChatManager.XXX() 解析消息 <br/>
    /// 根据设置的固定格式拆分消息: [tag/options:text]
    /// </summary>
    public static List<TextSnippet> ParseMessage(string input, Color baseColor)
    {
        // 删除文本中回车 (怎么会有回车捏?)
        input = input.Replace("\r", "");
        // 创建正则列表
        MatchCollection matchCollection = ChatManager.Regexes.Format.Matches(input);
        // 文字片段列表
        var snippets = new List<TextSnippet>();
        int textIndex = 0;
        foreach (Match match in matchCollection.Cast<Match>())
        {
            // 添加本次捕获与上次捕获之间的文本
            if (match.Index > textIndex)
            {
                snippets.Add(new TextSnippet(input[textIndex..match.Index], baseColor));
            }

            textIndex = match.Index + match.Length;

            string tag = match.Groups["tag"].Value;
            string text = match.Groups["text"].Value;
            string options = match.Groups["options"].Value;

            ITagHandler handler = ChatManager.GetHandler(tag);

            if (handler != null)
            {
                TextSnippet snippet = handler.Parse(text, baseColor, options);
                snippet.TextOriginal = match.ToString();
                snippets.Add(snippet);
            }
            else
            {
                snippets.Add(new TextSnippet(text, baseColor));
            }
        }

        // 添加最后一次捕获之后的文本
        if (input.Length > textIndex)
        {
            snippets.Add(new TextSnippet(input[textIndex..], baseColor));
        }

        return snippets;
    }

    public static List<List<TextSnippet>> WordwrapStringSmart2(string input, Color c, DynamicSpriteFont font, int maxWidth, int maxLines)
    {
        var snippets1 = new List<List<TextSnippet>>();  // 创建一个包含文本片段列表的列表
        var snippets2 = new List<TextSnippet>();  // 创建一个文本片段列表

        foreach (TextSnippet textSnippet in ParseMessage(input, c))  // 遍历文本片段数组
        {
            string[] array = textSnippet.Text.Split('\n');  // 根据换行符分割文本片段的内容

            for (int i = 0; i < array.Length - 1; i++)  // 遍历每个分割后的文本片段，除了最后一个
            {
                snippets2.Add(textSnippet.CopyMorph(array[i]));  // 将分割后的文本片段添加到列表 snippets2 中
                snippets1.Add(snippets2);  // 将 snippets2 添加到列表 snippets1 中
                snippets2 = [];  // 重置 snippets2 为一个空列表
            }

            snippets2.Add(textSnippet.CopyMorph(array[^1]));  // 将剩余的部分添加到 snippets2 中
        }

        snippets1.Add(snippets2);  // 将 snippets2 添加到列表 snippets1 中

        if (maxWidth != -1)  // 如果最大宽度不等于-1
        {
            for (int i = 0; i < snippets1.Count; i++)  // 遍历列表 snippets1 中的每个文本片段列表
            {
                float usedWidth = 0f;  // 用于跟踪已使用的宽度

                List<TextSnippet> snippets3 = snippets1[i];  // 获取当前文本片段列表
                for (int j = 0; j < snippets3.Count; j++)  // 遍历该列表中的每个文本片段
                {
                    float currentWidth = snippets3[j].GetStringLength(font);  // 获取当前文本片段的宽度

                    // 检查是否需要换行
                    if (currentWidth + usedWidth > maxWidth)
                    {
                        // 确定可以显示的文本长度
                        // ... (省略部分代码)

                        // 根据可显示的文本长度进行切分
                        // ... (省略部分代码)

                        // 将切分后的文本片段加入到列表中
                        // ... (省略部分代码)
                    }

                    usedWidth += currentWidth;  // 更新已使用宽度
                }
            }
        }

        if (maxLines != -1 && snippets1.Count > maxLines)  // 如果最大行数不等于-1 并且文本片段列表的数量大于最大行数
        {
            snippets1.RemoveRange(maxLines, snippets1.Count - maxLines);  // 则移除超出最大行数的部分
        }

        return snippets1;  // 返回最终的文本片段列表
    }

    /// <summary>
    /// 字符串智能换行
    /// </summary>
    public static List<List<TextSnippet>> WordwrapStringSmart(string input, Color c, DynamicSpriteFont font, int maxWidth, int maxLines)
    {
        var snippetsMain = new List<List<TextSnippet>>();
        var snippets = new List<TextSnippet>();

        // 对整个换行整理
        foreach (TextSnippet textSnippet in ParseMessage(input, c))
        {
            // 以换行分隔开
            var strings = textSnippet.Text.Split('\n');

            // 最后一行开头前分行
            for (int i = 0; i < strings.Length - 1; i++)
            {
                snippets.Add(textSnippet.CopyMorph(strings[i]));
                snippetsMain.Add(snippets);
                snippets = [];
            }

            // 最后一行开头
            snippets.Add(textSnippet.CopyMorph(strings[^1]));
        }

        snippetsMain.Add(snippets);

        // 根据宽度再次换行
        if (maxWidth != -1)
        {
            // 遍历分行后的
            for (int i = 0; i < snippetsMain.Count; i++)
            {
                // 已用宽度
                float usedWidth = 0f;

                // 遍历分行后列表
                List<TextSnippet> snippetsRow = snippetsMain[i];
                for (int j = 0; j < snippetsRow.Count; j++)
                {
                    var snippet = snippetsRow[j];

                    // 当前宽度
                    float currentWidth = snippet.GetStringLength(font);

                    // 当前宽度 + 已用宽度 > 宽度限制
                    if (currentWidth + usedWidth > maxWidth)
                    {
                        // 可用宽度
                        int availableWidth = maxWidth - (int)usedWidth;

                        // 已用宽度不为 > 0
                        if (usedWidth > 0f)
                        {
                            // 可用长度 - 16 ???
                            availableWidth -= 16;
                        }

                        // 可用下标
                        int availableIndex = 0;

                        // 可用宽度 > 0
                        if (availableWidth > 0)
                        {
                            availableIndex = Math.Min(snippet.Text.Length, availableWidth / 8);

                            for (int index3 = 0; index3 < snippet.Text.Length; index3++)
                            {
                                if (font.MeasureString(snippet.Text[..index3]).X * snippet.Scale > availableWidth)
                                {
                                    break;
                                }

                                availableIndex = index3;
                            }
                        }

                        // 根据空格拆分
                        string[] snippetArray = snippet.Text.Split(' ');

                        // 可用下标
                        int finalIndex = availableIndex;

                        if (snippetArray.Length > 1)
                        {
                            finalIndex = 0;

                            foreach (string text in snippetArray)
                            {
                                if (finalIndex + text.Length <= availableIndex || finalIndex == 0)
                                {
                                    finalIndex += text.Length + 1;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (finalIndex > availableIndex)
                            {
                                finalIndex = availableIndex;
                            }
                        }

                        string newText = snippet.Text[..finalIndex];
                        string newText2 = snippet.Text[finalIndex..];
                        snippets = [snippet.CopyMorph(newText2)];

                        for (int num5 = j + 1; num5 < snippetsRow.Count; num5++)
                        {
                            snippets.Add(snippetsRow[num5]);
                        }

                        snippetsRow[j] = snippet.CopyMorph(newText);
                        snippetsMain[i] = snippetsMain[i].Take(j + 1).ToList();
                        snippetsMain.Insert(i + 1, snippets);
                        break;
                    }

                    usedWidth += currentWidth;
                }
            }
        }

        // 删除超过行数限制后的文本
        if (maxLines != -1 && snippetsMain.Count > maxLines)
        {
            snippetsMain.RemoveRange(maxLines, snippetsMain.Count - maxLines);
        }

        return snippetsMain;
    }

    public static string[] WordwrapString(string text, DynamicSpriteFont font, int maxWidth, int maxLines, out int lineAmount)
    {
        string[] array = new string[maxLines];
        int num = 0;
        List<string> list = new List<string>(text.Split('\n'));
        List<string> list2 = new List<string>(list[0].Split(' '));
        for (int i = 1; i < list.Count && i < maxLines; i++)
        {
            list2.Add("\n");
            list2.AddRange(list[i].Split(' '));
        }

        bool flag = true;
        while (list2.Count > 0)
        {
            string text2 = list2[0];
            string text3 = " ";
            if (list2.Count == 1)
            {
                text3 = "";
            }

            if (text2 == "\n")
            {
                array[num++] += text2;
                flag = true;
                if (num >= maxLines)
                {
                    break;
                }

                list2.RemoveAt(0);
            }
            else if (flag)
            {
                if (font.MeasureString(text2).X > (float)maxWidth)
                {
                    string text4 = text2[0].ToString() ?? "";
                    int num2 = 1;
                    while (font.MeasureString(text4 + text2[num2] + "-").X <= (float)maxWidth)
                    {
                        text4 += text2[num2++];
                    }

                    text4 += "-";
                    array[num++] = text4 + " ";
                    if (num >= maxLines)
                    {
                        break;
                    }

                    list2.RemoveAt(0);
                    list2.Insert(0, text2.Substring(num2));
                }
                else
                {
                    ref string reference = ref array[num];
                    reference = reference + text2 + text3;
                    flag = false;
                    list2.RemoveAt(0);
                }
            }
            else if (font.MeasureString(array[num] + text2).X > (float)maxWidth)
            {
                num++;
                if (num >= maxLines)
                {
                    break;
                }

                flag = true;
            }
            else
            {
                ref string reference2 = ref array[num];
                reference2 = reference2 + text2 + text3;
                flag = false;
                list2.RemoveAt(0);
            }
        }

        lineAmount = num;
        if (lineAmount == maxLines)
        {
            lineAmount--;
        }

        return array;
    }
}
