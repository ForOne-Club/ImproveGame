using Terraria.ModLoader.IO;

namespace ImproveGame.UI.ItemContainer;

public interface IItemContainer
{
    string Name { get; }

    List<Item> ItemContainer { get; }
    bool AutoStorage { get; set; }
    bool AutoSort { get; set; }

    void SortContainer();

    /// <summary>
    /// 将物品放入容器中
    /// </summary>
    void ItemIntoContainer(Item item);

    /// <summary>
    /// 符合进入标准
    /// </summary>
    bool MeetEntryCriteria(Item item);

    static void SaveData(TagCompound tag, IItemContainer container)
    {
        tag["autoStorage"] = container.AutoStorage;
        tag["autoSort"] = container.AutoSort;
    }

    static void LoadData(TagCompound tag, IItemContainer container)
    {
        if (tag.TryGet("autoStorage", out bool autoStorage))
        {
            container.AutoStorage = autoStorage;
        }

        if (tag.TryGet("autoSort", out bool autoSort))
        {
            container.AutoSort = autoSort;
        }
    }
}
