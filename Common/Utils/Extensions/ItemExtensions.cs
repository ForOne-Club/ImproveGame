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
    public static bool CanStackToArray(this Item self, Item[] items)
    {
        if (items is null)
        {
            return false;
        }

        foreach (Item target in items)
        {
            if (target is { IsAir: true } || (target.type == self.type && target.stack < target.maxStack))
            {
                if (ItemLoader.CanStack(self, target))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 堆叠到一个物品集合中 <br/>
    /// 注：self 不能直接赋值 items 使用下标可以直接赋值
    /// </summary>
    public static void StackToArray(this Item self, Item[] items)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is { IsAir: true } || item.type != self.type)
            {
                continue;
            }

            self.StackToSameItem(item);
            if (self.IsAir)
            {
                return;
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];

            if (item is null || item.IsAir)
            {
                items[i] = self.Clone();
                self.TurnToAir();
                return;
            }
        }
    }

    /// <summary>
    /// 堆叠到另一个物品 <br/>
    /// 注：self item 均不能直接赋值
    /// </summary>
    public static void StackToSameItem(this Item self, Item item)
    {
        if (item.stack >= item.maxStack && !ItemLoader.CanStack(self, item))
        {
            return;
        }

        int itemStack = item.stack;
        item.stack = Math.Min(item.stack + self.stack, item.maxStack);
        self.stack = Math.Max(self.stack - (item.stack - itemStack), 0);

        if (self.stack == 0)
        {
            self.TurnToAir();
        }
    }
}
