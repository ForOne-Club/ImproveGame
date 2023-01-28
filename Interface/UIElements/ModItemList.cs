using ImproveGame.Interface.BaseViews;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.UIElements
{
    public class ModItemList : View
    {
        public Item[] items;
        public ModItemList()
        {
            SetPadding(0);
        }

        public static Vector2 GetSize(int HCount, int VCount)
        {
            Vector2 size;
            size.X = 52 * HCount + 10f * (HCount - 1);
            size.Y = 52 * VCount + 10f * (VCount - 1);
            return size;
        }

        public void ModifyHVCount(int HCount, int VCount)
        {
            Width.Pixels = 52 * HCount + 10f * (HCount - 1);
            Height.Pixels = 52 * VCount + 10f * (VCount - 1);
        }

        public event MouseEvent OnMouseDownSlot;

        public void SetInventory(Item[] items)
        {
            if (Elements.Count != items.Length)
            {
                RemoveAllChildren();
                this.items = items;
                for (int i = 0; i < items.Length; i++)
                {
                    BigBagItemSlot itemSlot = new BigBagItemSlot(items, i);
                    itemSlot.OnMouseDown += OnMouseDownSlot;
                    itemSlot.Join(this);
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    UIElement uie = Elements[i];
                    if (uie is BigBagItemSlot itemSlot)
                    {
                        itemSlot.Items = items;
                    }
                }
            }
        }

        // 因为,
        public override bool ContainsPoint(Vector2 point) => true;

        // 只绘制范围内的孩子.
        public override void DrawChildren(SpriteBatch spriteBatch)
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
