using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements
{
    /// <summary>
    /// 一个仿原版制作的物品UI格，由于是单独的所以应该适配
    /// </summary>
    public class ModItemSlot : UIElement
    {
        /// <summary>
        /// 无物品时显示的贴图
        /// </summary>
        private readonly Asset<Texture2D> _emptyTexture;
        public float emptyTextureScale = 1f;
        public float emptyTextureOpacity = 0.5f;
        private readonly Func<string> _emptyText; // 无物品的悬停文本

        public Item Item;
        public float Scale = 1f;

        /// <summary>
        /// 该槽位内的饰品/装备是否可在被右键时自动装备
        /// </summary>
        public bool AllowSwapEquip;
        /// <summary>
        /// 该槽位内的物品可否Alt键收藏
        /// </summary>
        public bool AllowFavorite;

        /// <summary>
        /// 物品槽UI元件
        /// </summary>
        /// <param name="scale">物品在槽内显示的大小，0.85是游戏内物品栏的大小</param>
        /// <param name="emptyTexturePath">当槽内无物品时，显示的贴图</param>
        /// <param name="emptyText">当槽内无物品时，悬停显示的文本</param>
        public ModItemSlot(float scale = 0.85f, string emptyTexturePath = null, Func<string> emptyText = null) {
            Item = new Item();
            Item.SetDefaults();
            Scale = scale;
            AllowSwapEquip = false;
            AllowFavorite = true;
            if (emptyTexturePath is not null && ModContent.HasAsset(emptyTexturePath)) {
                _emptyTexture = ModContent.Request<Texture2D>(emptyTexturePath);
            }
            _emptyText = emptyText;
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
                    if (AllowFavorite) {
                        Main.cursorOverride = 3; // 收藏图标
                    }
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
                            Main.cursorOverride = 6; // 垃圾箱图标
                        }
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

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            if (Item is null) {
                Item = new Item();
                Item.SetDefaults();
                ItemChange();
            }

            int lastStack = Item.stack;
            int lastType = Item.type;
            // 我把右键长按放在这里执行了
            if (IsMouseHovering) {
                SetCursorOverride();
                // 伪装，然后进行原版右键尝试
                // 千万不要伪装成箱子，因为那样多人会传同步信息，然后理所当然得出Bug
                if (Item is not null && !Item.IsAir) {
                    ItemSlot.RightClick(ref Item, AllowSwapEquip ? ItemSlot.Context.InventoryItem : ItemSlot.Context.CreativeSacrifice);
                }
                DrawText();
            }
            if (lastStack != Item.stack || lastType != Item.type) {
                ItemChange();
            }

            Vector2 origin = GetDimensions().Position();
            // 这里设置inventoryScale原版也是这么干的
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = Scale;

            // 假装自己是一个物品栏物品拿去绘制
            var temp = new Item[11];
            int context = ItemSlot.Context.InventoryItem;
            temp[10] = Item;
            ItemSlot.Draw(Main.spriteBatch, temp, context, 10, origin);

            // 空物品的话显示空贴图
            if (Item.IsAir) {
                if (_emptyText is not null && IsMouseHovering && Main.mouseItem.IsAir) {
                    Main.instance.MouseText(_emptyText.Invoke());
                }
                if (_emptyTexture is not null) {
                    origin = _emptyTexture.Size() / 2f;
                    Main.spriteBatch.Draw(_emptyTexture.Value, GetDimensions().Center(), null, Color.White * emptyTextureOpacity, 0f, origin, emptyTextureScale, SpriteEffects.None, 0f);
                }
            }

            Main.inventoryScale = oldScale;
        }

        /// <summary>
        /// 修改MouseText的
        /// </summary>
        public void DrawText() {
            if (!Item.IsAir && (Main.mouseItem is null || Main.mouseItem.IsAir)) {
                Main.HoverItem = Item.Clone();
                Main.instance.MouseText(string.Empty);
            }
        }

        public void LeftClickItem(ref Item placeItem) {
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
            if (Main.cursorOverride == 6) {
                // 假装自己是一个物品栏物品
                var temp = new Item[1];
                temp[0] = Item;
                SellOrTrash(temp, ItemSlot.Context.InventoryItem, 0);
                return;
            }

            // 放回物品栏图标
            if (Main.cursorOverride == 8) {
                Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            // 常规单点
            if (placeItem is not null && CanPlaceItem(placeItem)) {
                byte placeMode = MyUtils.CanPlaceInSlot(Item, placeItem);

                // type不同直接切换吧
                if (placeMode == 1) {
                    SwapItem(ref placeItem);
                    SoundEngine.PlaySound(SoundID.Grab);
                    return;
                }
                // type相同，里面的能堆叠，放进去
                if (placeMode == 2) {
                    int stackAvailable = Item.maxStack - Item.stack;
                    int stackAddition = Math.Min(placeItem.stack, stackAvailable);
                    placeItem.stack -= stackAddition;
                    Item.stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
        }

        public override void MouseDown(UIMouseEvent evt) {
            base.MouseDown(evt);

            if (!Main.LocalPlayer.ItemTimeIsZero || Main.LocalPlayer.itemAnimation != 0) {
                return;
            }

            if (Item is null) {
                Item = new Item();
                Item.SetDefaults();
                ItemChange();
            }

            int lastStack = Item.stack;
            int lastType = Item.type;
            int lastPrefix = Item.prefix;

            SetCursorOverride(); // Click在Update执行，因此必须在这里设置一次
            LeftClickItem(ref Main.mouseItem);

            if (lastStack != Item.stack || lastType != Item.type || lastPrefix != Item.prefix) {
                ItemChange();
            }
        }

        /// <summary>
        /// 可以在这里写额外的物品放置判定，第一个Item是当前槽位存储物品，第二个Item是<see cref="Main.mouseItem">
        /// </summary>
        public Func<Item, Item, bool> OnCanPlaceItem;
        public bool CanPlaceItem(Item item) {
            bool canPlace = true;

            if (Item is null) {
                Item = new Item();
                Item.SetDefaults();
            }

            if (OnCanPlaceItem is not null) {
                canPlace = OnCanPlaceItem.Invoke(Item, item);
            }

            return canPlace;
        }

        /// <summary>
        /// 物品改变后执行，可以写保存之类的
        /// </summary>
        public Action<Item> OnItemChange;
        public virtual void ItemChange() {
            if (Item is null) {
                Item = new Item();
                Item.SetDefaults();
            }

            if (OnItemChange is not null) {
                OnItemChange.Invoke(Item);
            }
        }

        // 原版里这个是private的，已经发pr让tML把这个改成public了，不过还得等一个月，在那之前就先用这个吧（懒得反射了）
        public static void SellOrTrash(Item[] inv, int context, int slot) {
            Player player = Main.player[Main.myPlayer];
            if (inv[slot].type <= ItemID.None)
                return;

            if (Main.npcShop > 0 && !inv[slot].favorited) {
                Chest chest = Main.instance.shop[Main.npcShop];
                if (inv[slot].type < ItemID.CopperCoin || inv[slot].type > ItemID.PlatinumCoin && PlayerLoader.CanSellItem(player, Main.npc[player.talkNPC], chest.item, inv[slot])) {
                    if (player.SellItem(inv[slot])) {
                        ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(inv[slot], context, 15));
                        int soldItemIndex = chest.AddItemToShop(inv[slot]);
                        inv[slot].TurnToAir();
                        SoundEngine.PlaySound(SoundID.Coins);
                        Recipe.FindRecipes();
                        PlayerLoader.PostSellItem(player, Main.npc[player.talkNPC], chest.item, chest.item[soldItemIndex]);
                    }
                    else if (inv[slot].value == 0) {
                        ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(inv[slot], context, 15));
                        int soldItemIndex = chest.AddItemToShop(inv[slot]);
                        inv[slot].TurnToAir();
                        SoundEngine.PlaySound(SoundID.Grab);
                        Recipe.FindRecipes();
                        PlayerLoader.PostSellItem(player, Main.npc[player.talkNPC], chest.item, chest.item[soldItemIndex]);
                    }
                }
            }
            else if (!inv[slot].favorited) {
                SoundEngine.PlaySound(SoundID.Grab);
                player.trashItem = inv[slot].Clone();
                ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(player.trashItem, context, 6));
                inv[slot].TurnToAir();

                Recipe.FindRecipes();
            }
        }

        public void SwapItem(ref Item item) {
            Utils.Swap(ref item, ref Item);
        }

        public void Unload() {
            Item = null;
        }
    }
}
