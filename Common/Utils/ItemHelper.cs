namespace ImproveGame.Common.Utils;

public static class ItemHelper
{
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
    public static void SetBaseValues(this Item item, int width, int height, int rare = 0, int value = 0, int maxStack = 1, bool consumable = false)
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
    public static void SetUseValues(this Item item, int useStyle, SoundStyle soundStyle, int useAnimation, int useTime, bool autoReuse = false, int mana = 0)
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
    public static void SetWeaponValues(this Item item, int damage, DamageClass damageType, float knockBack = 0f, bool noMelee = false)
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
                Main.spriteBatch.Draw(TextureAssets.Wire.Value, center + new Vector2(14f) * itemScale, new Rectangle(4, 58, 8, 8), lightColor, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

            if (ItemID.Sets.DrawUnsafeIndicator[item.type]) {
                Vector2 vector2 = new Vector2(-4f, -4f) * itemScale;
                Texture2D value7 = TextureAssets.Extra[258].Value;
                Rectangle rectangle2 = value7.Frame();
                Main.spriteBatch.Draw(value7, center + vector2 + new Vector2(14f) * itemScale, rectangle2, lightColor, 0f, rectangle2.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            if (item.type is ItemID.RubblemakerSmall or ItemID.RubblemakerMedium or ItemID.RubblemakerLarge) {
                Vector2 vector3 = new Vector2(2f, -6f) * itemScale;
                switch (item.type) {
                    case 5324: {
                            Texture2D value10 = TextureAssets.Extra[257].Value;
                            Rectangle rectangle5 = value10.Frame(3, 1, 2);
                            Main.spriteBatch.Draw(value10, center + vector3 + new Vector2(16f) * itemScale, rectangle5, lightColor, 0f, rectangle5.Size() / 2f, 1f, SpriteEffects.None, 0f);
                            break;
                        }
                    case 5329: {
                            Texture2D value9 = TextureAssets.Extra[257].Value;
                            Rectangle rectangle4 = value9.Frame(3, 1, 1);
                            Main.spriteBatch.Draw(value9, center + vector3 + new Vector2(16f) * itemScale, rectangle4, lightColor, 0f, rectangle4.Size() / 2f, 1f, SpriteEffects.None, 0f);
                            break;
                        }
                    case 5330: {
                            Texture2D value8 = TextureAssets.Extra[257].Value;
                            Rectangle rectangle3 = value8.Frame(3);
                            Main.spriteBatch.Draw(value8, center + vector3 + new Vector2(16f) * itemScale, rectangle3, lightColor, 0f, rectangle3.Size() / 2f, 1f, SpriteEffects.None, 0f);
                            break;
                        }
                }
            }
    }
}
