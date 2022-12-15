using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.Content.Items
{
    public class MagickWand : SelectorItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.MagickWand;

        public override bool ModifySelectedTiles(Player player, int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
            BongBong(new Vector2(i, j) * 16f, 16, 16);
            if (Main.tile[i, j].WallType > 0 && WandSystem.WallMode)
            {
                WorldGen.KillWall(i, j);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, i, j);
                player.statMana -= 1;
                if (player.statMana < 1)
                {
                    player.QuickMana();
                    if (player.statMana < 1)
                    {
                        return false;
                    }
                }
            }
            if (player.statMana < 2)
                player.QuickMana();
            if (player.statMana >= 2)
            {
                if (WandSystem.TileMode && Main.tile[i, j].HasTile && TryKillTile(i, j, player))
                {
                    player.statMana -= 2;
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        protected Point ExtraRange;
        protected Point KillSize;

        public override bool IsNeedKill()
        {
            if (WandSystem.FixedMode)
                return false;
            else
                return !Main.mouseLeft;
        }

        public override void SetItemDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            SelectRange = new(20, 20);
            KillSize = new(5, 3);
            ExtraRange = new(5, 3);
        }

        public override bool StartUseItem(Player player)
        {
            if (player.altFunctionUse == 0)
            {
                ItemRotation(player);
                if (WandSystem.FixedMode)
                {
                    Item.useAnimation = (int)(18 * player.pickSpeed);
                    Item.useTime = (int)(18 * player.pickSpeed);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Rectangle Reactangle = GetRectangle(player);
                        SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
                        ForeachTile(Reactangle, (x, y) =>
                        {
                            if (Main.tile[x, y].WallType > 0 && WandSystem.WallMode)
                            {
                                if (player.statMana < 1)
                                    player.QuickMana();
                                if (player.statMana >= 1)
                                {
                                    WorldGen.KillWall(x, y);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, x, y);
                                    player.statMana -= 1;
                                }
                            }
                            if (player.statMana < 2)
                                player.QuickMana();
                            if (player.statMana >= 2)
                            {
                                if (WandSystem.TileMode && Main.tile[x, y].HasTile && TryKillTile(x, y, player))
                                {
                                    player.statMana -= 2;
                                }
                            }
                            BongBong(new Vector2(x, y) * 16f, 16, 16);
                        });
                    }
                }
            }
            else if (player.altFunctionUse == 2)
            {
                return false;
            }

            return base.StartUseItem(player);
        }

        public override bool CanUseSelector(Player player)
        {
            return !WandSystem.FixedMode;
        }

        public override void HoldItem(Player player)
        {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                if (WandSystem.FixedMode)
                {
                    Box.NewBox(this, () => !WandSystem.FixedMode, GetRectangle(player), Color.Red * 0.35f, Color.Red);
                }
                // 我给他移动到 CanUseItem 中
                // 还在用物品的时候不能打开UI (直接写在 CanUseItem 似乎就没有问题了)
                if (player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 || Main.HoveringOverAnNPC || player.talkNPC != -1)
                {
                    return;
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.noBuilding)
                return false;

            if (player.altFunctionUse == 2)
            {
                if (BrustGUI.Visible && UISystem.Instance.BrustGUI.Timer.AnyOpen)
                    UISystem.Instance.BrustGUI.Close();
                else
                    UISystem.Instance.BrustGUI.Open();
                return false;
            }

            return base.CanUseItem(player);
        }

        protected Rectangle GetRectangle(Player player)
        {
            Rectangle rect = new();
            Point playerCenter = player.Center.ToTileCoordinates();
            Point mousePosition = Main.MouseWorld.ToTileCoordinates();
            mousePosition = ModifySize(playerCenter, mousePosition, Player.tileRangeX + ExtraRange.X, Player.tileRangeY + ExtraRange.Y);
            rect.X = mousePosition.X - KillSize.X / 2;
            rect.Y = mousePosition.Y - KillSize.Y / 2;
            rect.Width = KillSize.X;
            rect.Height = KillSize.Y;
            return rect;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 18)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddIngredient(ItemID.Ruby, 1)
                .AddTile(TileID.WorkBenches).Register();
        }
    }
}
