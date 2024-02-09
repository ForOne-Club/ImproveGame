using ImproveGame.UIFramework.UIElements;

namespace ImproveGame.UI.ExtremeStorage
{
    /// <summary>
    /// 与 <see cref="ItemSlot"/> 配套
    /// 在 ItemSlotList 中 items 变量无用
    /// </summary>
    public class ItemSlotList : ModItemList
    {
        public void SetInventory(List<ChestItemsInfo> targetedItems, int slotSize = 52, float spacing = 10f)
        {
            RemoveAllChildren();

            // 计算物品总数
            int itemCount = 0;
            
            foreach (var info in targetedItems)
            {
                foreach (int itemIndex in info.DisplayedItemsIndexes)
                {
                    itemCount++;
                    var itemSlot = new ItemSlot(info.ChestItemsArray, itemIndex, info.ChestIndex)
                    {
                        Spacing = new Vector2(spacing)
                    }.SetSize(slotSize, slotSize);
                    Append(itemSlot);
                }
            }
            
            // 计算显示的列数
            int itemColumn = (int)Math.Ceiling(itemCount / 10f);

            ModifyHVCount(10, itemColumn, slotSize, spacing);
        }
    }
}