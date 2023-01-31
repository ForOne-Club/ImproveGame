namespace ImproveGame.Interface.ExtremeStorage;

public struct ChestItemsInfo
{
    public readonly Item[] ChestItemsArray;
    public readonly int ChestIndex;
    /// <summary> 箱内可被显示出来的物品的索引，用于搜索功能 </summary>
    public readonly IEnumerable<int> DisplayedItemsIndexes;

    public ChestItemsInfo(Item[] chestItemsArray, int chestIndex, IEnumerable<int> displayedItemsIndexes)
    {
        ChestItemsArray = chestItemsArray;
        ChestIndex = chestIndex;
        DisplayedItemsIndexes = displayedItemsIndexes;
    }
}