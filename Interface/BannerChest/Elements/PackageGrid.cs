using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.BannerChest.Elements
{
    public class PackageGrid : View
    {
        public PackageList list;
        public SUIScrollbar scrollbar;
        public List<Item> items;

        public PackageGrid()
        {
            SetPadding(0);

            // 隐藏子元素, 可显示的范围是计算 Padding 之后的.
            OverflowHidden = true;

            Append(list = new PackageList());
            Vector2 size = PackageList.GetSize(5, 5);

            Append(scrollbar = new SUIScrollbar() { HAlign = 1f });
            scrollbar.Left.Pixels = -1;
            scrollbar.Height.Pixels = size.Y;

            Width.Pixels = size.X + 32;
            Height.Pixels = size.Y + 1;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (scrollbar.ViewPosition != list.Top.Pixels)
            {
                list.Top.Pixels = -scrollbar.ViewPosition;
                list.Recalculate();
            }
        }

        private int oldCount;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 写在 Grid 中最主要的原因是是因为 ScrollBar 在里面
            bool reload = false;
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.IsAir)
                {
                    reload = true;
                    items.RemoveAt(i--);
                }
            }
            if (oldCount != items.Count || reload)
            {
                oldCount = items.Count;
                SetInventory(items);
            }
        }

        public void SetInventory(List<Item> items)
        {
            this.items = items;
            list.SetInventory(items);
            Vector2 size = PackageList.GetSize(5, 5);
            scrollbar.SetView(size.Y, list.Height.Pixels);
        }
    }
}
