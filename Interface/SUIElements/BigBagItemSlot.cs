using ImproveGame.Assets;
using ImproveGame.Common.Animations;
using ImproveGame.Common.ModHooks;
using ImproveGame.Interface.Common;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.SUIElements
{
    /// <summary>
    /// 用于操作数组中的物品，初始化的时候必须给定数组 + 下标。
    /// </summary>
    public class BigBagItemSlot : View
    {
        public Item[] Items { get; set; }

        public readonly int Index;

        public Item Item
        {
            get => Items[Index];
            private set => Items[Index] = value;
        }

        private int _rightMouseDownTimer = -1;

        public BigBagItemSlot(Item[] items, int index)
        {
            Width.Pixels = 52;
            Height.Pixels = 52;
            Items = items;
            Index = index;

            Relative = RelativeMode.Horizontal;
            Spacing = new Vector2(10, 10);
            Wrap = true;
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Main.NewText("MouseDown");
            SetCursorOverride();
            MouseClickSlot();
            base.MouseDown(evt);
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            Main.NewText("RightMouseDown");
            if (!Item.IsAir && !ItemID.Sets.BossBag[Item.type] && !ItemID.Sets.IsFishingCrate[Item.type])
            {
                _rightMouseDownTimer = 0;
                TakeSlotItemToMouseItem();
            }

            if (ItemID.Sets.BossBag[Item.type] || ItemID.Sets.IsFishingCrate[Item.type])
            {
                if (ItemID.Sets.BossBag[Item.type] || ItemID.Sets.BossBag[Item.type])
                    Main.LocalPlayer.OpenBossBag(Item.type);

                if (ItemID.Sets.IsFishingCrate[Item.type])
                    Main.LocalPlayer.OpenFishingCrate(Item.type);

                if (ItemLoader.ConsumeItem(Item, Main.LocalPlayer))
                    Item.stack--;

                if (Item.stack == 0)
                    Item.SetDefaults();

                SoundEngine.PlaySound(SoundID.Grab);
                Main.stackSplit = 30;
                Main.mouseRightRelease = false;
                Recipe.FindRecipes();
                return;
            }

            if (ItemLoader.CanRightClick(Item))
            {
                Main.mouseRightRelease = true;
                ItemLoader.RightClick(Item, Main.LocalPlayer);
                Main.mouseRightRelease = false;
                return;
            }

            base.RightMouseDown(evt);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (Main.mouseRight && IsMouseHovering && !Item.IsAir && !ItemID.Sets.BossBag[Item.type] && !ItemID.Sets.IsFishingCrate[Item.type])
            {
                Main.NewText($"Update MouseRight {Main.mouseRight}");
                if (_rightMouseDownTimer >= 60)
                {
                    TakeSlotItemToMouseItem();
                }
                else if (_rightMouseDownTimer >= 30 && _rightMouseDownTimer % 3 == 0)
                {
                    TakeSlotItemToMouseItem();
                }
                else if (_rightMouseDownTimer >= 15 && _rightMouseDownTimer % 6 == 0)
                {
                    TakeSlotItemToMouseItem();
                }

                _rightMouseDownTimer++;
            }
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
                if (Item.IsAir)
                    Item.SetDefaults();
                playSound = true;
            }
            else if (Main.mouseItem.IsAir && !Item.IsAir && Item.maxStack > 1)
            {
                Main.mouseItem = new Item(Item.type);
                Item.stack--;
                if (Item.IsAir)
                    Item.SetDefaults();
                playSound = true;
            }

            if (playSound)
                SoundEngine.PlaySound(SoundID.MenuTick);
        }

        /// <summary>
        /// 改原版的 <see cref="Main.cursorOverride"/>
        /// </summary>
        private void SetCursorOverride()
        {
            if (Item.IsAir)
            {
                return;
            }

            if (!Item.favorited && ItemSlot.ShiftInUse)
            {
                Main.cursorOverride = CursorOverrideID.ChestToInventory; // 快捷放回物品栏图标
            }

            if (Main.keyState.IsKeyDown(Main.FavoriteKey))
            {
                Main.cursorOverride = CursorOverrideID.FavoriteStar; // 收藏图标
                if (Main.drawingPlayerChat)
                {
                    Main.cursorOverride = CursorOverrideID.Magnifiers; // 放大镜图标 - 输入到聊天框
                }
            }

            void TryTrashCursorOverride()
            {
                if (Item.favorited)
                {
                    return;
                }

                Main.cursorOverride = Main.npcShop <= 0
                    ? CursorOverrideID.TrashCan
                    : CursorOverrideID.QuickSell; // 垃圾箱图标
            }

            if (ItemSlot.ControlInUse && ItemSlot.Options.DisableLeftShiftTrashCan && !ItemSlot.ShiftForcedOn)
            {
                TryTrashCursorOverride();
            }
            // 如果左Shift快速丢弃打开了，按原版物品栏的物品应该是丢弃，但是我们这应该算箱子物品，所以不丢弃
            //if (!ItemSlot.Options.DisableLeftShiftTrashCan && ItemSlot.ShiftInUse) {
            //    TryTrashCursorOverride();
            //}
        }

        /// <summary>
        /// 左键点击物品
        /// </summary>
        private void MouseClickSlot()
        {
            if (Main.LocalPlayer.ItemAnimationActive)
                return;

            bool result = false;

            if (Item.ModItem is IItemOverrideLeftClick iItemOverrideLeftClick)
                result |= iItemOverrideLeftClick.OverrideLeftClick(Items, 114514, Index);

            if (result)
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
                // 收藏图标
                case CursorOverrideID.FavoriteStar:
                    Item.favorited = !Item.favorited;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    return;
                // 垃圾箱图标
                case CursorOverrideID.TrashCan:
                case CursorOverrideID.QuickSell:
                    // 假装自己是一个物品栏物品
                    var temp = new Item[1];
                    temp[0] = Item;
                    ItemSlot.SellOrTrash(temp, ItemSlot.Context.InventoryItem, 0);
                    return;
                // 放回物品栏图标
                case CursorOverrideID.ChestToInventory:
                    Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item,
                        GetItemSettings.InventoryEntityToPlayerInventorySettings);
                    SoundEngine.PlaySound(SoundID.Grab);
                    return;
            }

            if (ItemSlot.ShiftInUse)
                return;

            // 常规单点
            if (Item.IsAir)
            {
                if (Main.mouseItem.IsAir)
                {
                    return;
                }

                Item = Main.mouseItem;
                Main.mouseItem = new Item();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            else
            {
                if (Main.mouseItem.IsAir)
                {
                    Main.mouseItem = Item;
                    Item = new Item();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                else
                {
                    // 同种物品
                    if (Main.mouseItem.type == Item.type)
                    {
                        if (Item.stack < Item.maxStack)
                        {
                            if (Item.stack + Main.mouseItem.stack <= Item.maxStack)
                            {
                                Item.stack += Main.mouseItem.stack;
                                Main.mouseItem.SetDefaults();
                            }
                            else
                            {
                                Main.mouseItem.stack -= Item.maxStack - Item.stack;
                                Item.stack = Item.maxStack;
                            }
                        }
                        else if (Main.mouseItem.stack < Main.mouseItem.maxStack)
                        {
                            if (Main.mouseItem.stack + Item.stack <= Main.mouseItem.maxStack)
                            {
                                Main.mouseItem.stack += Item.stack;
                                Item.SetDefaults();
                            }
                            else
                            {
                                Item.stack -= Main.mouseItem.maxStack - Main.mouseItem.stack;
                                Main.mouseItem.stack = Main.mouseItem.maxStack;
                            }
                        }
                        else
                        {
                            (Main.mouseItem, Item) = (Item, Main.mouseItem);
                        }
                    }
                    else
                    {
                        (Item, Main.mouseItem) = (Main.mouseItem, Item);
                    }

                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 pos = dimensions.Position();
            Vector2 size = dimensions.Size();
            Color borderColor = Item.favorited && !Item.IsAir ? UIColor.ItemSlotBorderFav : UIColor.ItemSlotBorder;
            Color background = Item.favorited && !Item.IsAir ? UIColor.ItemSlotBgFav : UIColor.ItemSlotBG;

            PixelShader.RoundedRectangle(pos, size, 12, background, 2, borderColor);

            if (Item.IsAir)
                return;
            if (IsMouseHovering)
            {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item.Clone();
                SetCursorOverride();
            }

            DrawItemIcon(sb, Item, Color.White, dimensions);
            if (Item.stack <= 1)
            {
                return;
            }

            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(Item.stack.ToString()) * 0.75f;
            Vector2 textPos = pos + new Vector2(52 * 0.18f, (52 - textSize.Y) * 0.9f);
            TrUtils.DrawBorderString(sb, Item.stack.ToString(), textPos, Color.White, 0.75f);
            // 这段是直接绘制文字，不带边框的，留到这里防止忘了咋写。
            /*DynamicSpriteFontExtensionMethods.DrawString(
                    sb,
                    FontAssets.ItemStack.Value,
                    Item.stack.ToString(),
                    textPos,
                    Color.White,
                    0f,
                    new Vector2(0),
                    0.75f, 0, 0f);*/
        }

        public static void DrawItemIcon(SpriteBatch sb, Item item, Color lightColor, CalculatedStyle dimensions,
            float maxSize = 32f)
        {
            Main.instance.LoadItem(item.type);
            Texture2D texture2D = TextureAssets.Item[item.type].Value;
            Rectangle frame = Main.itemAnimations[item.type] is null
                ? texture2D.Frame()
                : Main.itemAnimations[item.type].GetFrame(texture2D);
            float size = frame.Width > maxSize || frame.Height > maxSize
                ? frame.Width > frame.Height ? maxSize / frame.Width : maxSize / frame.Height
                : 1f;
            Vector2 position = dimensions.Center() - frame.Size() * size / 2f;
            Vector2 origin = dimensions.Center();
            if (ItemLoader.PreDrawInInventory(item, sb, position, frame, item.GetAlpha(lightColor),
                    item.GetColor(lightColor), origin, size))
            {
                sb.Draw(texture2D, position, frame, item.GetAlpha(lightColor), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);
                if (item.color != Color.Transparent)
                    sb.Draw(texture2D, position, frame, item.GetColor(lightColor), 0f, Vector2.Zero, size,
                        SpriteEffects.None, 0f);
            }

            ItemLoader.PostDrawInInventory(item, sb, position, frame, item.GetAlpha(lightColor),
                item.GetColor(lightColor), origin, size);
        }

        public static void DrawItemIcon(SpriteBatch sb, Texture2D texture2D, Color color, CalculatedStyle dimensions,
            float maxSize = 32f)
        {
            Vector2 size = texture2D.Size();
            float scale = size.X > maxSize || size.Y > maxSize
                ? size.X > size.Y ? maxSize / size.X : maxSize / size.Y
                : 1f;
            Vector2 position = dimensions.Center();
            Vector2 origin = size / 2;
            sb.Draw(texture2D, position, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public static void DrawItemIcon(SpriteBatch sb, Texture2D texture2D, Color color, Vector2 pos, Vector2 origin,
            float maxSize = 32f)
        {
            Vector2 size = texture2D.Size();
            float scale = size.X > maxSize || size.Y > maxSize
                ? size.X > size.Y ? maxSize / size.X : maxSize / size.Y
                : 1f;
            sb.Draw(texture2D, pos, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public static void OpenItemGlow(SpriteBatch sb)
        {
            Effect effect = EffectAssets.Transform.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.CurrentTechnique.Passes["EnchantedPass"].Apply();
            Main.instance.GraphicsDevice.Textures[1] = GetTexture("Enchanted").Value; // 传入调色板

            sb.ReBegin(effect, Main.UIScaleMatrix);
        }
    }
}