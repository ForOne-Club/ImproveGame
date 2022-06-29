using Terraria;
using Terraria.UI;

namespace ImproveGame.Common.ModHooks
{
    public interface IItemOverrideHover
    {
        /// <summary>
        /// 与我的提交一样，可以修改<see cref="Main.cursorOverride"/>
        /// </summary>
        /// <param name="inventory">物品所在的物品数组</param>
        /// <param name="context">物品槽标识(<see cref="ItemSlot.Context"/></param>
        /// <param name="slot">所在的槽的索引</param>
        /// <returns>是否禁用原版，true为禁用，false不禁用</returns>
        bool OverrideHover(Item[] inventory, int context, int slot);
    }
}
