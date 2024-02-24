using ImproveGame.UI.ExtremeStorage.Filters;

namespace ImproveGame.UI.ExtremeStorage;

public static class StorageHelper
{
    public static ItemGroup RedirectChestToGroup(this Chest chest)
    {
        switch (chest.name[0])
        {
            case '!':
            case '@':
            case '#':
            case '$':
            case '%':
            case '=':
            case '*':
            case '-':
                return ItemGroup.Everything;
            case '&':
                return ItemGroup.Furniture;
            case '+':
                return ItemGroup.Alchemy;
        }

        // 不合规的
        return ItemGroup.Setting;
    }
    
    /// <summary>
    /// 除了Everything, Furniture, Alchemy，其他的Group都是筛选组，不是实际的组，要重定向
    /// </summary>
    public static ItemGroup RedirectGroupToCategory(this ItemGroup group) =>
        group switch
        {
            ItemGroup.Everything => ItemGroup.Everything,
            ItemGroup.Weapon => ItemGroup.Everything,
            ItemGroup.Tool => ItemGroup.Everything,
            ItemGroup.Ammo => ItemGroup.Everything,
            ItemGroup.Armor => ItemGroup.Everything,
            ItemGroup.Accessory => ItemGroup.Everything,
            ItemGroup.Block => ItemGroup.Everything,
            ItemGroup.Misc => ItemGroup.Everything,
            ItemGroup.Furniture => ItemGroup.Furniture,
            ItemGroup.Alchemy => ItemGroup.Alchemy,
            // 不可能
            ItemGroup.Setting => ItemGroup.Setting,
            _ => ItemGroup.Setting
        };

    public static char GetIdentifier(this ItemGroup group)
    {
        group = RedirectGroupToCategory(group);
        return group switch
        {
            ItemGroup.Everything => '!',
            ItemGroup.Furniture => '&',
            ItemGroup.Alchemy => '+',
            _ => '!'
        };
    }

    /// <summary>
    /// 从筛选器检测物品是否符合
    /// </summary>
    public static bool CheckFromFilter(Item item, ItemGroup group)
    {
        return !Filter.Filters.TryGetValue(group, out var filter) || filter.Check(item);
    }
}