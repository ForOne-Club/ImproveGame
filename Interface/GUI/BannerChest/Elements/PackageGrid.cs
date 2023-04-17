using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI.BannerChest.Elements
{
    public class PackageGrid : ScrollView
    {
        public List<Item> Items;

        public PackageGrid()
        {
            ListView.SetInnerPixels(GridSize(52f, 8f, 5));

            Scrollbar.HAlign = 1f;
            Scrollbar.Left.Pixels = -1;
            Scrollbar.SetSizePixels(16, ListView.Height.Pixels);
            Scrollbar.SetPadding(4);

            SetInnerPixels(ListView.Width.Pixels + Scrollbar.Width.Pixels + 9, ListView.Height.Pixels + 1);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (!(Math.Abs(-Scrollbar.ViewPosition - ListView.Top.Pixels) > 0.000000001f))
            {
                return;
            }

            ListView.Top.Pixels = -Scrollbar.ViewPosition;
            ListView.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            // 寻找 Items 中有没有 Item.IsAir
            // 有的话删除掉
            for (int i = 0; i < Items.Count; i++)
            {
                Item item = Items[i];
                if (!item.IsAir)
                {
                    continue;
                }

                Items.RemoveAt(i--);
            }

            if (ListView.Children.Count() != RequiredChildrenCount(Items.Count))
            {
                SetInventory(Items);
            }

            // 因为 Update 是一层一层调用子元素的 Update()，所以不能放在前面。
            base.Update(gameTime);
        }

        private static int RequiredChildrenCount(int length)
        {
            if (length < 25)
            {
                length = 25;
            }
            else
            {
                length += 5 - length % 5;
            }

            return length;
        }

        public void SetInventory(List<Item> items)
        {
            Items = items;
            ListView.RemoveAllChildren();

            int length = RequiredChildrenCount(items.Count);
            for (int i = 0; i < length; i++)
            {
                new PackageItemSlot(items, i).Join(ListView);
            }

            ListView.SetInnerPixels(GridSize(52f, 8f, 5, length / 5));
            Scrollbar.SetView(GetInnerDimensions().Height, ListView.Height.Pixels + 1);
        }
    }
}