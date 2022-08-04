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
            this.SetSize(new Vector2(0f, 30f), 1f);
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
                Left = new StyleDimension(-150f, 1f)
            };
            Append(StackInfo);

            StackCheckedInfo = new(stackRequired.ToString(), 1f)
            {
                IgnoresMouseInteraction = true,
                DrawPanel = false,
                VAlign = 0.5f,
                Left = new StyleDimension(-280f, 1f)
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
            // 只检测物品栏
            int stackChecked = 0;
            for (int i = 0; i < 50; i++)
            {
                if (Main.LocalPlayer.inventory[i].type == ItemType)
                {
                    stackChecked += Main.LocalPlayer.inventory[i].stack;
                }
                if (stackChecked > 9999)
                {
                    stackChecked = 10000;
                    break;
                }
            }
            StackCheckedInfo.SetText($"已有: {stackChecked}");
            if (stackChecked == 10000)
            {
                StackCheckedInfo.SetText($"已有: >9999");
            }
            if (stackChecked < StackRequired)
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
            dimensions.Width = 30f;
            ArrayItemSlot.DrawItem(sb: spriteBatch,
                                   Item: iconItem,
                                   lightColor: Color.White,
                                   dimensions: dimensions,
                                   ItemSize: 28);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
