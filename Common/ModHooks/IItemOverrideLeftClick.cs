namespace ImproveGame.Common.ModHooks
{
    public interface IItemOverrideLeftClick
    {
        /// <summary>
        /// 当物品被左键时执行
        /// </summary>
        /// <param name="inventory">物品所在的物品数组</param>
        /// <param name="context">物品槽标识(<see cref="ItemSlot.Context"/></param>
        /// <param name="slot">所在的槽的索引</param>
        /// <returns>是否禁用原版，true为禁用，false不禁用</returns>
        bool OverrideLeftClick(Item[] inventory, int context, int slot);
    }
}
