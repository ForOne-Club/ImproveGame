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
}
