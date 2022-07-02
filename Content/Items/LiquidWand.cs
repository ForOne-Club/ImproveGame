﻿using ImproveGame.Common.ModHooks;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ImproveGame.Content.Items
{
    public class LiquidWand : SelectorItem, IItemOverrideHover
    {
        public override void PostModifyTiles(Player player, int minI, int minJ, int maxI, int maxJ) {
            for (int j = minJ; j <= maxJ; j++) {
                for (int i = minI; i <= maxI; i++) {
                    WorldGen.SquareTileFrame(i, j, false);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.sendWater(i, j);
                    else
                        Liquid.AddWater(i, j);
                }
            }
        }

        public override Color ModifyColor(bool cancelled) {
            if (cancelled)
                return base.ModifyColor(cancelled);
            return WandSystem.LiquidMode switch {
                LiquidID.Lava => new(253, 32, 3),
                LiquidID.Honey => new(255, 156, 12),
                _ => new(9, 61, 191),
            };
        }

        public override bool ModifySelectedTiles(Player player, int i, int j) {
            Tile t = Main.tile[i, j];
            // 吸收模式
            if (WandSystem.AbsorptionMode) {
                if (t.LiquidAmount > 0) {
                    UISystem.Instance.LiquidWandGUI.TryChangeLiquidAmount((byte)t.LiquidType, ref t.LiquidAmount, true);
                    if (!MyUtils.TryConsumeMana(player, 2))
                        return false;
                }
            }
            // 放置模式
            else if (!t.HasTile || !WorldGen.SolidTile(new Point(i, j))) {
                // 原来没有液体的
                if (t.LiquidAmount == 0) {
                    int oldType = t.LiquidType;
                    t.LiquidType = WandSystem.LiquidMode; // 设置成相应的液体
                    UISystem.Instance.LiquidWandGUI.TryChangeLiquidAmount((byte)t.LiquidType, ref t.LiquidAmount, false);
                    // 还是没有液体，设置回来（虽然我不知道有啥用）
                    if (t.LiquidAmount == 0) {
                        t.LiquidType = oldType;
                        return true;
                    }
                    int manaCost = t.LiquidAmount / 120; // 根据液体放置了多少分成两个阶段
                    if (t.LiquidType != LiquidID.Water && manaCost > 0)
                        manaCost++; // 熔岩和蜂蜜额外耗蓝
                    if (!MyUtils.TryConsumeMana(player, manaCost))
                        return false;
                }
                else {
                    byte liquidAmountPrev = t.LiquidAmount;
                    UISystem.Instance.LiquidWandGUI.TryChangeLiquidAmount((byte)t.LiquidType, ref t.LiquidAmount, false);
                    int manaCost = (t.LiquidAmount - liquidAmountPrev) / 120; // 根据液体放置了多少分成两个阶段
                    if (t.LiquidType != LiquidID.Water && manaCost > 0)
                        manaCost++; // 熔岩和蜂蜜额外耗蓝
                    if (!MyUtils.TryConsumeMana(player, manaCost))
                        return false;
                }
            }
            return true;
        }

        [CloneByReference]
        internal float Water;
        [CloneByReference]
        internal float Lava;
        [CloneByReference]
        internal float Honey;

        public override void SaveData(TagCompound tag) {
            tag[nameof(Water)] = Water;
            tag[nameof(Lava)] = Lava;
            tag[nameof(Honey)] = Honey;
        }

        public override void LoadData(TagCompound tag) {
            tag.TryGet(nameof(Water), out Water);
            tag.TryGet(nameof(Lava), out Lava);
            tag.TryGet(nameof(Honey), out Honey);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void SetItemDefaults() {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            SelectRange = new(15, 10);
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
                UISystem.Instance.LiquidWandGUI.CurrentSlot = player.selectedItem;
                // 还在用物品的时候不能打开UI
                if (player.mouseInterface || player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 || Main.HoveringOverAnNPC || player.talkNPC != -1) {
                    return;
                }
                if (!LiquidWandGUI.Visible) {
                    UISystem.Instance.LiquidWandGUI.Open();
                }
                else {
                    UISystem.Instance.LiquidWandGUI.Close();
                }
            }
        }

        public bool ItemInInventory;

        public bool OverrideHover(Item[] inventory, int context, int slot) {
            if (context == ItemSlot.Context.InventoryItem) {
                ItemInInventory = true;
                if (Main.mouseMiddle && Main.mouseMiddleRelease) {
                    if (!LiquidWandGUI.Visible) {
                        UISystem.Instance.LiquidWandGUI.Open(slot);
                    }
                    else {
                        UISystem.Instance.LiquidWandGUI.Close();
                    }
                }
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            // 决定文本显示的是“开启”还是“关闭”
            if (ItemInInventory) {
                string tooltip = MyUtils.GetText("Tips.LiquidWandOn");
                if (LiquidWandGUI.Visible) {
                    tooltip = MyUtils.GetText("Tips.LiquidWandOff");
                }

                tooltips.Add(new(Mod, "LiquidWand", tooltip) { OverrideColor = Color.LightGreen });
            }
            ItemInInventory = false;
        }

        public override void AddRecipes() {
            // 金锭
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddIngredient(ItemID.ShadowScale, 18)
                .AddIngredient(ItemID.GoldBar, 12)
                .Register();
            // 铂金锭
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddIngredient(ItemID.ShadowScale, 18)
                .AddIngredient(ItemID.PlatinumBar, 12)
                .Register();
            // 金锭
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddIngredient(ItemID.TissueSample, 18)
                .AddIngredient(ItemID.GoldBar, 12)
                .Register();
            // 铂金锭
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddIngredient(ItemID.TissueSample, 18)
                .AddIngredient(ItemID.PlatinumBar, 12)
                .Register();
        }
    }
}
