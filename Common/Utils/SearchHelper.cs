using PinyinNet;

namespace ImproveGame;

partial class MyUtils
{
    public class SearchResult
    {
        public int OriginalIndex { get; set; }
        public string OriginalText { get; set; }
        // 中文文本的拼音
        public string Pinyin { get; set; }
        // 英文处理后文本（小写、无空格）
        public string ProcessedEnglish { get; set; }
        // 原始文本是否包含关键词子串
        public bool ContainsOriginalSubstring { get; set; }
        // 拼音是否包含拼音关键词子串
        public bool ContainsPinyinSubstring { get; set; }
        // Levenshtein 距离
        public int LevenshteinDistance { get; set; }
        // 拼音匹配的 Levenshtein 距离
        public int LevenshteinDistancePinyin { get; set; }
        // 综合评分
        public int MatchScore { get; set; }

        public bool HasAnyMatch => ContainsOriginalSubstring || ContainsPinyinSubstring;
    }

    /// <summary>
    /// 基于 Levenshtein 距离的文本搜索，支持英文（不分大小写，无视空格）和拼音搜索 <br/>
    /// 感谢 DeepSeek 对其的大力支持（是的，代码全是 DeepSeek 写的）
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="texts"></param>
    public static List<SearchResult> TextSearch(string keyword, List<string> texts, bool outputResult = false)
    {
        // 预处理所有文本：生成拼音、处理后的英文版本
        var processedTexts = texts
            .Select((text, index) => new SearchResult
            {
                OriginalIndex = index,
                OriginalText = text,
                Pinyin = ProcessEnglish(PinyinConvert.GetPinyinForAutoComplete(text)), // 中文转拼音
                ProcessedEnglish = ProcessEnglish(text),               // 处理英文（小写、去空格）
            })
            .ToList();

        // 标准化搜索关键词
        keyword = ProcessEnglish(keyword);
        // 获取关键词对应拼音内容
        string keywordPinyin = ProcessEnglish(PinyinConvert.GetPinyinForAutoComplete(keyword));
        
        // 计算匹配度
        foreach (var item in processedTexts)
        {
            // 检查原始文本是否包含关键词（不区分大小写）
            item.ContainsOriginalSubstring = item.ProcessedEnglish
                .Contains(keyword, StringComparison.OrdinalIgnoreCase);

            // 检查原始文本拼音是否包含拼音关键词（不区分大小写）
            item.ContainsPinyinSubstring = item.Pinyin
                .Contains(keywordPinyin, StringComparison.OrdinalIgnoreCase);

            // 计算 Levenshtein 距离
            item.LevenshteinDistance = CalculateLevenshteinDistance(
                keyword,
                item.ProcessedEnglish
            );

            // 计算拼音下的 Levenshtein 距离
            item.LevenshteinDistancePinyin = CalculateLevenshteinDistance(
                keyword,
                item.Pinyin
            );
            
            // 综合评分公式
            int score = 0;
            if (item.ContainsOriginalSubstring) score += 120;
            if (item.ContainsPinyinSubstring) score += 100;
            // 距离越小，得分越高，原始文本距离比重比拼音大，计算最终得分
            item.MatchScore = score - item.LevenshteinDistance * 5 - item.LevenshteinDistancePinyin;
        }

        // 排序规则：
        // 1. 综合评分 → 优先
        // 2. 原始索引小 → 稳定性
        var results = processedTexts
            .OrderByDescending(x => x.MatchScore)
            // .ThenBy(x => x.OriginalIndex)
            .ToList();

        // 输出排序后的索引和文本
        if (outputResult)
        {
            Console.WriteLine("\n搜索结果索引（按优先级排序）:");
            foreach (var result in results)
            {
                Console.WriteLine($"Index: {result.OriginalIndex}, Text: {result.OriginalText}");
            }
        }

        return results;
    }

    /// <summary>
    /// 处理英文：移除空格并转为小写
    /// </summary>
    public static string ProcessEnglish(string text)
    {
        return text.Replace(" ", "").ToLowerInvariant();
    }

    /// <summary>
    /// Levenshtein 距离计算
    /// </summary>
    static int CalculateLevenshteinDistance(string s, string t)
    {
        int[,] dp = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost
                );
            }
        }

        return dp[s.Length, t.Length];
    }
}