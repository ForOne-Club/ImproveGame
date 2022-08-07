namespace ImproveGame.Interface.UIElements
{
    /// <summary>
    /// 一个单元的掉率信息
    /// </summary>
    public class MaterialInfoElement : UIPanel
    {
        private readonly Item iconItem;
        public int ItemType;
        public int StackRequired;
        public UITextPanel<string> StackInfo;
        public UITextPanel<string> StackCheckedInfo;

        public MaterialInfoElement(int itemType, int stackRequired)
        {
            this.SetSize(new Vector2(0f, 28f), 1f);
            BorderColor = Color.Transparent;
            BackgroundColor = Color.Transparent;

            StackRequired = stackRequired;
            ItemType = itemType;
            iconItem = new(ItemType);

            StackInfo = new($"需求: {stackRequired}", 1f)
            {
                IgnoresMouseInteraction = true,
                DrawPanel = false,
                VAlign = 0.5f,
                Left = new StyleDimension(-110f, 1f)
            };
            Append(StackInfo);

            StackCheckedInfo = new(stackRequired.ToString(), 1f)
            {
                IgnoresMouseInteraction = true,
                DrawPanel = false,
                VAlign = 0.5f,
                Left = new StyleDimension(-250f, 1f)
            };
            Append(StackCheckedInfo);

            UITextPanel<string> itemNameInfo = new(Lang.GetItemNameValue(ItemType), 1f)
            {
                IgnoresMouseInteraction = true,
                DrawPanel = false,
                VAlign = 0.5f,
                Left = new StyleDimension(40f, 0f)
            };
            Append(itemNameInfo);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var inventory = GetAllInventoryItemsList(Main.LocalPlayer, ignorePortable: true).ToArray();
            GetItemCount(inventory, (item) => item.type == ItemType, out int stackCount);
            StackCheckedInfo.SetText($"已有: {stackCount}");
            if (stackCount >= 100000)
            {
                StackCheckedInfo.SetText($"已有: >99999");
            }
            if (stackCount < StackRequired)
            {
                StackCheckedInfo.TextColor = Color.Yellow;
            }
            else
            {
                StackCheckedInfo.TextColor = Color.LightGreen;
            }
            Recalculate();

            base.DrawSelf(spriteBatch);

            var dimensions = GetDimensions();
            dimensions.Width = 28f;
            dimensions.X += 16f;
            ArrayItemSlot.DrawItem(sb: spriteBatch,
                                   Item: iconItem,
                                   lightColor: Color.White,
                                   dimensions: dimensions,
                                   ItemSize: 26);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
