using ImproveGame.Common;
using ImproveGame.Common.Conditions;
using ImproveGame.Common.Configs;
using ImproveGame.Common.ModSystems;
using ImproveGame.Packets;
using ImproveGame.UI;
using ImproveGame.UIFramework;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.Content.Items
{
    public class MagickWand : SelectorItem
    {
        public bool WallMode;
        public bool TileMode;
        public bool ChestMode;

        public override bool ModifySelectedTiles(Player player, int i, int j)
        {
            // 单人模式下直接设置成WandSystem.TileMode，物品的LiquidMode是给多人用的
            if (Main.netMode is not NetmodeID.Server)
            {
                TileMode = WandSystem.TileMode;
                WallMode = WandSystem.WallMode;
                ChestMode = WandSystem.ChestMode;
            }

            if (UIConfigs.Instance.ExplosionEffect && Main.netMode is not NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
                BongBong(new Vector2(i, j) * 16f, 16, 16);
            }

            var tile = Main.tile[i, j];
            if (tile.WallType > 0 && WallMode)
            {
                WorldGen.KillWall(i, j);
            }

            if (TileMode && tile.HasTile)
            {
                TryKillTile(i, j, player);
                CheckChestDestroy(player, i, j);
            }

            return true;
        }

        private bool CheckChestDestroy(Player player, int i, int j)
        {
            var tile = Main.tile[i, j];
            // 先一步if判断再下一步，不要每次都FindChestByGuessing，性能损耗太大
            if (!TileID.Sets.IsAContainer[tile.TileType] || !ChestMode)
                return false;

            var origin = GetTileOrigin(i, j);
            int chestIndex = Chest.FindChest(origin.X, origin.Y);
            if (chestIndex == -1 || !Main.chest.IndexInRange(chestIndex))
                return false;

            var chest = Main.chest[chestIndex];
            if (Chest.IsLocked(chest.x, chest.y) || chest?.item is null)
            {
                return false;
            }

            // 爆物品
            for (int k = 0; k < chest.item.Length; k++)
                if (!chest.item[k].IsAir)
                    SpawnTileBreakItem(i, j, ref chest.item[k], "ChestBrokenFromBlastsWand");
            // 爆箱子
            TryKillTile(i, j, player);
            return true;
        }

        public override void PostModifyTiles(Player player, int minI, int minJ, int maxI, int maxJ)
        {
            var size = new Point(maxI - minI, maxJ - minJ).Abs();
            size.X += 2;
            size.Y += 2;
            var center = new Point(minI + size.X / 2, minJ + size.Y / 2).ToWorldCoordinates();
            var rect = new Rectangle(minI, minJ, size.X, size.Y);

            if (Main.netMode is NetmodeID.Server)
                NetMessage.SendTileSquare(-1, minI - 1, minJ - 1, size.X , size.Y);

            DoBoomPacket.Send(rect);
            PlaySoundPacket.SendSound(LegacySoundIDs.Item, center, style: 14);
        }

        public override bool AltFunctionUse(Player player) => true;

        protected Point ExtraRange;
        protected Point KillSize;

        public override bool IsNeedKill()
        {
            if (WandSystem.FixedMode)
                return false;
            return !Main.mouseLeft;
        }

        public override void SetItemDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 2, 0, 0);

            MaxTilesPerFrame = 100;
            SelectRange = new(20, 20);
            KillSize = new(5, 3);
            ExtraRange = new(5, 3);
            RunOnServer = true;
        }

        public override bool StartUseItem(Player player)
        {
            if (player.altFunctionUse == 0)
            {
                FixedModeAction(player);
            }
            else if (player.altFunctionUse == 2)
            {
                return false;
            }

            return base.StartUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            return 1f + (1f - player.pickSpeed);
        }

        public override bool? UseItem(Player player)
        {
            if (player.ItemAnimationJustStarted)
            {
                FixedModeAction(player);
            }

            return base.UseItem(player);
        }

        private void FixedModeAction(Player player)
        {
            if (player.whoAmI == Main.myPlayer && WandSystem.FixedMode)
            {
                Rectangle rectangle = GetRectangle(player);
                // 带同步的音效
                if (UIConfigs.Instance.ExplosionEffect)
                    PlaySoundPacket.PlaySound(LegacySoundIDs.Item, Main.MouseWorld, style: 14);
                ForeachTile(rectangle, (x, y) =>
                {
                    if (Main.tile[x, y].WallType > 0 && WandSystem.WallMode)
                    {
                        WorldGen.KillWall(x, y);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, x, y);
                    }

                    if (WandSystem.TileMode && Main.tile[x, y].HasTile)
                    {
                        TryKillTile(x, y, player);
                        ChestMode = WandSystem.ChestMode;
                        CheckChestDestroy(player, x, y);
                    }

                    if (UIConfigs.Instance.ExplosionEffect)
                        BongBong(new Vector2(x, y) * 16f, 16, 16);
                }, (x, y, wid, hei) => DoBoomPacket.Send(x, y, wid, hei)); // 同步爆炸特效
            }
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
                    GameRectangle.Create(this, () => !WandSystem.FixedMode, GetRectangle(player), Color.Red * 0.35f,
                        Color.Red);
                }

                // 我给他移动到 CanUseItem 中
                // 还在用物品的时候不能打开UI (直接写在 CanUseItem 似乎就没有问题了)
                /*if (player.itemAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease ||
                    Main.SmartInteractShowingGenuine || PlayerInput.LockGamepadTileUseButton || player.noThrow != 0 ||
                    Main.HoveringOverAnNPC || player.talkNPC != -1)
                {
                    return;
                }*/
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.noBuilding)
                return false;

            if (player.altFunctionUse == 2)
            {
                if (BurstGUI.Visible && UISystem.Instance.BurstGUI.Timer.AnyOpen)
                    UISystem.Instance.BurstGUI.Close();
                else
                    UISystem.Instance.BurstGUI.Open();
                return false;
            }

            return base.CanUseItem(player);
        }

        protected Rectangle GetRectangle(Player player)
        {
            Rectangle rect = new();
            Point playerCenter = player.Center.ToTileCoordinates();
            Point mousePosition = Main.MouseWorld.ToTileCoordinates();
            mousePosition = ModifySize(playerCenter, mousePosition, Player.tileRangeX + ExtraRange.X,
                Player.tileRangeY + ExtraRange.Y);
            rect.X = mousePosition.X - KillSize.X / 2;
            rect.Y = mousePosition.Y - KillSize.Y / 2;
            rect.Width = KillSize.X;
            rect.Height = KillSize.Y;
            return rect;
        }

        public override void NetSend(BinaryWriter writer)
        {
            WallMode = WandSystem.WallMode;
            TileMode = WandSystem.TileMode;
            ChestMode = WandSystem.ChestMode;
            writer.Write(new BitsByte(WallMode, TileMode, ChestMode));
        }

        public override void NetReceive(BinaryReader reader)
        {
            var bitsByte = (BitsByte)reader.ReadByte();
            WallMode = bitsByte[0];
            TileMode = bitsByte[1];
            ChestMode = bitsByte[2];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 18)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddIngredient(ItemID.Ruby, 1)
                .AddTile(TileID.WorkBenches)
                .AddCondition(ConfigCondition.AvailableMagickWandC)
                .Register();
        }
    }
}