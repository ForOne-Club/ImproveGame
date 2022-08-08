using ImproveGame.Interface.UIElements_Shader;

namespace ImproveGame.Interface.UIElements
{
    // 一个有滚动条的视图有三部分.
    // 一个最大的元素, 用来包裹显示内容部分和滚动条. 最大的元素负责隐藏超过范围的元素.
    public class ModItemGrid : UIElement
    {
        public ModItemList ItemList;
        public ZeroScrollbar Scrollbar;

        // 可以在 new 的时候将其他元素也初始化, 或者在执行 Active() 的时候初始化.
        public ModItemGrid()
        {
            // 正经人谁要内边距啊. 你要内边距吗?
            SetPadding(0);

            // 隐藏子元素, 可显示的范围是计算 Padding 之后的.
            OverflowHidden = true;

            // 滚动主体, 我做的这个是在初始化的时候就直接计算好大小了.
            Append(ItemList = new());

            // 滚动条, 一定要放到滚动主体后面, 问就是 UIElement 的锅.
            Append(Scrollbar = new() { HAlign = 1f });
            Scrollbar.Left.Pixels = -1;
            Scrollbar.Height.Pixels = ItemList.Height();

            // 既然尺寸都已知了, 那就直接设置他们爹地的大小吧.
            Width.Pixels = ItemList.Width() + Scrollbar.Width() + 10;
            Height.Pixels = ItemList.Height() + 1;
        }

        // 咱就是说, 这名字挺好看的.
        public void SetInventory(Item[] items)
        {
            // 初始化 ItemList 的时候会计算高度, 但是计算的是显示的高度.
            // 在 SetInventory 之后还会再计算一次, 计算的是 添加 items 之后的实际高度.
            ItemList.SetInventory(items);
            Scrollbar.SetView(MathF.Min(Height.Pixels - 1, ItemList.Height.Pixels), ItemList.Height.Pixels);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            // 下滑: -, 上滑: +
            Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Scrollbar.ViewPosition != ItemList.Top.Pixels)
            {
                ItemList.Top.Pixels = -Scrollbar.ViewPosition;
                ItemList.Recalculate();
            }
        }
    }
}
