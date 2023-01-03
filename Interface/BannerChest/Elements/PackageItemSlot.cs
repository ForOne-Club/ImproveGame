using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.BannerChest.Elements
{
    public class PackageItemSlot : View
    {
        public Texture2D Banner;
        public Texture2D Potion;
        private int RightMouseTimer;
        public Item AirItem;
        public Func<Item, bool> CanPutItemSlot;
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
            Spacing = new(10, 10);
            Relative = RelativeMode.Horizontal;
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            bool MouseItemIsAir = Main.mouseItem.IsAir;
            base.MouseDown(evt);
            if (Item.IsAir)
                return;
            if (MouseItemIsAir)
            {
                SetCursor();
                MouseDown_ItemSlot();
            }
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
            if (Main.mouseRight && IsMouseHovering && !Item.IsAir)
            {
                if (RightMouseTimer >= 60)
                {
                    TakeSlotItemToMouseItem();
                }
                else if (RightMouseTimer >= 30 && RightMouseTimer % 3 == 0)
                {
                    TakeSlotItemToMouseItem();
                }
                else if (RightMouseTimer >= 15 && RightMouseTimer % 6 == 0)
                {
                    TakeSlotItemToMouseItem();
                }
                RightMouseTimer++;
            }
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            PixelShader.DrawRoundRect(dimensions.Position(), dimensions.Size(), 12, UIColor.ItemSlotBG, 3, UIColor.ItemSlotBorder);

            if (Item.IsAir)
            {
                switch (PackageGUI.storageType)
                {
                    case PackageGUI.StorageType.Banners:
                        BigBagItemSlot.DrawItemIcon(sb, Banner, Color.White * 0.5f, dimensions);
                        break;
                    case PackageGUI.StorageType.Potions:
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
                Vector2 textSize = MouseTextSize(Item.stack.ToString()) * 0.75f;
                Vector2 textPos = dimensions.Position() + new Vector2(52 * 0.18f, (52 - textSize.Y) * 0.9f);
                TrUtils.DrawBorderString(sb, Item.stack.ToString(), textPos, Color.White, 0.75f);
            }

            if (IsMouseHovering)
            {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item.Clone();
                SetCursor();
            }
        }

        /// <summary>
        /// 拿物品槽内物品到鼠标物品上
        /// </summary>
        public void TakeSlotItemToMouseItem()
        {
            bool CanPlaySound = false;
            if (Main.mouseItem.type == Item.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
            {
                Main.mouseItem.stack++;
                Item.stack--;
                CanPlaySound = true;
            }
            else if (Main.mouseItem.IsAir)
            {
                Main.mouseItem = new Item(Item.type, 1);
                Item.stack--;
                CanPlaySound = true;
            }
            if (CanPlaySound)
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

            // 放大镜图标 - 输入到聊天框
            if (Main.cursorOverride == CursorOverrideID.Magnifiers)
            {
                if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(Item), Vector2.One))
                    SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 放回物背包图标
            if (Main.cursorOverride == CursorOverrideID.ChestToInventory)
            {
                Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (!CanPutItemSlot?.Invoke(Main.mouseItem) ?? false)
                return;

            if (Main.mouseItem.IsAir)
            {
                Main.mouseItem = Item;
                Item = new Item();
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }
    }
}
