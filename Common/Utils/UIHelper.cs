using ImproveGame.Interface.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace ImproveGame
{
    partial class MyUtils
    {
        /// <summary>
        /// 快捷做一个ItemSlot
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="iconTextureName">空物品时显示的贴图</param>
        /// <param name="canPlace">是否可以放入物品的判断</param>
        /// <param name="onItemChanged">物品更改时执行</param>
        /// <returns>一个<see cref="ModItemSlot"/>实例</returns>
        public static ModItemSlot CreateItemSlot(float x, float y, string iconTextureName, float scale = 0.85f, Func<Item, Item, bool> canPlace = null, Action<Item> onItemChanged = null, Func<string> emptyText = null, UIElement parent = null) {
            ModItemSlot slot = new(scale, $"ImproveGame/Assets/Images/UI/Icon_{iconTextureName}", emptyText);
            slot.Left.Set(x, 0f);
            slot.Top.Set(y, 0f);
            slot.Width.Set(46f, 0f);
            slot.Height.Set(46f, 0f);
            if (canPlace is not null)
                slot.OnCanPlaceItem += canPlace;
            if (onItemChanged is not null)
                slot.OnItemChange += onItemChanged;
            if (parent is not null)
                parent.Append(slot);
            return slot;
        }
    }
}
