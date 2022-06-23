using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

namespace ImproveGame.UI.UIElements
{
	/// <summary>
	/// 一个仿原版制作的物品UI格，由于是单独的所以应该适配
	/// </summary>
	public class ModItemSlot : UIElement
	{
		public Item Item;
		public float Scale = 1f;

		public ModItemSlot(float scale = 0.85f) {
			Item = new Item();
			Item.SetDefaults();
			Scale = scale;
        }

		private void SetCursorOverride() {
			if (!Item.IsAir) {
				if (ItemSlot.ShiftInUse) {
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

        protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (IsMouseHovering) {
				DrawText();
				SetCursorOverride();
				// 假装自己是旅途用来研究的物品，然后进行原版右键尝试
				// 为啥不伪装成物品栏物品？那样的话右键饰品就会切饰品了
				// 千万不要伪装成箱子，因为那样多人会传同步信息，然后理所当然得出Bug
				if (Item is not null && !Item.IsAir) {
					ItemSlot.RightClick(ref Item, ItemSlot.Context.CreativeSacrifice);
				}
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

			Main.inventoryScale = oldScale;
		}

		public void DrawText() {
			if (!Item.IsAir) {
				Main.HoverItem = Item.Clone();
				Main.instance.MouseText(string.Empty);
			}
		}

		public override void Click(UIMouseEvent evt) {
			base.Click(evt);

			if (Item is null) {
				Item = new Item();
				Item.SetDefaults();
			}

			SetCursorOverride(); // Click在Update执行，因此必须在这里设置一次

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

			if (Main.mouseItem is not null) {
				if (Item.type != Main.mouseItem.type || Item.prefix != Main.mouseItem.prefix) {
					SwapItem(ref Main.mouseItem);
					SoundEngine.PlaySound(SoundID.Grab);
					return;
				}
				if (!Item.IsAir && ItemLoader.CanStack(Item, Main.mouseItem)) {
					int stackAvailable = Item.maxStack - Item.stack;
					int stackAddition = Math.Min(Main.mouseItem.stack, stackAvailable);
					Main.mouseItem.stack -= stackAddition;
					Item.stack += stackAddition;
					SoundEngine.PlaySound(SoundID.Grab);
				}
			}
		}
		
		// 原版里这个是private的，我正在请求tML把这个改成public，在那之前就先用这个吧（懒得反射了）
		private static void SellOrTrash(Item[] inv, int context, int slot) {
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
