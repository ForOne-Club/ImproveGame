namespace ImproveGame.Interface.UIElements
{
    public class ModItemList : UIElement
    {
        public readonly static int HCount = 10; // 横向格子的数量
        public readonly static int VCount = 5; // 纵向格子的数量

        public Item[] items;

        public void SetInventory(Item[] items)
        {
            RemoveAllChildren();
            this.items = items;
            for (int i = 0; i < items.Length; i++)
            {
                float col = i % HCount;
                float row = i / HCount;
                ArrayItemSlot ItemSlot = new(items, i);
                ItemSlot.SetPos(col * (ItemSlot.Width() + 10f), row * (ItemSlot.Height() + 10f));
                Append(ItemSlot);
            }
        }

        public ModItemList(Vector2 SlotSize)
        {
            ModifySize(SlotSize);
        }

        public void ModifySize(Vector2 SlotSize)
        {
            Width.Pixels = SlotSize.X * HCount + 10f * (HCount - 1);
            Height.Pixels = SlotSize.Y * VCount + 10f * (VCount - 1);
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
            foreach (UIElement uie in Elements)
            {
                var position2 = uie.GetDimensions().Position();
                var dimensions2 = new Vector2(uie.GetDimensions().Width, uie.GetDimensions().Height);
                if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
                {
                    uie.Draw(spriteBatch);
                }
            }
        }
    }
}
