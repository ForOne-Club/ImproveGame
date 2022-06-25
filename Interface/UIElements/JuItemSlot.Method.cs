using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements
{
    partial class JuItemSlot
    {
        private static Texture2D Back => TextureAssets.InventoryBack.Value;
        private static Texture2D Back10 => TextureAssets.InventoryBack10.Value;
        private Texture2D backgroundTexture2D => Item.favorited ? Back10 : Back;
        public Item[] SuperVault;
        public int index;
        public Item Item {
            get => SuperVault[index];
            set => SuperVault[index] = value;
        }
        public UIText text;
        private int RightMouseTimer = -1;

        /// <summary>
        /// 右键持续按住物品，向鼠标物品堆叠数量
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
        /// 改原版的<see cref="Main.cursorOverride"/>
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
                        Main.cursorOverride = 6; // 垃圾箱图标
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
        private void LeftClickItem() {
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

                Main.instance.LoadItem(Item.type);
                var ItemTexture2D = TextureAssets.Item[Item.type];

                Rectangle rectangle;
                if (Main.itemAnimations[Item.type] == null)
                    rectangle = ItemTexture2D.Frame(1, 1, 0, 0);
                else
                    rectangle = Main.itemAnimations[Item.type].GetFrame(ItemTexture2D.Value);

                float size = rectangle.Width > ItemSize || rectangle.Height > ItemSize ?
                    rectangle.Width > rectangle.Height ? ItemSize / rectangle.Width : ItemSize / rectangle.Height :
                    1f;

                sb.Draw(ItemTexture2D.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), Item.GetAlpha(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);
                sb.Draw(ItemTexture2D.Value, dimensions.Center() - rectangle.Size() * size / 2f,
                    new Rectangle?(rectangle), Item.GetColor(Color.White), 0f, Vector2.Zero, size,
                    SpriteEffects.None, 0f);

                if (Item.GetGlobalItem<GlobalItemData>().InventoryGlow) {
                    Item.GetGlobalItem<GlobalItemData>().InventoryGlow = false;
                    CloseItemGlow(sb);
                }
            }
        }

        public static void OpenItemGlow(SpriteBatch sb) {
            var rasterizerState = sb.GraphicsDevice.RasterizerState;
            var rectangle1 = sb.GraphicsDevice.ScissorRectangle;
            sb.End();
            sb.GraphicsDevice.RasterizerState = rasterizerState;
            sb.GraphicsDevice.ScissorRectangle = rectangle1;
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
            Color lerpColor;
            float time = Main.LocalPlayer.GetModPlayer<ImprovePlayer>().PlayerTimer;
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
            RasterizerState rasterizerState = sb.GraphicsDevice.RasterizerState;
            Rectangle rectangle1 = sb.GraphicsDevice.ScissorRectangle;
            sb.End();
            sb.GraphicsDevice.RasterizerState = rasterizerState;
            sb.GraphicsDevice.ScissorRectangle = rectangle1;
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
        }
    }
}
