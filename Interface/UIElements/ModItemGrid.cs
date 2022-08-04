using ImproveGame.Interface.UIElements_Shader;

namespace ImproveGame.Interface.UIElements
{
    // 一个有滚动条的视图有三部分.
    // 一个最大的元素, 用来包裹显示内容部分和滚动条. 最大的元素负责隐藏超过范围的元素.
    public class ModItemGrid : UIElement
    {
        public readonly Vector2 SlotSize;

        public ModItemList ItemList;
        // public ModScrollbar Scrollbar;
        public ZeroScrollbar ZeroScrollbar;

        // 可以在 new 的时候将其他元素也初始化, 或者在执行 Active() 的时候初始化.
        public ModItemGrid()
        {
            SlotSize = new(52);
            SetPadding(0);
            OverflowHidden = true; // 隐藏超过显示范围的部分, 计算 padding 后的.

            ItemList = new(SlotSize);
            Append(ItemList);

            Append(ZeroScrollbar = new());
            ZeroScrollbar.Height.Pixels = ItemList.Height();
            ZeroScrollbar.HAlign = 1f;
            ZeroScrollbar.VAlign = 0.5f;

            Width.Pixels = ItemList.Width() + ZeroScrollbar.Width() + 10.1f;
            Height.Pixels = ItemList.Height() + 0.1f;
        }

        public void SetInventory(Item[] items)
        {
            ZeroScrollbar.SetView(Height.Pixels, SlotSize.Y * (items.Length / ModItemList.HCount) + 10f * (items.Length / ModItemList.HCount) - 10f);
            ItemList.SetInventory(items);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            // evt.ScrollWheelValue 鼠标滚轮 下滑: - 上滑: +
            ZeroScrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ZeroScrollbar.ViewPosition != Top.Pixels)
            {
                ItemList.Top.Pixels = -ZeroScrollbar.ViewPosition;
                ItemList.Recalculate();
            }
        }
    }
}
