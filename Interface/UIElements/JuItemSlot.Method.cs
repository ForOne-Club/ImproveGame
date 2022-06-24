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
        private static Texture2D Back9 => TextureAssets.InventoryBack9.Value;
        private static Texture2D Back10 => TextureAssets.InventoryBack10.Value;
        private Asset<Texture2D> ItemTexture2D {
            get {
                Main.instance.LoadItem(Item.type);
                return TextureAssets.Item[Item.type];
            }
        }
        public Item[] SuperVault;
        public int index;
        public Item Item {
            get => SuperVault[index];
            set => SuperVault[index] = value;
        }
        public UIText text;
        private int RightMouseTimer = -1;

        /// <summary>
        /// 改原版的<see cref="Main.cursorOverride"/>
        /// </summary>
        private void SetCursorOverride() {
            if (!Item.IsAir) {
                if (Main.keyState.IsKeyDown(Main.FavoriteKey)) {
                    Main.cursorOverride = 3; // 收藏图标
                    if (Main.drawingPlayerChat) {
                        Main.cursorOverride = 2; // 放大镜图标 - 输入到聊天框
                    }
                }
                void TryTrashCursorOverride() {
                    if (!Item.favorited) {
                        if (Main.npcShop > 0) {
                            Main.cursorOverride = 10; // 卖出图标
                        }
                        else {
                            Main.cursorOverride = 8; // 拿回背包图标
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
    }
}
