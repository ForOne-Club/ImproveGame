using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UIFramework.UIElements
{
    // 一个有滚动条的视图有三部分.
    // 一个最大的元素, 用来包裹显示内容部分和滚动条. 最大的元素负责隐藏超过范围的元素.
    public class ModItemGrid : View
    {
        public ModItemList ItemList;
        // protected 保证只有子类可以访问与修改
        protected SUIScrollBar Scrollbar;
        protected Vector2 ShowSize;

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
            ItemList.JoinParent(this);

            Scrollbar = new SUIScrollBar
            {
                HAlign = 1f
            };
            Scrollbar.JoinParent(this);

            ShowSize = ModItemList.GetSize(10, 5);

            Scrollbar.Left.Pixels = -1;
            Scrollbar.Height.Pixels = ItemList.Height();
            Scrollbar.SetView(ShowSize.Y, ItemList.Height.Pixels);

            Width.Pixels = ShowSize.X + Scrollbar.Width.Pixels + 11f;
            Height.Pixels = ShowSize.Y + 1f;
        }

        public void SetInventory(Item[] items)
        {
            // 初始化 ItemList 的时候会计算高度, 但是计算的是显示的高度.
            // 在 SetInventory 之后还会再计算一次, 计算的是 添加 items 之后的实际高度.
            ItemList.SetInventory(items);
            Scrollbar.SetView(ShowSize.Y, ItemList.Height.Pixels);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            // 下滑: -, 上滑: +
            Scrollbar.BarTopBuffer += evt.ScrollWheelValue;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (-Scrollbar.BarTop == ItemList.Top.Pixels)
            {
                return;
            }

            ItemList.Top.Pixels = -Scrollbar.BarTop;
            ItemList.Recalculate();
        }
    }
}