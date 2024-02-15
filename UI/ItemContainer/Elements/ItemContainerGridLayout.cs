using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ItemContainer.Elements;

public class ItemContainerGridLayout : SUIScrollView2
{
    public const int HNumber = 6;

    public IList<Item> Items;

    public ItemContainerGridLayout() : base(Orientation.Vertical)
    {
        FixedSize = false;

        ScrollBar.ScrollMultiplier = 0.8f;

        ScrollBar.BarColor = UIStyle.ScrollBarBorder * 0.5f;
        ScrollBar.BarHoverColor = UIStyle.ScrollBarBorder * 0.75f;
        /*ListView.SetInnerPixels(new Vector2(GridSize(52f, 4f, 5)));

        Scrollbar.HAlign = 1f;
        Scrollbar.Left.Pixels = -1;
        Scrollbar.SetSizePixels(16, ListView.Height.Pixels);
        Scrollbar.SetPadding(4);

        SetInnerPixels(ListView.Width.Pixels + Scrollbar.Width.Pixels + 9, ListView.Height.Pixels + 1);*/
    }

    public override void Update(GameTime gameTime)
    {
        // 如果 Items 中有 Item.IsAir 删除掉
        for (int i = 0; i < Items.Count; i++)
        {
            Item item = Items[i];

            if (!item.IsAir)
                continue;

            Items.RemoveAt(i--);
        }

        int slotNumber = GetSlotNumber(Items.Count);
        if (ListView.Children.Count() != slotNumber)
        {
            SetInventory(Items, slotNumber);
        }

        // 因为 Update 是一层一层调用子元素的 Update()，所以不能放在前面。
        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
    }

    public static int GetSlotNumber(int count) => Math.Max(HNumber * 6, count - count % HNumber + HNumber * 2);

    public void SetInventory(IList<Item> items, int slotNumber = -1)
    {
        Items = items;
        ListView.RemoveAllChildren();

        if (slotNumber < 0)
            slotNumber = GetSlotNumber(Items.Count);

        for (int i = 0; i < slotNumber; i++)
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

            itemSlot.JoinParent(ListView);
        }

        Recalculate();
    }
}