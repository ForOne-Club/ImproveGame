using ImproveGame.Interface.BaseUIEs;

namespace ImproveGame.Interface.BannerChest.Elements
{
    public class PackageList : View
    {
        public List<Item> items;

        public override bool ContainsPoint(Vector2 point) => Parent.GetInnerDimensions().Contains(Main.MouseScreen);
        public PackageList()
        {
            SetPadding(0);
            ModifyHVCount(5, 5);
        }

        // 只绘制范围内的孩子.
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = Parent.GetDimensions();
            // Scrn 是 Screen 的缩写
            // POS 是 Position 的缩写
            // S 是 Size 的缩写
            Vector2 ScrnPOS = dimensions.Position();
            Vector2 S = dimensions.Size();
            foreach (UIElement uie in Elements)
            {
                CalculatedStyle dimensions2 = uie.GetDimensions();
                Vector2 position2 = dimensions2.Position();
                Vector2 size2 = dimensions2.Size();
                if (Collision.CheckAABBvAABBCollision(ScrnPOS, S, position2, size2))
                {
                    uie.Draw(spriteBatch);
                }
            }
        }

        public void SetInventory(List<Item> items)
        {
            RemoveAllChildren();
            this.items = items;
            int length = items.Count;
            // 未达到 1 整行填满 1 整行
            // 达到 1 整行再加 1 行
            length += 5 - length % 5;
            // 小于 5 行填满 5 行
            if (length < 25)
                length = 25;
            for (int i = 0; i < length; i++)
            {
                Append(new PackageItemSlot(items, i));
            }
            int HCount = 5;
            int VCount = length / 5;
            ModifyHVCount(HCount, VCount);
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
    }
}
