namespace ImproveGame.Common.Utils.Extensions;

internal static class ItemExtensions
{
    public static bool InArray(this Item target, Item[] items)
    {
        foreach (var item in items)
        {
            if (!item.IsAir && item.type == target.type)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 能否堆叠到指定物品集合
    /// </summary>
    /// <returns>返回 <see langword="true"/> 代表存在于集合，并且通过 <see cref="ItemLoader.CanStack(Item, Item)"/></returns>
    public static bool CanStackToArray(this Item target, Item[] items)
    {
        if (items is null)
        {
            return false;
        }

        bool hasItem = false;
        foreach (Item item in items)
        {
            if (!hasItem && item is not null && !item.IsAir && item.type == target.type)
            {
                hasItem = true;
            }

            if (hasItem && item.stack < item.maxStack && ItemLoader.CanStack(target, item))
            {
                return true;
            }
        }
        return false;
    }
}
