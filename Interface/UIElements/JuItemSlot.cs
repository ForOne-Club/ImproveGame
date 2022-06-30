using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements
{
    public class JuItemSlot : UIElement
    {
        private readonly JuItemList ItemList;
        public Texture2D Background => Item.favorited ? TextureAssets.InventoryBack10.Value : TextureAssets.InventoryBack.Value;
        public int index;
        public Item Item { get => ItemList.items[index]; set => ItemList.items[index] = value; }
        private int RightMouseTimer = -1;

        public JuItemSlot(JuItemList ItemList, int index) {
            this.ItemList = ItemList;
            this.index = index;
            Width.Set(TextureAssets.InventoryBack.Value.Width, 0f);
            Height.Set(TextureAssets.InventoryBack.Value.Height, 0f);

            UIText text = new(string.Empty, 0.75f) {
                VAlign = 0.8f
            };
            text.Left.Set(0, 0.2f);
            text.OnUpdate += (uie) => {
                (uie as UIText).SetText(Item.IsAir || Item.stack <= 1 ? string.Empty : Item.stack.ToString());
                (uie as UIText).Recalculate();
            };
            Append(text);

            UIText text2 = new((index + 1).ToString(), 0.75f) {
                VAlign = 0.15f
            };
            text2.Left.Set(0, 0.15f);
            Append(text2);
        }

        public override void MouseDown(UIMouseEvent evt) {
            SetCursorOverride();
            MouseClickSlot();
        }

        public override void RightMouseDown(UIMouseEvent evt) {
            RightMouseTimer = 0;
            TakeSlotItemToMouseItem();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            // 右键长按物品持续拿出
            if (Main.mouseRight && IsMouseHovering) {
                if (RightMouseTimer >= 60) {
                    TakeSlotItemToMouseItem();
                }
                else if (RightMouseTimer >= 30 && RightMouseTimer % 3 == 0) {
                    TakeSlotItemToMouseItem();
                }
                else if (RightMouseTimer >= 15 && RightMouseTimer % 6 == 0) {
                    TakeSlotItemToMouseItem();
                }
                RightMouseTimer++;
            }
        }

        protected override void DrawSelf(SpriteBatch sb) {
            // 按下 Ctrl 改变鼠标指针外观
            if (IsMouseHovering && !Item.IsAir) {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item.Clone();
                SetCursorOverride();
            }
            // 绘制背景框
            CalculatedStyle dimensions = GetDimensions();
            sb.Draw(Background, dimensions.Position(), null, Color.White * 0.8f, 0f, Vector2.Zero, 1f, 0, 0f);

            DrawItem(sb, Item, dimensions);

            /*Vector2 position = dimensions.Position();
            string text = Item.IsAir || Item.stack <= 1 ? "" : Item.stack.ToString();
            Vector2 textSize = MyUtils.GetStringSize(text) * 0.8f;
            position.Y += this.Height() * 0.8f;
            position.X += this.Width() * 0.2f;
            MyUtils.DrawString(position, text, Color.White, Color.Black, 0.8f);*/
        }

        /// <summary>
        /// 拿物品槽内物品到鼠标物品上
        /// </summary>
        public void TakeSlotItemToMouseItem() {
            if (Main.mouseItem.type == Item.type && Main.mouseItem.stack < Main.mouseItem.maxStack) {
                Main.mouseItem.stack++;
                Item.stack--;
            }
            else if (Main.mouseItem.IsAir && !Item.IsAir && Item.maxStack > 1) {
                Main.mouseItem = new Item(Item.type, 1);
                Item.stack--;
            }
            if (Item.IsAir) {
                Item.SetDefaults(0);
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        /// <summary>
        /// 改原版的 <see cref="Main.cursorOverride"/>
        /// </summary>
        private void SetCursorOverride() {
            if (!Item.IsAir) {
                if (!Item.favorited && ItemSlot.ShiftInUse) {
                    Main.cursorOverride = 8; // 快捷放回物品栏图标
                }
                if (Main.keyState.IsKeyDown(Main.FavoriteKey)) {
                    Main.cursorOverride = 3; // 收藏图标
                    if (Main.drawingPlayerChat) {
                        Main.cursorOverride = 2; // 放大镜图标 - 输入到聊天框
                    }
                }
                void TryTrashCursorOverride() {
                    if (!Item.favorited) {
                        if (Main.npcShop <= 0)
                            Main.cursorOverride = 6; // 垃圾箱图标
                        else
                            Main.cursorOverride = 10;
                    }
                }
                if (ItemSlot.ControlInUse && ItemSlot.Options.DisableLeftShiftTrashCan && !ItemSlot.ShiftForcedOn) {
                    TryTrashCursorOverride();
                }
                // 如果左Shift快速丢弃打开了，按原版物品栏的物品应该是丢弃，但是我们这应该算箱子物品，所以不丢弃
                //if (!ItemSlot.Options.DisableLeftShiftTrashCan && ItemSlot.ShiftInUse) {
                //    TryTrashCursorOverride();
                //}
            }
        }

        /// <summary>
        /// 左键点击物品
        /// </summary>
        private void MouseClickSlot() {
            if (Main.LocalPlayer.ItemAnimationActive)
                return;

            // 放大镜图标 - 输入到聊天框
            if (Main.cursorOverride == 2) {
                if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(Item), Vector2.One))
                    SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 收藏图标
            if (Main.cursorOverride == 3) {
                Item.favorited = !Item.favorited;
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 垃圾箱图标
            if (Main.cursorOverride == 6 || Main.cursorOverride == 10) {
                // 假装自己是一个物品栏物品
                var temp = new Item[1];
                temp[0] = Item;
                ModItemSlot.SellOrTrash(temp, ItemSlot.Context.InventoryItem, 0);
                return;
            }

            // 放回物品栏图标
            if (Main.cursorOverride == 8) {
                Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (ItemSlot.ShiftInUse)
                return;

            // 常规单点
            if (Item.IsAir) {
                if (!Main.mouseItem.IsAir) {
                    Item = Main.mouseItem;
                    Main.mouseItem = new Item();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
            else {
                if (Main.mouseItem.IsAir) {
                    Main.mouseItem = Item;
                    Item = new Item();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                else {
                    // 同种物品
                    if (Main.mouseItem.type == Item.type) {
                        if (Item.stack < Item.maxStack) {
                            if (Item.stack + Main.mouseItem.stack <= Item.maxStack) {
                                Item.stack += Main.mouseItem.stack;
                                Main.mouseItem.SetDefaults(0);
                            }
                            else {
                                Main.mouseItem.stack -= Item.maxStack - Item.stack;
                                Item.stack = Item.maxStack;
                            }
                        }
                        else if (Main.mouseItem.stack < Main.mouseItem.maxStack) {
                            if (Main.mouseItem.stack + Item.stack <= Main.mouseItem.maxStack) {
                                Main.mouseItem.stack += Item.stack;
                                Item.SetDefaults(0);
                            }
                            else {
                                Item.stack -= Main.mouseItem.maxStack - Main.mouseItem.stack;
                                Main.mouseItem.stack = Main.mouseItem.maxStack;
                            }
                        }
                    }
                    else {
                        (Item, Main.mouseItem) = (Main.mouseItem, Item);
                    }
                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
        }

        public static void DrawItem(SpriteBatch sb, Item Item, CalculatedStyle dimensions, float ItemSize = 30f) {
            // 绘制物品
            if (!Item.IsAir) {
                if (Item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    OpenItemGlow(sb);
                }

                DrawItemInternal(sb, Item, Color.White, dimensions, ItemSize);

                if (Item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    Item.GetGlobalItem<GlobalItemData>().InventoryGlow = false;
                    CloseItemGlow(sb);
                }
            }
        }

        public static void DrawItemInternal(SpriteBatch sb, Item Item, Color lightColor, CalculatedStyle dimensions, float ItemSize = 30f) {
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

        public static void OpenItemGlow(SpriteBatch sb) {
            //var rasterizerState = sb.GraphicsDevice.RasterizerState;
            //var rectangle1 = sb.GraphicsDevice.ScissorRectangle;
            sb.End();
            //sb.GraphicsDevice.RasterizerState = rasterizerState;
            //sb.GraphicsDevice.ScissorRectangle = rectangle1;
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
            Color lerpColor;
            int milliSeconds = (int)Main.gameTimeCache.TotalGameTime.TotalMilliseconds;
            float time = milliSeconds * 0.05f;
            if (time % 60f < 30) {
                lerpColor = Color.Lerp(Color.White * 0.25f, Color.Transparent, (float)(time % 60f % 30 / 29));
            }
            else {
                lerpColor = Color.Lerp(Color.Transparent, Color.White * 0.25f, (float)(time % 60f % 30 / 29));
            }
            MyAssets.ItemEffect.Parameters["uColor"].SetValue(lerpColor.ToVector4());
            MyAssets.ItemEffect.CurrentTechnique.Passes["Test"].Apply();
        }

        public static void CloseItemGlow(SpriteBatch sb) {
            //RasterizerState rasterizerState = sb.GraphicsDevice.RasterizerState;
            //Rectangle rectangle1 = sb.GraphicsDevice.ScissorRectangle;
            sb.End();
            //sb.GraphicsDevice.RasterizerState = rasterizerState;
            //sb.GraphicsDevice.ScissorRectangle = rectangle1;
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }
    }
}
