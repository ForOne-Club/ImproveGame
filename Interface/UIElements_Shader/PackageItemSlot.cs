using ImproveGame.Common.Animations;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements_Shader
{
    public class PackageItemSlot : UIElement
    {
        private readonly Color BorderColor = new(18, 18, 38, 200);
        private readonly Color Background = new(63, 65, 151, 200);

        private int RightMouseTimer;

        public Func<Item, bool> CanPutItemSlot;
        public List<Item> items;
        public int index;
        public Item Item
        {
            get => items[index];
            set
            {
                if (value is null || value.IsAir)
                {
                    items.RemoveAt(index);
                    Parent.RemoveChild(Parent.Children.ToArray()[^1]);
                }
                else
                {
                    items[index] = value;
                }
            }
        }

        public PackageItemSlot(List<Item> items, int index)
        {
            this.SetSize(52, 52);
            this.items = items;
            this.index = index;

            UIText text = new(string.Empty, 0.75f)
            {
                VAlign = 0.8f
            };
            text.Left.Set(0, 0.2f);
            text.OnUpdate += (uie) =>
            {
                text.SetText(Item.stack.ToString());
                text.Recalculate();
            };
            Append(text);

            UIText text2 = new((index + 1).ToString(), 0.75f)
            {
                VAlign = 0.15f
            };
            text2.Left.Set(0, 0.15f);
            Append(text2);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            SetCursor();
            MouseDown_ItemSlot();
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            if (!Item.IsAir && !ItemID.Sets.BossBag[Item.type] && !ItemID.Sets.IsFishingCrate[Item.type])
            {
                RightMouseTimer = 0;
                TakeSlotItemToMouseItem();
            }
            base.MouseDown(evt);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (Main.mouseRight && IsMouseHovering && !Item.IsAir && !ItemID.Sets.BossBag[Item.type] && !ItemID.Sets.IsFishingCrate[Item.type])
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
            PixelShader.DrawBox(Main.UIScaleMatrix, dimensions.Position(), dimensions.Size(), 12, 3, BorderColor, Background);

            if (Item.IsAir)
                return;

            DrawItem(sb, Item, Color.White, dimensions, 30);

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
                if (Item.IsAir)
                    Item.SetDefaults();
                CanPlaySound = true;
            }
            else if (Main.mouseItem.IsAir && !Item.IsAir && Item.maxStack > 1)
            {
                Main.mouseItem = new Item(Item.type, 1);
                Item.stack--;
                if (Item.IsAir)
                    Item.SetDefaults();
                CanPlaySound = true;
            }
            if (CanPlaySound)
                SoundEngine.PlaySound(SoundID.MenuTick);
        }

        private void SetCursor()
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

            // 放回物品栏图标
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

        public static void DrawItem(SpriteBatch sb, Item Item, Color lightColor, CalculatedStyle dimensions, float ItemSize = 30f)
        {
            Main.instance.LoadItem(Item.type);
            var ItemTexture2D = TextureAssets.Item[Item.type];

            Rectangle rectangle;
            if (Main.itemAnimations[Item.type] is null)
                rectangle = ItemTexture2D.Frame(1, 1, 0, 0);
            else
                rectangle = Main.itemAnimations[Item.type].GetFrame(ItemTexture2D.Value);

            float size = rectangle.Width > ItemSize || rectangle.Height > ItemSize ?
                rectangle.Width > rectangle.Height ? ItemSize / rectangle.Width : ItemSize / rectangle.Height :
                1f;

            sb.Draw(ItemTexture2D.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                new Rectangle?(rectangle), Item.GetAlpha(lightColor), 0f, Vector2.Zero, size,
                SpriteEffects.None, 0f);
            sb.Draw(ItemTexture2D.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                new Rectangle?(rectangle), Item.GetColor(lightColor), 0f, Vector2.Zero, size,
                SpriteEffects.None, 0f);
        }
    }
}
