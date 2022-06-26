using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ImproveGame.Content.Items
{
    public class LiquidWand : SelectorItem
    {
        public override bool ModifySelectedTiles(Player player, int i, int j) {
            if (WandSystem.AbsorptionMode) {
                
            }
            return true;
        }

        [CloneByReference]
        internal Item Bucket;

        public override void SaveData(TagCompound tag) {
            tag[nameof(Bucket)] = Bucket;
        }

        public override void LoadData(TagCompound tag) {
            Bucket = tag.Get<Item>(nameof(Bucket));
        }

        public override bool AltFunctionUse(Player player) => true;

        protected Point ExtraRange;
        protected Point KillSize;

        public override void SetItemDefaults() {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            SelectRange = new(20, 10);
            KillSize = new(5, 3);
            ExtraRange = new(5, 3);
        }

        public override bool StartUseItem(Player player) {
            if (player.altFunctionUse == 0) {
                MyUtils.ItemRotation(player);
            }
            else if (player.altFunctionUse == 2) {
                return false;
            }

            return base.StartUseItem(player);
        }

        public override void HoldItem(Player player) {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI) {
                // 还在用物品的时候不能打开UI
                if (player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 || Main.HoveringOverAnNPC || player.talkNPC != -1) {
                    return;
                }
                if (!BrustGUI.Visible) {
                    LiquidWandGUI.Open();
                }
                else {
                    LiquidWandGUI.Close();
                }
            }
        }

        /// <summary>
        /// 设置物品，用于UI和物品存储数据间的同步
        /// </summary>
        /// <param name="item">物品实例</param>
        internal void SetBucket(Item item, int inventoryIndex) {
            Bucket = item;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, inventoryIndex, Main.LocalPlayer.inventory[inventoryIndex].prefix);
            }
        }

        public bool ItemInInventory;

        // 现在还没开放OverrideHover的接口，只能On了
        // 等我的PR: https://github.com/tModLoader/tModLoader/pull/2620 被接受就可以改掉了
        public override void Load() {
            On.Terraria.UI.ItemSlot.OverrideHover_ItemArray_int_int += ApplyHover;
        }

        // 为了确保只有在物品栏才能用
        private void ApplyHover(On.Terraria.UI.ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            if (context == ItemSlot.Context.InventoryItem && inv[slot].type == ModContent.ItemType<LiquidWand>() && inv[slot].ModItem is LiquidWand) {
                (inv[slot].ModItem as LiquidWand).ItemInInventory = true;
                if (Main.mouseMiddle && Main.mouseMiddleRelease) {
                    if (!LiquidWandGUI.Visible) {
                        LiquidWandGUI.Open(slot);
                    }
                    else {
                        LiquidWandGUI.Close();
                    }
                }
            }
            orig.Invoke(inv, context, slot);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            // 决定文本显示的是“开启”还是“关闭”
            if (ItemInInventory) {
                string tooltip = MyUtils.GetText("Tips.LiquidWandOn");
                if (ArchitectureGUI.Visible) {
                    tooltip = MyUtils.GetText("Tips.LiquidWandOff");
                }

                tooltips.Add(new(Mod, "LiquidWand", tooltip) { OverrideColor = Color.LightGreen });
            }
            ItemInInventory = false;
        }

        //public override void AddRecipes() {
        //    CreateRecipe()
        //        .AddTile(TileID.WorkBenches).Register();
        //}
    }
}
