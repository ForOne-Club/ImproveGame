using ImproveGame.Interface.BaseViews;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.UIElements
{
    // 一个有滚动条的视图有三部分.
    // 一个最大的元素, 用来包裹显示内容部分和滚动条. 最大的元素负责隐藏超过范围的元素.
    public class ModItemGrid : View
    {
        public readonly ModItemList ItemList;
        private readonly SUIScrollbar _scrollbar;
        private readonly Vector2 _showSize;

        // 可以在 new 的时候将其他元素也初始化, 或者在执行 Active() 的时候初始化.
        public ModItemGrid()
        {
            // 隐藏子元素, 可显示的范围是计算 Padding 之后的.
            OverflowHidden = true;
            DragIgnore = true;

            ItemList = new ModItemList
            {
                DragIgnore = true
            };
            ItemList.ModifyHVCount(10, 10);
            ItemList.Join(this);

            _scrollbar = new SUIScrollbar
            {
                HAlign = 1f
            };
            _scrollbar.Join(this);

            _showSize = ModItemList.GetSize(10, 5);

            _scrollbar.Left.Pixels = -1;
            _scrollbar.Height.Pixels = ItemList.Height();
            _scrollbar.SetView(_showSize.Y, ItemList.Height.Pixels);

            Width.Pixels = _showSize.X + _scrollbar.Width.Pixels + 11f;
            Height.Pixels = _showSize.Y + 1f;
        }

        public void SetInventory(Item[] items)
        {
            // 初始化 ItemList 的时候会计算高度, 但是计算的是显示的高度.
            // 在 SetInventory 之后还会再计算一次, 计算的是 添加 items 之后的实际高度.
            ItemList.SetInventory(items);
            _scrollbar.SetView(_showSize.Y, ItemList.Height.Pixels);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            // 下滑: -, 上滑: +
            _scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Math.Abs(-_scrollbar.ViewPosition - ItemList.Top.Pixels) < 0.000000001)
            {
                return;
            }

            ItemList.Top.Pixels = -_scrollbar.ViewPosition;
            ItemList.Recalculate();
        }
    }
}