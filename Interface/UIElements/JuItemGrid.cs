using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace ImproveGame.Interface.UIElements
{
    public class JuItemGrid : UIElement
    {
        public static Vector2 SlotSize => TextureAssets.InventoryBack.Size();

        public JuItemList ItemList;
        public ModScrollbar Scrollbar;

        public JuItemGrid(UserInterface userInterface)
        {
            SetPadding(0);
            OverflowHidden = true;

            ItemList = new();
            ItemList.Width.Pixels = SlotSize.X * JuItemList.HCount + 10f * (JuItemList.HCount - 1);
            ItemList.Height.Pixels = SlotSize.Y * JuItemList.VCount + 10f * (JuItemList.VCount - 1);
            Append(ItemList);
            Width.Pixels = ItemList.Width.Pixels;
            Height.Pixels = ItemList.Height.Pixels;

            Scrollbar = new(userInterface);
            Scrollbar.Height.Set(Height.Pixels - 12f, 0f);
            Scrollbar.HAlign = 1f;
            Scrollbar.VAlign = 0.5f;
            Width.Pixels += Scrollbar.Width.Pixels + 10f;
            Append(Scrollbar);
        }

        public void SetInventory(Item[] items)
        {
            Scrollbar.SetView(Height.Pixels, SlotSize.Y * (items.Length / JuItemList.HCount) + 10f * (items.Length / JuItemList.HCount) - 10f);
            ItemList.SetInventory(items);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            Scrollbar.SetViewPosition(evt.ScrollWheelValue);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Scrollbar != null)
            {
                ItemList.Top.Set(-Scrollbar.GetValue(), 0);
            }
            ItemList.Recalculate();
        }
    }

    /// <summary>
    /// 显示物品的列表
    /// </summary>
    public class JuItemList : UIElement
    {
        public readonly static int HCount = 10;
        public readonly static int VCount = 5;

        public Item[] items;

        public void SetInventory(Item[] items)
        {
            this.items = items;
            for (int i = 0; i < items.Length; i++)
            {
                var ItemSlot = new ArrayItemSlot(this, i);
                ItemSlot.Left.Pixels = i % HCount * (ItemSlot.Width.Pixels + 10f);
                ItemSlot.Top.Pixels = i / HCount * (ItemSlot.Height.Pixels + 10f);
                Append(ItemSlot);
            }
        }

        public override void OnInitialize()
        {
            SetPadding(0);
        }

        public override bool ContainsPoint(Vector2 point) => true;

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            var position = Parent.GetDimensions().Position();
            var dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
            foreach (UIElement current in Elements)
            {
                var position2 = current.GetDimensions().Position();
                var dimensions2 = new Vector2(current.GetDimensions().Width, current.GetDimensions().Height);
                if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
                {
                    current.Draw(spriteBatch);
                }
            }
        }
    }
}
