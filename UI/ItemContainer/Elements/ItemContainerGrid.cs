using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;

namespace ImproveGame.UI.ItemContainer.Elements;

public class ItemContainerGrid : ScrollView
{
    public List<Item> Items;

    public ItemContainerGrid()
    {
        ListView.SetInnerPixels(new Vector2(GridSize(52f, 8f, 5)));

        Scrollbar.HAlign = 1f;
        Scrollbar.Left.Pixels = -1;
        Scrollbar.SetSizePixels(16, ListView.Height.Pixels);
        Scrollbar.SetPadding(4);

        SetInnerPixels(ListView.Width.Pixels + Scrollbar.Width.Pixels + 9, ListView.Height.Pixels + 1);
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

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Math.Abs(-Scrollbar.BarTop - ListView.Top.Pixels) > 0.000000001f)
        {
            ListView.Top.Pixels = -Scrollbar.BarTop;
            ListView.Recalculate();
        }

        base.DrawSelf(spriteBatch);
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
            var itemSlot = new ItemContainerItemSlot(items, i);

            itemSlot.SetSizePixels(52f, 52f);
            itemSlot.PreventOverflow = true;
            itemSlot.Spacing = new Vector2(8);
            itemSlot.RelativeMode = RelativeMode.Horizontal;
            itemSlot.Rounded = new Vector4(12f);
            itemSlot.BgColor = UIStyle.ItemSlotBg;
            itemSlot.Border = 2f;
            itemSlot.BorderColor = UIStyle.ItemSlotBorder;

            itemSlot.JoinParent(ListView);
        }

        ListView.SetInnerPixels(GridSize(52f, 8f, 5, length / 5));
        Scrollbar.SetView(GetInnerDimensions().Height, ListView.Height.Pixels + 1);
    }
}