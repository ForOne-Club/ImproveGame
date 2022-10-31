namespace ImproveGame.Interface.BannerChestUI
{
    public class PackageList : UIElement
    {
        public List<Item> items;
        private Vector2 ChildSize;
        private Vector2 Spaceing;
        private Point DisplayCount;

        public override bool ContainsPoint(Vector2 point) => Parent.GetInnerDimensions().Contains(Main.MouseScreen);
        public Vector2 DisplaySize => ChildSize * DisplayCount.ToVector2() + (DisplayCount - new Point(1, 1)).ToVector2() * Spaceing;
        public PackageList(Vector2 childSize, Point displayCount, Vector2 spaceing)
        {
            SetPadding(0);
            ChildSize = childSize;
            DisplayCount = displayCount;
            Spaceing = spaceing;
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
            for (int i = 0; i < items.Count; i++)
            {
                float col = i % DisplayCount.X;
                float row = i / DisplayCount.X;
                ItemSlot_Package ItemSlot;
                Append(ItemSlot = new(items, i));
                ItemSlot.SetPos(col * (ChildSize.X + Spaceing.X), row * (ChildSize.Y + Spaceing.Y));
            }
            int VCount = items.Count / DisplayCount.X + (items.Count % DisplayCount.X > 0 ? 1 : 0);
            Height.Pixels = ChildSize.Y * VCount + Spaceing.Y * (VCount - 1);
            Width.Pixels = DisplaySize.X;
        }
    }
}
