using ImproveGame.UIFramework.UIElements;
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
        /// <param name="emptyText">没物品时悬停在上面显示的文本</param>
        /// <param name="parent">该元件的父元件</param>
        /// <param name="folderName">贴图资源在Assets/Images/UI文件夹里面的子文件夹名称</param>
        /// <returns>一个<see cref="ModItemSlot"/>实例</returns>
        public static ModItemSlot CreateItemSlot(float x, float y, string iconTextureName = null, float scale = 0.85f, Func<Item, Item, bool> canPlace = null, Action<Item, bool> onItemChanged = null, Func<string> emptyText = null, UIElement parent = null, string folderName = null)
        {
            string path = null;
            if (iconTextureName is not null)
            {
                path = $"ImproveGame/Assets/Images/UI/{iconTextureName}";
                if (folderName is not null)
                    path = $"ImproveGame/Assets/Images/UI/{folderName}/{iconTextureName}";
            }
            ModItemSlot slot = new(scale, path, emptyText);
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

        public static float GetWidestUI(params UIElement[] uies)
        {
            float width = 0;
            foreach (var uie in uies)
            {
                if (uie.Width() > width)
                    width = uie.Width();
            }
            return width;
        }

        public static void RecalculateS(params UIElement[] uies)
        {
            foreach (var uie in uies)
            {
                uie.Recalculate();
            }
        }
    }
}
