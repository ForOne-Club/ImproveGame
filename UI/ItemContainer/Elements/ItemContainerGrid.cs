using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ItemContainer.Elements;

public class ItemContainerGrid : SUIScrollView2
{
    public static int HNumber => 6;

    public List<Item> Items;

    public ItemContainerGrid() : base(ScrollType.Vertical)
    {
        IsAdaptiveWidth = true;
        OverflowHiddenView.IsAdaptiveWidth = true;
        AdaptiveView.IsAdaptiveWidth = true;

        VScrollBar.BarColor = UIStyle.ScrollBarBorder * 0.5f;
        VScrollBar.BarHoverColor = UIStyle.ScrollBarBorder * 0.75f;
        /*ListView.SetInnerPixels(new Vector2(GridSize(52f, 4f, 5)));

        Scrollbar.HAlign = 1f;
        Scrollbar.Left.Pixels = -1;
        Scrollbar.SetSizePixels(16, ListView.Height.Pixels);
        Scrollbar.SetPadding(4);

        SetInnerPixels(ListView.Width.Pixels + Scrollbar.Width.Pixels + 9, ListView.Height.Pixels + 1);*/
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

        if (AdaptiveView.Children.Count() != GetSlotNumber(Items.Count))
        {
            SetInventory(Items);
        }

        // 因为 Update 是一层一层调用子元素的 Update()，所以不能放在前面。
        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        /*if (Math.Abs(-Scrollbar.BarTop - ListView.Top.Pixels) > 0.000000001f)
        {
            ListView.Top.Pixels = -Scrollbar.BarTop;
            ListView.Recalculate();
        }*/

        base.DrawSelf(spriteBatch);
    }

    public static int GetSlotNumber(int count) => Math.Max(HNumber * 6, count - count % HNumber + HNumber * 2);

    public void SetInventory(List<Item> items)
    {
        Items = items;
        AdaptiveView.RemoveAllChildren();

        int length = GetSlotNumber(Items.Count);
        for (int i = 0; i < length; i++)
        {
            var itemSlot = new ItemContainerItemSlot(items, i)
            {
                Spacing = new Vector2(4),
                RelativeMode = RelativeMode.Horizontal,
                Rounded = new Vector4(12f),
                BgColor = UIStyle.ItemSlotBg,
                Border = 2f,
                BorderColor = UIStyle.ItemSlotBorder
            };

            if (i % HNumber == 0)
                itemSlot.DirectLineBreak = true;

            itemSlot.JoinParent(AdaptiveView);
        }

        Recalculate();
        // ListView.SetInnerPixels(GridSize(52f, 8f, 5, length / 5));
        // Scrollbar.SetView(GetInnerDimensions().Height, ListView.Height.Pixels + 1);
    }
}