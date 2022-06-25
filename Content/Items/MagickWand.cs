using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.Content.Items
{
    public class MagickWand : SelectorItem
    {
        public override bool ModifySelectedTiles(Player player, int i, int j) {
            SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
            MyUtils.BongBong(new Vector2(i, j) * 16f, 16, 16);
            if (Main.tile[i, j].WallType > 0 && BrustWandSystem.WallMode) {
                WorldGen.KillWall(i, j);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, i, j);
                player.statMana -= 1;
                if (player.statMana < 1) {
                    player.QuickMana();
                    if (player.statMana < 1) {
                        return false;
                    }
                }
            }
            if (player.statMana < 2)
                player.QuickMana();
            if (player.statMana >= 2) {
                if (BrustWandSystem.TileMode && Main.tile[i, j].HasTile && MyUtils.TryKillTile(i, j, player)) {
                    player.statMana -= 2;
                }
            }
            return true;
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
                if (BrustWandSystem.FixedMode) {
                    Item.useAnimation = (int)(18 * player.pickSpeed);
                    Item.useTime = (int)(18 * player.pickSpeed);
                    if (player.whoAmI == Main.myPlayer) {
                        Rectangle rect = GetKillRect(player);
                        SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
                        MyUtils.ForechTile(rect, (i, j) => {
                            if (Main.tile[i, j].WallType > 0 && BrustWandSystem.WallMode) {
                                if (player.statMana < 1)
                                    player.QuickMana();
                                if (player.statMana >= 1) {
                                    WorldGen.KillWall(i, j);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, i, j);
                                    player.statMana -= 1;
                                }
                            }
                            if (player.statMana < 2)
                                player.QuickMana();
                            if (player.statMana >= 2) {
                                if (BrustWandSystem.TileMode && Main.tile[i, j].HasTile && MyUtils.TryKillTile(i, j, player)) {
                                    player.statMana -= 2;
                                }
                            }
                            MyUtils.BongBong(new Vector2(i, j) * 16f, 16, 16);
                        });
                    }
                }
            }
            else if (player.altFunctionUse == 2) {
                return false;
            }

            return base.StartUseItem(player);
        }

        public override bool CanUseSelector(Player player) {
            return !BrustWandSystem.FixedMode;
        }

        public override void HoldItem(Player player) {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI) {
                if (BrustWandSystem.FixedMode) {
                    Box.NewBox(GetKillRect(player), Color.Red * 0.35f, Color.Red);
                }
                // 还在用物品的时候不能打开UI
                if (player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 || Main.HoveringOverAnNPC || player.talkNPC != -1) {
                    return;
                }
                if (!BrustGUI.Visible) {
                    BrustGUI.Open();
                }
                else {
                    BrustGUI.Close();
                }
            }
        }

        protected Rectangle GetKillRect(Player player) {
            Rectangle rect = new();
            Point playerCenter = player.Center.ToTileCoordinates();
            Point mousePosition = Main.MouseWorld.ToTileCoordinates();
            mousePosition = MyUtils.LimitRect(playerCenter, mousePosition, Player.tileRangeX + ExtraRange.X, Player.tileRangeY + ExtraRange.Y);
            rect.X = mousePosition.X - KillSize.X / 2;
            rect.Y = mousePosition.Y - KillSize.Y / 2;
            rect.Width = KillSize.X;
            rect.Height = KillSize.Y;
            return rect;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 18)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddIngredient(ItemID.Ruby, 1)
                .AddTile(TileID.WorkBenches).Register();
        }
    }
}
