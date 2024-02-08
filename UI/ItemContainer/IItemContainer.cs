using Terraria.ModLoader.IO;

namespace ImproveGame.UI.ItemContainer;

public interface IItemContainer
{
    List<Item> ItemContainer { get; }
    bool AutoStorage { get; set; }
    bool AutoSort { get; set; }

    void Sort();
    void PutInPackage(ref Item item);

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
