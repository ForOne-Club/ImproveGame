using ImproveGame.Interface.UIElements_Shader;

namespace ImproveGame.Interface.BannerChestUI
{
    public class PackageGrid : UIElement
    {
        public PackageList list;
        public ZeroScrollbar scrollbar;
        public List<Item> items;

        public PackageGrid()
        {
            SetPadding(0);

            // 隐藏子元素, 可显示的范围是计算 Padding 之后的.
            OverflowHidden = true;

            Append(list = new(new(52), new(5, 5), new(8)));

            Vector2 liseSize = list.DisplaySize;
            // 滚动条, 一定要放到滚动主体后面, 问就是 UIElement 的锅.
            Append(scrollbar = new());
            scrollbar.Left.Pixels = liseSize.X + 7;
            scrollbar.Height.Pixels = liseSize.Y;

            // 既然尺寸都已知了, 那就直接设置他们爹地的大小吧.
            Width.Pixels = scrollbar.Right() + 1;
            Height.Pixels = scrollbar.Height() + 1;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }

        // 如果少于原来的数量就重新计算
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 检测时候有空余的位置需要刷新
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsAir)
                {
                    items.Remove(items[i]);
                    i--;
                    continue;
                }
            }
            if (items is not null && list.Children.Count() != items.Count)
            {
                SetInventory(items);
                Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (scrollbar.ViewPosition != list.Top.Pixels)
            {
                list.Top.Pixels = -scrollbar.ViewPosition;
                list.Recalculate();
            }
        }

        // 咱就是说, 这名字挺好看的.
        public void SetInventory(List<Item> items)
        {
            this.items = items;
            list.SetInventory(items);
            scrollbar.SetView(MathF.Min(scrollbar.Height.Pixels, list.Height.Pixels), list.Height.Pixels);
        }
    }
}
