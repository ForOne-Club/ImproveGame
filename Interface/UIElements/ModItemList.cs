using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.UIElements
{
    public class ModItemList : UIElement
    {
        public int HCount = 10; // 横向格子的数量
        public int VCount = 5; // 纵向格子的数量

        public Item[] items;

        public void SetInventory(Item[] items)
        {
            RemoveAllChildren();
            this.items = items;
            for (int i = 0; i < items.Length; i++)
            {
                float col = i % HCount;
                float row = i / HCount;
                ItemSlot_BigBag ItemSlot = new(items, i);
                ItemSlot.OnMouseDown += OnMouseDownSlot;
                ItemSlot.SetPos(col * (ItemSlot.Width() + 10f), row * (ItemSlot.Height() + 10f));
                Append(ItemSlot);
            }
            int VCount = items.Length / HCount + (items.Length % HCount > 0 ? 1 : 0);
            Height.Pixels = 52 * VCount + 10f * (VCount - 1);
        }

        public event MouseEvent OnMouseDownSlot;

        public ModItemList(int HCount = 10, int VCount = 5)
        {
            SetPadding(0);

            this.HCount = HCount;
            this.VCount = VCount;

            Width.Pixels = 52 * HCount + 10f * (HCount - 1);
            Height.Pixels = 52 * VCount + 10f * (VCount - 1);
        }

        // 因为,
        public override bool ContainsPoint(Vector2 point) => true;

        // 只绘制范围内的孩子.
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions1 = Parent.GetDimensions();
            var position1 = dimensions1.Position();
            var size1 = dimensions1.Size();
            foreach (UIElement uie in Elements)
            {
                CalculatedStyle dimensions2 = uie.GetDimensions();
                var position2 = dimensions2.Position();
                var size2 = dimensions2.Size();
                if (Collision.CheckAABBvAABBCollision(position1, size1, position2, size2))
                {
                    uie.Draw(spriteBatch);
                }
            }
        }
    }
}
