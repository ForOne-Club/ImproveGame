using FuzzySearchNet;
using ImproveGame.Interface.Common;
using PinyinNet;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI;

namespace ImproveGame.Common.Utils.Extensions;

/// <summary>
/// <see cref="Item"/> 拓展
/// </summary>
public static class ItemExtensions
{
    /// <summary>
    /// 搜索匹配，支持拼音
    /// </summary>
    public static bool AnyMatchWithString(this IEnumerable<Item> items, string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
            return false;

        string searchContent = RemoveSpaces(searchString.ToLower());
        return items.Any(item => MatchWithString(item, searchContent));
    }

    /// <summary>
    /// 搜索匹配，支持拼音
    /// </summary>
    public static bool MatchWithString(this Item item, string searchString, bool stringLowered = true)
    {
        if (string.IsNullOrEmpty(searchString))
            return false;

        string searchContent = stringLowered ? searchString : RemoveSpaces(searchString.ToLower());

        UIPlayerSetting setting = Main.LocalPlayer.GetModPlayer<UIPlayerSetting>();
        bool fuzzySearch = setting.FuzzySearch;
        bool tooltipSearch = setting.SearchTooltip;

        string currentLanguageName = RemoveSpaces(
                tooltipSearch
                    ? string.Concat(GetItemTooltipLines(item))
                    : Lang.GetItemNameValue(item.type))
            .ToLower();

        if (fuzzySearch)
        {
            if (FuzzySearch.Find(searchContent, currentLanguageName, 1).Any())
                return true;
        }

        if (currentLanguageName.Contains(searchContent))
            return true;

        if (Language.ActiveCulture.Name is not "zh-Hans") return false;

        string pinyin = RemoveSpaces(PinyinConvert.GetPinyinForAutoComplete(currentLanguageName));
        return fuzzySearch ? FuzzySearch.Find(searchContent, pinyin, 1).Any() : pinyin.Contains(searchContent);
    }

    /// <summary>
    /// 有其中一个
    /// </summary>
    public static bool HasOne(this Item[] array, params int[] types)
    {
        if (array is null)
        {
            return false;
        }

        foreach (Item item in array)
        {
            if (item is not null && !item.IsAir && types.Contains(item.type))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 有全部
    /// </summary>
    public static bool HasAll(this Item[] array, params int[] types)
    {
        if (array is null)
        {
            return false;
        }

        int count = 0;

        foreach (var type in types)
        {
            foreach (Item item in array)
            {
                if (item is not null && !item.IsAir && item.type == type)
                {
                    count++;
                    break;
                }
            }
        }

        return count == types.Length;
    }

    /// <summary>
    /// 数组里面有这个物品
    /// </summary>
    public static bool TheArrayHas(this Item self, Item[] items)
    {
        if (items is null)
        {
            return false;
        }

        foreach (var target in from i in items where i is not null && i.stack > 0 select i)
        {
            if (target.type == self.type)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 无需考虑集合内有没有此物品，能堆叠进去即可
    /// </summary>
    /// <returns></returns>
    public static bool CanStackToArray(this Item source, Item[] items)
    {
        if (items is null)
        {
            return false;
        }

        foreach (Item target in items)
        {
            if ((target.IsAir || (target.type == source.type && target.stack < target.maxStack)) &&
                ItemLoader.CanStack(source, target))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 堆叠到一个物品集合中
    /// </summary>
    public static void StackToArray(this Item source, Item[] items)
    {
        if (items is null)
        {
            return;
        }

        // source 来源
        // destination 目的地
        // 填补
        foreach (var destination in items)
        {
            if (destination is not null && !destination.IsAir && destination.type == source.type)
            {
                ItemLoader.TryStackItems(destination, source, out var _);

                if (source.IsAir)
                {
                    return;
                }
            }
        }

        // 创建
        for (int i = 0; i < items.Length; i++)
        {
            ref Item destination = ref items[i];

            if (destination is null || destination.IsAir)
            {
                destination = source.Clone();
                source.TurnToAir();
                return;
            }
        }
    }

    private static IEnumerable<string> GetItemTooltipLines(Item item)
    {
        Item hoverItem = item;
        int yoyoLogo = -1;
        int researchLine = -1;
        int rare = hoverItem.rare;
        float knockBack = hoverItem.knockBack;
        float num = 1f;
        if (hoverItem.CountsAsClass(DamageClass.Melee) && Main.LocalPlayer.kbGlove)
            num += 1f;

        if (Main.LocalPlayer.kbBuff)
            num += 0.5f;

        if (num != 1f)
            hoverItem.knockBack *= num;

        if (hoverItem.CountsAsClass(DamageClass.Ranged) && Main.LocalPlayer.shroomiteStealth)
            hoverItem.knockBack *= 1f + (1f - Main.LocalPlayer.stealth) * 0.5f;

        long num2 = 30;
        int numLines = 1;
        string[] array = new string[num2];
        bool[] array2 = new bool[num2];
        bool[] array3 = new bool[num2];
        for (int i = 0; i < num2; i++)
        {
            array2[i] = false;
            array3[i] = false;
        }

        string[] tooltipNames = new string[num2];

        Main.MouseText_DrawItemTooltip_GetLinesInfo(item, ref yoyoLogo, ref researchLine, knockBack, ref numLines,
            array, array2, array3, tooltipNames, out _);

        // Fix a bug where item knockback grows to infinity
        hoverItem.knockBack = knockBack;

        if (Main.npcShop > 0 && hoverItem.value >= 0 &&
            (hoverItem.type < ItemID.CopperCoin || hoverItem.type > ItemID.PlatinumCoin))
        {
            Main.LocalPlayer.GetItemExpectedPrice(hoverItem, out long calcForSelling, out long calcForBuying);

            long num5 = (hoverItem.isAShopItem || hoverItem.buyOnce) ? calcForBuying : calcForSelling;
            if (hoverItem.shopSpecialCurrency != -1)
            {
                tooltipNames[numLines] = "SpecialPrice";
                CustomCurrencyManager.GetPriceText(hoverItem.shopSpecialCurrency, array, ref numLines, num5);
            }
            else if (num5 > 0)
            {
                string text = "";
                long num6 = 0;
                long num7 = 0;
                long num8 = 0;
                long num9 = 0;
                long num10 = num5 * hoverItem.stack;
                if (!hoverItem.buy)
                {
                    num10 = num5 / 5;
                    if (num10 < 1)
                        num10 = 1;

                    long num11 = num10;
                    num10 *= hoverItem.stack;
                    int amount = Main.shopSellbackHelper.GetAmount(hoverItem);
                    if (amount > 0)
                        num10 += (-num11 + calcForBuying) * Math.Min(amount, hoverItem.stack);
                }

                if (num10 < 1)
                    num10 = 1;

                if (num10 >= 1000000)
                {
                    num6 = num10 / 1000000;
                    num10 -= num6 * 1000000;
                }

                if (num10 >= 10000)
                {
                    num7 = num10 / 10000;
                    num10 -= num7 * 10000;
                }

                if (num10 >= 100)
                {
                    num8 = num10 / 100;
                    num10 -= num8 * 100;
                }

                if (num10 >= 1)
                    num9 = num10;

                if (num6 > 0)
                    text = text + num6 + " " + Lang.inter[15].Value + " ";

                if (num7 > 0)
                    text = text + num7 + " " + Lang.inter[16].Value + " ";

                if (num8 > 0)
                    text = text + num8 + " " + Lang.inter[17].Value + " ";

                if (num9 > 0)
                    text = text + num9 + " " + Lang.inter[18].Value + " ";

                if (!hoverItem.buy)
                    array[numLines] = Lang.tip[49].Value + " " + text;
                else
                    array[numLines] = Lang.tip[50].Value + " " + text;

                tooltipNames[numLines] = "Price";
                numLines++;
            }
            else if (hoverItem.type != ItemID.DefenderMedal)
            {
                array[numLines] = Lang.tip[51].Value;
                tooltipNames[numLines] = "Price";
                numLines++;
            }
        }

        List<TooltipLine> lines = ItemLoader.ModifyTooltips(item, ref numLines, tooltipNames, ref array, ref array2,
            ref array3, ref yoyoLogo, out _, 0);

        return lines.Select(line => line.Text);
    }
}