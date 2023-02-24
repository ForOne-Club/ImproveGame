namespace ImproveGame.Common.Utils.Extensions;

internal static class ItemExtensions
{
    public static bool HasAnyOneItem(this Item[] self, params int[] types)
    {
        if (self is null)
        {
            return false;
        }

        foreach (Item item in self)
        {
            if (item is null || item.IsAir)
            {
                continue;
            }

            foreach (int type in types)
            {
                if (item.type == type)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool InArray(this Item self, Item[] items)
    {
        if (items is null)
        {
            return false;
        }

        foreach (var target in items)
        {
            if (target is null)
            {
                continue;
            }

            if (!target.IsAir && target.type == self.type)
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
    public static bool CanStackToArray(this Item self, Item[] items)
    {
        if (items is null)
        {
            return false;
        }

        foreach (Item target in items)
        {
            if (target is null || target.IsAir)
            {
                return true;
            }

            if (target.type == self.type && target.stack < target.maxStack && ItemLoader.CanStack(self, target))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 集合内拥有此物品，并且此物品可以堆叠进去
    /// </summary>
    /// <returns>返回 <see langword="true"/> 同时代表通过 <see cref="ItemLoader.CanStack(Item, Item)"/></returns>
    /*public static bool HasAndCanStackToArray(this Item self, Item[] items)
    {
        bool inArray = false;

        if (items is null)
        {
            return false;
        }

        foreach (Item target in items)
        {
            if (target is null)
            {
                continue;
            }

            if (!inArray && !target.IsAir && target.type == self.type)
            {
                inArray = true;
            }

            if (inArray)
            {
                if (target.IsAir)
                {
                    return true;
                }

                if (target.type == self.type && target.stack < target.maxStack && ItemLoader.CanStack(self, target))
                {
                    return true;
                }
            }


        }
        return false;
    }*/
}
