using ImproveGame.Common.ModHooks;
using ImproveGame.Common.Systems;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public class LiquidWand : SelectorItem, IItemOverrideHover
    {
        // 修改物块
        public override void PostModifyTiles(Player player, int minI, int minJ, int maxI, int maxJ)
        {
            for (int j = minJ; j <= maxJ; j++)
            {
                for (int i = minI; i <= maxI; i++)
                {
                    WorldGen.SquareTileFrame(i, j, false);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.sendWater(i, j);
                    else
                        Liquid.AddWater(i, j);
                }
            }
        }

        // 修改 Box 颜色
        public override Color ModifyColor(bool cancelled)
        {
            if (cancelled)
                return base.ModifyColor(cancelled);
            return WandSystem.LiquidMode switch
            {
                LiquidID.Lava => new(253, 32, 3),
                LiquidID.Honey => new(255, 156, 12),
                _ => new(9, 61, 191),
            };
        }

        public override bool IsNeedKill() => !Main.mouseLeft;

        // 修改选择的方块
        public override bool ModifySelectedTiles(Player player, int i, int j)
        {
            Tile t = Main.tile[i, j];
            // 吸收模式
            if (WandSystem.AbsorptionMode)
            {
                if (t.LiquidAmount > 0)
                {
                    UISystem.Instance.LiquidWandGUI.TryChangeLiquidAmount((byte)t.LiquidType, ref t.LiquidAmount, true);
                }
            }
            // 放置模式
            else if (!t.HasTile || !WorldGen.SolidTile(new Point(i, j)))
            {
                // 原来没有液体的
                if (t.LiquidAmount == 0)
                {
                    int oldType = t.LiquidType;
                    t.LiquidType = WandSystem.LiquidMode; // 设置成相应的液体
                    UISystem.Instance.LiquidWandGUI.TryChangeLiquidAmount((byte)t.LiquidType, ref t.LiquidAmount, false);
                    // 还是没有液体，设置回来（虽然我不知道有啥用）
                    if (t.LiquidAmount == 0)
                    {
                        t.LiquidType = oldType;
                        return true;
                    }
                }
                else
                {
                    UISystem.Instance.LiquidWandGUI.TryChangeLiquidAmount((byte)t.LiquidType, ref t.LiquidAmount, false);
                }
            }
            return true;
        }

        [CloneByReference]
        internal float Water = 0f;
        [CloneByReference]
        internal float Lava = 0f;
        [CloneByReference]
        internal float Honey = 0f;

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(Water)] = Water;
            tag[nameof(Lava)] = Lava;
            tag[nameof(Honey)] = Honey;
        }

        public override void LoadData(TagCompound tag)
        {
            tag.TryGet(nameof(Water), out Water);
            tag.TryGet(nameof(Lava), out Lava);
            tag.TryGet(nameof(Honey), out Honey);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Water);
            writer.Write(Lava);
            writer.Write(Honey);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Water = reader.ReadSingle();
            Lava = reader.ReadSingle();
            Honey = reader.ReadSingle();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void SetItemDefaults()
        {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.mana = 20;

            SelectRange = new(15, 10);
        }

        public override bool StartUseItem(Player player)
        {
            if (player.altFunctionUse == 0)
            {
                MyUtils.ItemRotation(player);
            }
            else if (player.altFunctionUse == 2)
            {
                return false;
            }

            return base.StartUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                UISystem.Instance.LiquidWandGUI.CurrentSlot = player.selectedItem;
                // 还在用物品的时候不能打开UI
                if (player.mouseInterface || player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 || Main.HoveringOverAnNPC || player.talkNPC != -1)
                {
                    return;
                }
                if (!LiquidWandGUI.Visible)
                {
                    UISystem.Instance.LiquidWandGUI.Open();
                }
                else
                {
                    UISystem.Instance.LiquidWandGUI.Close();
                }
            }
        }

        public bool ItemInInventory;

        public bool OverrideHover(Item[] inventory, int context, int slot)
        {
            if (context == ItemSlot.Context.InventoryItem)
            {
                ItemInInventory = true;
                if (Main.mouseMiddle && Main.mouseMiddleRelease)
                {
                    if (!LiquidWandGUI.Visible)
                    {
                        UISystem.Instance.LiquidWandGUI.Open(slot);
                    }
                    else
                    {
                        UISystem.Instance.LiquidWandGUI.Close();
                    }
                }
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 决定文本显示的是“开启”还是“关闭”
            if (ItemInInventory)
            {
                string tooltip = MyUtils.GetText("Tips.LiquidWandOn");
                if (LiquidWandGUI.Visible)
                {
                    tooltip = MyUtils.GetText("Tips.LiquidWandOff");
                }

                tooltips.Add(new(Mod, "LiquidWand", tooltip) { OverrideColor = Color.LightGreen });
            }
            ItemInInventory = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddRecipeGroup(RecipeSystem.ShadowGroup, 18)
                .AddRecipeGroup(RecipeSystem.GoldGroup, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
