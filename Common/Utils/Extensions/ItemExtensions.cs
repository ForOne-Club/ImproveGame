using FuzzySearchNet;
using ImproveGame.Core;
using ImproveGame.UIFramework.Common;
using PinyinNet;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI;
using Terraria.ModLoader.Default;

namespace ImproveGame.Common.Utils.Extensions;

/// <summary>
/// <see cref="Item"/> 拓展
/// </summary>
public static class ItemExtensions
{
    #region 搜索 - Searching

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

    #endregion

    #region 集合操作 - Collection Operations

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
            return false;

        return (from i in items where i is not null && i.stack > 0 select i).Any(target => target.type == self.type);
    }

    #endregion

    #region 堆叠 - Stacking

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
            if (target.IsAir)
                return true;

            if (ItemLoader.CanStack(source, target) && target.type == source.type && target.stack < target.maxStack)
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

    #endregion

    #region 设置物品默认参数 - Base Values Setting

    /// <summary>
    /// 设置物品默认参数
    /// </summary>
    /// <param name="item"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="rare">推荐使用 <see cref="ItemRarityID"/> 设置稀有度</param>
    /// <param name="value">可以使用 <see cref="Item.buyPrice(int, int, int, int)"/> 或 <see cref="Item.sellPrice(int, int, int, int)"/> 设置价格</param>
    /// <param name="maxStack"></param>
    /// <param name="consumable"></param>
    public static void SetBaseValues(this Item item, int width, int height, int rare = 0, int value = 0,
        int maxStack = 1, bool consumable = false)
    {
        item.width = width;
        item.height = height;
        item.rare = rare;
        item.value = value;
        item.maxStack = maxStack;
        item.consumable = consumable;
    }

    /// <summary>
    /// 设置挥舞物品的默认参数
    /// </summary>
    /// <param name="item"></param>
    /// <param name="useStyle">推荐使用 <see cref="ItemUseStyleID"/> 设置使用类型</param>
    /// <param name="soundStyle">推荐使用 <see cref="SoundStyle"/> 设置使用音效</param>
    /// <param name="useAnimation"></param>
    /// <param name="useTime"></param>
    /// <param name="autoReuse"></param>
    /// <param name="mana"></param>
    public static void SetUseValues(this Item item, int useStyle, SoundStyle soundStyle, int useAnimation, int useTime,
        bool autoReuse = false, int mana = 0)
    {
        item.useStyle = useStyle;
        item.UseSound = soundStyle;
        item.useAnimation = useAnimation;
        item.useTime = useTime;
        item.autoReuse = autoReuse;
        item.mana = mana;
    }

    /// <summary>
    /// 设置武器物品的默认参数
    /// </summary>
    /// <param name="item"></param>
    /// <param name="damage"></param>
    /// <param name="damageType">推荐使用 <see cref="DamageClass"/> 设置伤害类型</param>
    /// <param name="knockBack"></param>
    /// <param name="noMelee"></param>
    public static void SetWeaponValues(this Item item, int damage, DamageClass damageType, float knockBack = 0f,
        bool noMelee = false)
    {
        item.damage = damage;
        item.DamageType = damageType;
        item.knockBack = knockBack;
        item.noMelee = noMelee;
    }

    /// <summary>
    /// 弹幕相关属性设置
    /// </summary>
    /// <param name="item"></param>
    /// <param name="shoot"></param>
    /// <param name="shootSpeed"></param>
    /// <param name="useAmmo">推荐使用 <see cref="AmmoID"/> 设置使用的弹药类型</param>
    /// <param name="ammo">推荐使用 <see cref="AmmoID"/> 设置弹药归属</param>
    public static void SetShootValues(this Item item, int shoot, float shootSpeed, int useAmmo = 0, int ammo = 0)
    {
        item.shoot = shoot;
        item.shootSpeed = shootSpeed;
        item.useAmmo = useAmmo;
        item.ammo = ammo;
    }

    #endregion

    #region 筛选 - Filtering

    public static bool IsHook(this Item item) => Main.projHook.IndexInRange(item.shoot) && Main.projHook[item.shoot];

    public static bool IsTool(this Item item) => IsOrdinaryTool(item) || item.IsHook() || item.fishingPole > 0 ||
                                                 IsWiringTool(item) || IsOtherTool(item);

    public static bool IsAccessory(this Item item) => item.accessory;

    public static bool IsAmmo(this Item item) => item.ammo != AmmoID.None && !item.notAmmo;

    public static bool IsArmor(this Item item) => item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0;

    public static bool IsPlaceable(this Item item) => item.createTile >= TileID.Dirt || item.createWall > WallID.None;

    public static bool IsWeapon(this Item item) =>
        item.damage > 0 && item.axe is 0 && item.hammer is 0 && item.pick is 0;

    public static ContentSamples.CreativeHelper.ItemGroup GetCreativeItemGroup(this Item item) =>
        ContentSamples.CreativeHelper.GetItemGroup(item, out _);

    public static bool IsOrdinaryTool(this Item item) => item.axe != 0 || item.hammer != 0 || item.pick != 0;

    public static bool IsOtherTool(this Item item) => ItemID.Sets.DuplicationMenuToolsFilter[item.type];

    public static bool IsWiringTool(this Item item) =>
        GetCreativeItemGroup(item) is ContentSamples.CreativeHelper.ItemGroup.Wiring;

    public static bool IsHerb(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.AlchemyPlants
        or ContentSamples.CreativeHelper.ItemGroup.AlchemySeeds;

    public static bool IsSummonItem(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.EventItem
        or ContentSamples.CreativeHelper.ItemGroup.BossItem;

    public static bool IsPet(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.VanityPet
        or ContentSamples.CreativeHelper.ItemGroup.LightPet;

    public static bool IsMount(this Item item) => item.GetCreativeItemGroup()
        is ContentSamples.CreativeHelper.ItemGroup.Mount;

    public static bool IsMaterial(this Item item) => item.material;

    public static bool IsBankItem(this Item item) => IsBankItem(item.type);

    public static bool IsBankItem(int type) => Lookups.Bank2Items.Contains(type) || Lookups.Bank3Items.Contains(type) ||
                                               Lookups.Bank4Items.Contains(type) || Lookups.Bank5Items.Contains(type);

    #endregion

    #region 其他 - Misc

    public static bool IsSameItem(Item item1, Item item2)
    {
        // 卸载物品是特殊的
        if (item1.ModItem is UnloadedItem u1 && item2.ModItem is UnloadedItem u2)
        {
            if (u1.ItemName == u2.ItemName && u1.ModName == u2.ModName)
                return true;
            return false;
        }

        // 正常物品
        return item1.type == item2.type;
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

    public static bool IsAvailableRedPotionExtension(this Item item)
    {
        bool isFtw = Config.RedPotionEverywhere || Main.getGoodWorld;
        return item.type is ItemID.RedPotion && Config.InfiniteRedPotion && isFtw &&
               item.stack >= Config.RedPotionRequirement && Config.NoConsume_Potion;
    }

    public static void DrawIcon(this Item item, SpriteBatch sb, Color lightColor, Vector2 center,
        float maxSize = 32f, float itemScale = 1f)
    {
        Main.instance.LoadItem(item.type);
        Texture2D texture2D = TextureAssets.Item[item.type].Value;
        Rectangle frame = Main.itemAnimations[item.type] is null
            ? texture2D.Frame()
            : Main.itemAnimations[item.type].GetFrame(texture2D);
        itemScale *= frame.Width > maxSize || frame.Height > maxSize
            ? frame.Width > frame.Height ? maxSize / frame.Width : maxSize / frame.Height
            : 1f;
        Vector2 origin = frame.Size() / 2f;
        if (ItemLoader.PreDrawInInventory(item, sb, center, frame, item.GetAlpha(lightColor),
                item.GetColor(lightColor), origin, itemScale))
        {
            sb.Draw(texture2D, center, frame, item.GetAlpha(lightColor), 0f, origin, itemScale,
                SpriteEffects.None, 0f);
            if (item.color != Color.Transparent)
                sb.Draw(texture2D, center, frame, item.GetColor(lightColor), 0f, origin, itemScale,
                    SpriteEffects.None, 0f);
        }

        ItemLoader.PostDrawInInventory(item, sb, center, frame, item.GetAlpha(lightColor),
            item.GetColor(lightColor), origin, itemScale);

        if (ItemID.Sets.TrapSigned[item.type])
            Main.spriteBatch.Draw(TextureAssets.Wire.Value, center + new Vector2(14f) * itemScale,
                new Rectangle(4, 58, 8, 8), lightColor, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

        if (ItemID.Sets.DrawUnsafeIndicator[item.type])
        {
            Vector2 vector2 = new Vector2(-4f, -4f) * itemScale;
            Texture2D value7 = TextureAssets.Extra[258].Value;
            Rectangle rectangle2 = value7.Frame();
            Main.spriteBatch.Draw(value7, center + vector2 + new Vector2(14f) * itemScale, rectangle2, lightColor, 0f,
                rectangle2.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        if (item.type is ItemID.RubblemakerSmall or ItemID.RubblemakerMedium or ItemID.RubblemakerLarge)
        {
            Vector2 vector3 = new Vector2(2f, -6f) * itemScale;
            switch (item.type)
            {
                case 5324:
                    {
                        Texture2D value10 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle5 = value10.Frame(3, 1, 2);
                        Main.spriteBatch.Draw(value10, center + vector3 + new Vector2(16f) * itemScale, rectangle5,
                            lightColor, 0f, rectangle5.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case 5329:
                    {
                        Texture2D value9 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle4 = value9.Frame(3, 1, 1);
                        Main.spriteBatch.Draw(value9, center + vector3 + new Vector2(16f) * itemScale, rectangle4,
                            lightColor, 0f, rectangle4.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case 5330:
                    {
                        Texture2D value8 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle3 = value8.Frame(3);
                        Main.spriteBatch.Draw(value8, center + vector3 + new Vector2(16f) * itemScale, rectangle3,
                            lightColor, 0f, rectangle3.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
            }
        }
    }

    #endregion
}