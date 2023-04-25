using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Players;

public class AutoTrashPlayer : ModPlayer
{
    #region 设置 Instance
    public static AutoTrashPlayer Instance;

    public override void OnEnterWorld(Player player)
    {
        Instance = this;
    }

    public override void Unload()
    {
        Instance = null;
    }
    #endregion

    /// <summary>
    /// 自动被丢弃的物品的 type
    /// </summary>
    private readonly List<int> ItemTypes = new List<int>();

    /// <summary>
    /// 近期被丢弃的 <see cref="MaxCapacity"/> 件物品
    /// </summary>
    public readonly List<Item> LastItems = new List<Item>();

    /// <summary>
    /// 启用自动垃圾桶功能
    /// </summary>
    public bool Enabled = false;

    /// <summary>
    /// 垃圾桶最大容量，当然我现在还不会写设置调这个
    /// </summary>
    public int MaxCapacity = 5;

    public AutoTrashPlayer()
    {
        LastItems = new List<Item>(MaxCapacity);
    }

    public bool ContainsAutoTrash(int type)
    {
        return ItemTypes.Contains(type);
    }

    /// <summary>
    /// 加入到自动垃圾桶的行列中
    /// </summary>
    /// <param name="type"></param>
    public void AddToAutoTrash(int type)
    {
        if (!ItemTypes.Contains(type))
        {
            ItemTypes.Add(type);
        }
    }

    /// <summary>
    /// 从自动回收站删除项目
    /// </summary>
    /// <param name="type"></param>
    public void RemoveItemFromAutoTrash(int type)
    {
        ItemTypes.Remove(type);
    }

    /// <summary>
    /// 判断 <see cref="LastItems"/> 有没有这个 type 的物品，只判断前 <see cref="MaxCapacity"/> 位
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool LastItemHasItem(int itemType, out int index)
    {
        index = -1;

        float cycleCount = Math.Min(MaxCapacity, LastItems.Count);

        for (int i = 0; i < cycleCount; i++)
        {
            if (LastItems[i].type == itemType)
            {
                index = i;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 添加物品到垃圾桶
    /// </summary>
    /// <param name="item"></param>
    public void RmoveItemFromLastItem(Item item)
    {
        if (LastItemHasItem(item.type, out int index))
        {
            LastItems[index] = new Item();
            RemoveExcessItems();
        }
    }

    /// <summary>
    /// 添加物品到垃圾桶
    /// </summary>
    /// <param name="item"></param>
    public void AddToLastItem(Item item)
    {
        if (LastItemHasItem(item.type, out int index))
        {
            Item lastItem = LastItems[index];
            lastItem.stack = Math.Min(lastItem.maxStack, lastItem.stack + item.stack);
            if (index != 0)
            {
                LastItems.RemoveAt(index);
                LastItems.Insert(0, lastItem);
            }
        }
        else
        {
            LastItems.Insert(0, item);
        }

        RemoveExcessItems();
    }

    /// <summary>
    /// 清除多余物品
    /// </summary>
    public void RemoveExcessItems()
    {
        for (int i = 0; i < LastItems.Count; i++)
        {
            if (LastItems is null || LastItems[i].IsAir)
            {
                LastItems.RemoveAt(i--);
            }
        }

        if (LastItems.Count > MaxCapacity)
        {
            LastItems.RemoveRange(MaxCapacity, LastItems.Count - MaxCapacity);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag["ItemTypes"] = ItemTypes;
        tag["LastItems"] = LastItems;
        tag["Enabled"] = Enabled;
        tag["MaxCapacity"] = 8; // MaxCapacity;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("ItemTypes", out List<int> itemTypes))
        {
            foreach (var itemType in itemTypes)
            {
                ItemTypes.Add(itemType);
            }
        }

        if (tag.TryGet("LastItems", out List<Item> lastItems))
        {
            for (int i = 0; i < lastItems.Count; i++)
            {
                LastItems.Add(lastItems[i]);
            }
        }

        tag.TryGet("Enabled", out Enabled);
        tag.TryGet("MaxCapacity", out MaxCapacity);
        MaxCapacity = 8;
    }
}
