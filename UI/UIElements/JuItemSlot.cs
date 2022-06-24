using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.UI.UIElements
{
    public class JuItemSlot : UIElement
    {
        private static Texture2D Back9 {
            get => TextureAssets.InventoryBack9.Value;
        }
        private static Texture2D Back10 {
            get => TextureAssets.InventoryBack10.Value;
        }
        private Asset<Texture2D> texture {
            get {
                Main.instance.LoadItem(item.type);
                return TextureAssets.Item[item.type];
            }
        }
        public Item[] SuperVault;
        public int index;
        public Item item {
            get {
                return SuperVault[index];
            }
            set {
                SuperVault[index] = value;
            }
        }

        public UIText text;

        public JuItemSlot(Item[] SuperVault, int index) {
            this.SuperVault = SuperVault;
            this.index = index;
            Width.Set(Back9.Width, 0f);
            Height.Set(Back9.Height, 0f);

            text = new UIText("") {
                VAlign = 0.8f
            };
            text.Left.Set(0, 0.2f);
            Append(text);

            UIText text2 = new UIText((index + 1).ToString(), 0.75f) {
                VAlign = 0.15f
            };
            text2.Left.Set(0, 0.15f);
            Append(text2);
        }

        // 左键点击事件
        public override void MouseDown(UIMouseEvent evt) {
            Player player = Main.LocalPlayer;
            if (Main.mouseItem.IsAir && !item.IsAir) {
                // Ctrl 放入垃圾桶
                if (Main.keyState.IsKeyDown(Keys.LeftControl) || PlayerInput.GetPressedKeys().Contains(Keys.RightControl)) {
                    player.trashItem = item;
                    item = new Item();
                }
                else {
                    Main.mouseItem = item;
                    item = new Item();
                }
            }
            else if (item.IsAir && !Main.mouseItem.IsAir) {
                item = Main.mouseItem;
                Main.mouseItem = new Item();
            }
            else if (!Main.mouseItem.IsAir && !item.IsAir) {
                if (Main.mouseItem.type == item.type && item.stack < item.maxStack) {
                    if (item.stack + Main.mouseItem.stack < Main.mouseItem.maxStack) {
                        item.stack += Main.mouseItem.stack;
                        Main.mouseItem = new Item();
                    }
                    else {
                        Main.mouseItem.stack -= item.maxStack - item.stack;
                        item.stack = item.maxStack;
                    }
                }
                else {
                    Item mouseItem = Main.mouseItem;
                    Main.mouseItem = item;
                    item = mouseItem;
                }
            }
            item.favorited = false;
        }

        /// <summary>
        /// 右键持续按住物品，向鼠标物品堆叠数量
        /// </summary>
        public void RightMouseKeepPressItem() {
            if (Main.mouseItem.type == item.type && Main.mouseItem.stack < Main.mouseItem.maxStack) {
                Main.mouseItem.stack++;
                item.stack--;
            }
            else if (Main.mouseItem.IsAir && !item.IsAir && item.maxStack > 1) {
                Main.mouseItem = new Item(item.type, 1);
                item.stack--;
            }
            if (item.type != ItemID.None && item.stack < 1) {
                item = new Item();
            }
        }

        // 鼠标右键按住项目计时器，-1代表不进行计时。
        public int rightMouseDownTimer = -1;
        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        /// <param name="evt"></param>
        public override void RightMouseDown(UIMouseEvent evt) {
            rightMouseDownTimer = 0;
            RightMouseKeepPressItem();
        }

        /// <summary>
        /// 鼠标右键放开
        /// </summary>
        /// <param name="evt"></param>
        public override void RightMouseUp(UIMouseEvent evt) {
            rightMouseDownTimer = -1;
        }

        /// <summary>
        /// Update 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (rightMouseDownTimer >= 60) {
                RightMouseKeepPressItem();
            }
            else if (rightMouseDownTimer >= 30 && rightMouseDownTimer % 3 == 0) {
                RightMouseKeepPressItem();
            }
            else if (rightMouseDownTimer >= 15 && rightMouseDownTimer % 6 == 0) {
                RightMouseKeepPressItem();
            }
            if (rightMouseDownTimer != -1) {
                rightMouseDownTimer++;
            }
        }

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="sb"></param>
        protected override void DrawSelf(SpriteBatch sb) {
            // 按下 Ctrl 改变鼠标指针外观
            if (IsMouseHovering && !item.IsAir) {
                SetCursorOverride();
            }
            // 绘制背景框
            CalculatedStyle dimensions = GetDimensions();
            if (item.favorited) {
                sb.Draw(Back10, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None, 0f);
            }
            else {
                sb.Draw(Back9, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None, 0f);
            }

            // 绘制物品
            if (!item.IsAir) {
                if (item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                        DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
                    Color lerpColor;
                    float time = ImprovePlayer.G(Main.LocalPlayer).PlayerTimer;
                    if (time % 60f < 30) {
                        lerpColor = Color.Lerp(Color.White * 0.25f, Color.Transparent, (float)(time % 60f % 30 / 29));
                    }
                    else {
                        lerpColor = Color.Lerp(Color.Transparent, Color.White * 0.25f, (float)(time % 60f % 30 / 29));
                    }
                    MyAssets.ItemEffect.Parameters["uColor"].SetValue(lerpColor.ToVector4());
                    MyAssets.ItemEffect.CurrentTechnique.Passes["Test"].Apply();
                }
                Rectangle rectangle;
                if (Main.itemAnimations[item.type] == null) {
                    rectangle = texture.Frame(1, 1, 0, 0);
                }
                else {
                    rectangle = Main.itemAnimations[item.type].GetFrame(texture.Value);
                }
                float textureSize = 30f;
                float size = rectangle.Width > textureSize || rectangle.Height > textureSize ?
                    rectangle.Width > rectangle.Height ? textureSize / rectangle.Width : textureSize / rectangle.Height :
                    1f;
                sb.Draw(texture.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), item.GetAlpha(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);
                sb.Draw(texture.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), item.GetColor(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);

                if (item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    item.GetGlobalItem<GlobalItemData>().InventoryGlow = false;
                    sb.End();
                    sb.Begin(0, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                        DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
                }
            }
            // 物品信息
            if (IsMouseHovering) {
                Main.hoverItemName = item.Name;
                Main.HoverItem = item.Clone();
            }
            text.SetText(item.IsAir || item.stack <= 1 ? "" : item.stack.ToString(), 0.8f, false);
            text.Recalculate();
        }

        /// <summary>
        /// 改原版的<see cref="Main.cursorOverride"/>
        /// </summary>
        private void SetCursorOverride() {
            if (!item.IsAir) {
                if (!item.favorited && ItemSlot.ShiftInUse) {
                    Main.cursorOverride = 8; // 快捷放回物品栏图标
                }
                if (Main.keyState.IsKeyDown(Main.FavoriteKey)) {
                    Main.cursorOverride = 3; // 收藏图标
                    if (Main.drawingPlayerChat) {
                        Main.cursorOverride = 2; // 放大镜图标 - 输入到聊天框
                    }
                }
                void TryTrashCursorOverride() {
                    if (!item.favorited) {
                        if (Main.npcShop > 0) {
                            Main.cursorOverride = 10; // 卖出图标
                        }
                        else {
                            Main.cursorOverride = 6; // 垃圾箱图标
                        }
                    }
                }
                if (ItemSlot.ControlInUse && ItemSlot.Options.DisableLeftShiftTrashCan && !ItemSlot.ShiftForcedOn) {
                    TryTrashCursorOverride();
                }
                if (!ItemSlot.Options.DisableLeftShiftTrashCan && ItemSlot.ShiftInUse) {
                    TryTrashCursorOverride();
                }
            }
        }

        private void LeftClickItem() {
            // 放大镜图标 - 输入到聊天框
            if (Main.cursorOverride == 2) {
                if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(item), Vector2.One))
                    SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 收藏图标
            if (Main.cursorOverride == 3) {
                item.favorited = !item.favorited;
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 垃圾箱图标
            if (Main.cursorOverride == 6) {
                // 假装自己是一个物品栏物品
                
                return;
            }

            // 放回物品栏图标
            if (Main.cursorOverride == 8) {
                item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            // 常规单点
            if (Main.mouseItem is not null && CanPlaceItem()) {
                // type不同直接切换吧
                if (Item.type != Main.mouseItem.type || Item.prefix != Main.mouseItem.prefix) {
                    SwapItem(ref Main.mouseItem);
                    SoundEngine.PlaySound(SoundID.Grab);
                    return;
                }
                // type相同，里面的能堆叠，放进去
                if (!Item.IsAir && ItemLoader.CanStack(Item, Main.mouseItem)) {
                    int stackAvailable = Item.maxStack - Item.stack;
                    int stackAddition = Math.Min(Main.mouseItem.stack, stackAvailable);
                    Main.mouseItem.stack -= stackAddition;
                    Item.stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
        }
    }
}
