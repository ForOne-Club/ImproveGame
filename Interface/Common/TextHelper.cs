global using ImproveGame.Interface.Common;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.Common;

public static class TextHelper
{
    public static List<List<TextSnippet>> WordwrapStringSmart(string text, Color c, DynamicSpriteFont font, int maxWidth, int maxLines)
    {
        var list = new List<List<TextSnippet>>();
        var list2 = new List<TextSnippet>();

        TextSnippet[] snippets = [.. ChatManager.ParseMessage(text, c)];
        foreach (TextSnippet textSnippet in snippets)
        {
            string[] array3 = textSnippet.Text.Split('\n');
            for (int j = 0; j < array3.Length - 1; j++)
            {
                list2.Add(textSnippet.CopyMorph(array3[j]));
                list.Add(list2);
                list2 = new List<TextSnippet>();
            }

            list2.Add(textSnippet.CopyMorph(array3[^1]));
        }

        list.Add(list2);
        if (maxWidth != -1)
        {
            for (int k = 0; k < list.Count; k++)
            {
                List<TextSnippet> list3 = list[k];
                float num = 0f;
                for (int l = 0; l < list3.Count; l++)
                {
                    float stringLength = list3[l].GetStringLength(font);
                    if (stringLength + num > (float)maxWidth)
                    {
                        int num2 = maxWidth - (int)num;
                        if (num > 0f)
                        {
                            num2 -= 16;
                        }

                        int num3 = Math.Min(list3[l].Text.Length, num2 / 8);
                        for (int m = 0; m < list3[l].Text.Length; m++)
                        {
                            if (font.MeasureString(list3[l].Text.Substring(0, m)).X * list3[l].Scale < (float)num2)
                            {
                                num3 = m;
                            }
                        }

                        if (num3 < 0)
                        {
                            num3 = 0;
                        }

                        string[] array4 = list3[l].Text.Split(' ');
                        int num4 = num3;
                        if (array4.Length > 1)
                        {
                            num4 = 0;
                            for (int n = 0; n < array4.Length; n++)
                            {
                                bool flag = num4 == 0;
                                if (!(num4 + array4[n].Length <= num3 || flag))
                                {
                                    break;
                                }

                                num4 += array4[n].Length + 1;
                            }

                            if (num4 > num3)
                            {
                                num4 = num3;
                            }
                        }

                        string newText = list3[l].Text.Substring(0, num4);
                        string newText2 = list3[l].Text.Substring(num4);
                        list2 = new List<TextSnippet> { list3[l].CopyMorph(newText2) };
                        for (int num5 = l + 1; num5 < list3.Count; num5++)
                        {
                            list2.Add(list3[num5]);
                        }

                        list3[l] = list3[l].CopyMorph(newText);
                        list[k] = list[k].Take(l + 1).ToList();
                        list.Insert(k + 1, list2);
                        break;
                    }

                    num += stringLength;
                }

                num = 0f;
            }
        }

        if (maxLines != -1)
        {
            while (list.Count > maxLines)
            {
                list.RemoveAt(maxLines);
            }
        }

        return list;
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
