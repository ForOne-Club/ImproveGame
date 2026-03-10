using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UIFramework.UIElements
{
    public class ModItemList : View
    {
        public Item[] items;
        public ModItemList()
        {
            SetPadding(0);
            MaxWidth = MaxHeight = new StyleDimension(float.MaxValue, 0f);
        }

        public static Vector2 GetSize(int HCount, int VCount, int slotSize = 52, float spacing = 10f)
        {
            Vector2 size;
            size.X = slotSize * HCount + spacing * (HCount - 1);
            size.Y = slotSize * VCount + spacing * (VCount - 1);
            return size;
        }

        public void ModifyHVCount(int HCount, int VCount, int slotSize = 52, float spacing = 10f)
        {
            Width.Pixels = slotSize * HCount + spacing * (HCount - 1);
            Height.Pixels = slotSize * VCount + spacing * (VCount - 1);
        }

        public event MouseEvent OnMouseDownSlot;

        public void SetInventory(Item[] items, int slotSize = 52, float spacing = 10f)
        {
            if (Elements.Count != items.Length)
            {
                RemoveAllChildren();
                this.items = items;
                for (int i = 0; i < items.Length; i++)
                {
                    BigBagItemSlot itemSlot = new BigBagItemSlot(items, i);
                    itemSlot.OnLeftMouseDown += OnMouseDownSlot;
                    itemSlot.JoinParent(this);
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
        public override bool ContainsPoint(Vector2 point) => base.ContainsPoint(point);

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
