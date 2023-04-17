using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.GUI.BannerChest.Elements
{
    public class PackageItemSlot : View
    {
        private Texture2D Banner;
        private Texture2D Potion;
        private int RightMouseTimer;
        private Item AirItem;
        private Func<Item, bool> _canPutItemSlot;
        private List<Item> items;
        private int index;

        private Item Item
        {
            get => items.IndexInRange(index) ? items[index] : AirItem;
            set => items[index] = value;
        }

        public PackageItemSlot(List<Item> items, int index)
        {
            Banner = GetTexture("UI/Banner").Value;
            Potion = GetTexture("UI/Potion").Value;
            AirItem = new Item();
            Width.Pixels = 52;
            Height.Pixels = 52;
            this.items = items;
            this.index = index;

            Wrap = true;
            Spacing = new Vector2(8);
            Relative = RelativeMode.Horizontal;
            Rounded = new Vector4(12f);
            BgColor = UIColor.ItemSlotBg;
            Border = 2f;
            BorderColor = UIColor.ItemSlotBorder;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            bool mouseItemIsAir = Main.mouseItem.IsAir;
            base.LeftMouseDown(evt);
            if (Item.IsAir || !mouseItemIsAir)
                return;

            SetCursor();
            MouseDown_ItemSlot();
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.RightMouseDown(evt);
            if (Item.IsAir)
                return;
            RightMouseTimer = 0;
            TakeSlotItemToMouseItem();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (!Main.mouseRight || !IsMouseHovering || Item.IsAir)
            {
                return;
            }

            switch (RightMouseTimer)
            {
                case >= 60:
                case >= 30 when RightMouseTimer % 3 == 0:
                case >= 15 when RightMouseTimer % 6 == 0:
                    TakeSlotItemToMouseItem();
                    break;
            }

            RightMouseTimer++;
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            CalculatedStyle dimensions = GetDimensions();

            if (Item.IsAir)
            {
                switch (PackageGUI.StorageType)
                {
                    case StorageType.Banners:
                        BigBagItemSlot.DrawItemIcon(sb, Banner, Color.White * 0.5f, dimensions);
                        break;
                    case StorageType.Potions:
                        BigBagItemSlot.DrawItemIcon(sb, Potion, Color.White * 0.5f, dimensions);
                        break;
                }
            }
            else
            {
                BigBagItemSlot.DrawItemIcon(sb, Item, Color.White, dimensions);
            }

            if (!Item.IsAir && Item.stack > 1)
            {
                Vector2 textSize = GetFontSize(Item.stack) * 0.75f;
                Vector2 textPos = dimensions.Position() + new Vector2(52 * 0.18f, (52 - textSize.Y) * 0.9f);
                TrUtils.DrawBorderString(sb, Item.stack.ToString(), textPos, Color.White, 0.75f);
            }

            if (!IsMouseHovering)
            {
                return;
            }

            Main.hoverItemName = Item.Name;
            Main.HoverItem = Item.Clone();
            SetCursor();
        }

        /// <summary>
        /// 拿物品槽内物品到鼠标物品上
        /// </summary>
        private void TakeSlotItemToMouseItem()
        {
            bool playSound = false;
            if (Main.mouseItem.type == Item.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
            {
                Main.mouseItem.stack++;
                Item.stack--;
                playSound = true;
            }
            else if (Main.mouseItem.IsAir)
            {
                Main.mouseItem = new Item(Item.type, 1);
                Item.stack--;
                playSound = true;
            }

            if (playSound)
                SoundEngine.PlaySound(SoundID.MenuTick);
        }

        private static void SetCursor()
        {
            // 快速取出
            if (ItemSlot.ShiftInUse)
            {
                Main.cursorOverride = CursorOverrideID.ChestToInventory; // 快捷放回物品栏图标
            }

            // 放入聊天框
            if (Main.keyState.IsKeyDown(Main.FavoriteKey) && Main.drawingPlayerChat)
            {
                Main.cursorOverride = CursorOverrideID.Magnifiers;
            }
        }

        /// <summary>
        /// 左键点击物品
        /// </summary>
        private void MouseDown_ItemSlot()
        {
            if (Main.LocalPlayer.ItemAnimationActive)
                return;

            switch (Main.cursorOverride)
            {
                // 放大镜图标 - 输入到聊天框
                case CursorOverrideID.Magnifiers:
                    {
                        if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(Item),
                                Vector2.One))
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        return;
                    }
                // 放回物背包图标
                case CursorOverrideID.ChestToInventory:
                    Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item,
                        GetItemSettings.InventoryEntityToPlayerInventorySettings);
                    SoundEngine.PlaySound(SoundID.Grab);
                    return;
            }

            if (!_canPutItemSlot?.Invoke(Main.mouseItem) ?? false)
                return;

            if (!Main.mouseItem.IsAir)
            {
                return;
            }

            Main.mouseItem = Item;
            Item = new Item();
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }
}