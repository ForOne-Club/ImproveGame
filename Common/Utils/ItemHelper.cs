namespace ImproveGame.Common.Utils;

public static class ItemHelper
{
    /// <summary>
    /// 设置物品默认参数
    /// </summary>
    public static void SetBaseValue(this Item item, int width, int height, int rare, int value, int maxStack = 1, bool consumable = false)
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
    public static void SetUseValue(this Item item, int useStyle, SoundStyle soundStyle, int useAnimation, int useTime, bool autoReuse = false, int mana = 0)
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
    public static void SetWeaponValue(this Item item, int damage, DamageClass damageType, bool noMelee)
    {
        item.damage = damage;
        item.DamageType = damageType;
        item.noMelee = noMelee;
    }
}
