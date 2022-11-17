using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public enum PlaceType : byte
    {
        platform, // 平台
        soild, // 方块
        rope, // 绳子
        rail, // 轨道
        grassSeed, // 草种
        plantPot // 种植盆
    }

    public class SpaceWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.SpaceWand;

        public PlaceType placeType;
        public int[] GrassSeed = new int[] { 2, 23, 60, 70, 199, 109, 82 };

        public override bool AltFunctionUse(Player player) => true;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.channel = true;

            Item.mana = 20;
        }

        private Point start;
        private Point end;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (SpaceWandGUI.Visible && UISystem.Instance.SpaceWandGUI.timer.AnyOpen)
                    UISystem.Instance.SpaceWandGUI.Close();
                else
                    UISystem.Instance.SpaceWandGUI.Open(this);
                return false;
            }
            TileCount(player.inventory, out int count);
            if (count < 1)
            {
                return false;
            }
            ItemRotation(player);
            start = Main.MouseWorld.ToTileCoordinates();
            CanPlaceTile = true;
            return true;
        }

        /// <summary>
        /// 能否放置
        /// </summary>
        private bool CanPlaceTile;
        public override bool? UseItem(Player player)
        {
            UseItem_PreUpdate(player);
            if (!Main.mouseLeft)
            {
                player.itemAnimation = 0;
                UseItem_LeftMouseUp(player);
                return true;
            }
            UseItem_PostUpdate(player);
            player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation - player.direction * MathHelper.ToRadians(90f));
            return false;
        }

        public void UseItem_PreUpdate(Player player)
        {
            player.itemAnimation = player.itemAnimationMax;
            ItemRotation(player);
            end = Main.MouseWorld.ToTileCoordinates();
        }

        public void UseItem_PostUpdate(Player player)
        {
            // 开启绘制
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                if (Main.mouseRight && CanPlaceTile)
                {
                    CanPlaceTile = false;
                    CombatText.NewText(player.getRect(), new Color(250, 40, 80), GetText("CombatText_Item.SpaceWand_Cancel"));
                }
                Color color;
                if (CanPlaceTile)
                    color = new Color(135, 0, 180);
                else
                    color = new Color(250, 40, 80);
                Rectangle rectangle = GetRectangle(player);
                Box.NewBox(this, () => !Main.mouseLeft, rectangle, color * 0.35f, color, GetTextDisplayMode(rectangle));
            }
        }

        public void UseItem_LeftMouseUp(Player player)
        {
            // 放置平台
            if (CanPlaceTile && player.whoAmI == Main.myPlayer && !Main.dedServ)
            {
                bool playSound = false;
                Rectangle platfromRect = GetRectangle(player);
                // 瓷砖处理
                ForeachTile(platfromRect, (x, y) =>
                {
                    int oneIndex = EnoughItem(player, GetCondition());
                    if (oneIndex > -1)
                    {
                        Item item = player.inventory[oneIndex];
                        // 是否允许放置瓷砖，TML 的一个判定
                        if (!TileLoader.CanPlace(x, y, item.createTile)) return;
                        // 草种（环境）
                        if (GrassSeed.Contains(item.createTile))
                        {
                            if (WorldGen.PlaceTile(x, y, item.createTile, true, false, player.whoAmI, item.placeStyle))
                            {
                                playSound = true;
                                PickItemInInventory(player, GetCondition(), true, out _);
                            }
                        }
                        else
                        {
                            if (Main.tile[x, y].HasTile)
                            {
                                if (player.TileReplacementEnabled)
                                {
                                    // 物品放置的瓷砖就是位置对应的瓷砖则无需替换
                                    if (!ValidTileForReplacement(item, x, y))
                                        return;

                                    // 有没有足够强大的镐子破坏瓷砖
                                    if (!player.HasEnoughPickPowerToHurtTile(x, y))
                                        return;

                                    // 开始替换
                                    if (WorldGen.ReplaceTile(x, y, (ushort)item.createTile, item.placeStyle))
                                    {
                                        playSound = true;
                                        PickItemInInventory(player, GetCondition(), true, out _);
                                    }
                                    else
                                    {
                                        // 尝试破坏
                                        TryKillTile(x, y, player);
                                        if (!Main.tile[x, y].HasTile && WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                                        {
                                            playSound = true;
                                            PickItemInInventory(player, GetCondition(), true, out _);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                                {
                                    playSound = true;
                                    PickItemInInventory(player, GetCondition(), true, out _);
                                }
                            }
                        }
                    }
                }, (x, y, width, height) =>
                {
                    if (playSound)
                        SoundEngine.PlaySound(SoundID.Dig);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TileSquare, player.whoAmI, -1, null, x, y, width, height);
                });
            }
        }

        public TextDisplayMode GetTextDisplayMode(Rectangle rectangle)
        {
            return rectangle.Width >= rectangle.Height ? TextDisplayMode.Width : TextDisplayMode.Height;
        }

        public bool NeedKillTile(Player player, Item item, int x, int y)
        {
            return Main.tile[x, y].HasTile && player.TileReplacementEnabled && !SameTile(x, y, item.createTile, item.placeStyle, (placeType == PlaceType.soild || placeType == PlaceType.rope) ? CheckType.Type : default);
        }

        public Rectangle GetRectangle(Player player)
        {
            int maxWidth = Main.netMode == NetmodeID.MultiplayerClient ? 244 : 500;
            // 平台数量
            if (!TileCount(player.inventory, out int count) || count > maxWidth)
            {
                count = maxWidth;
            }
            if (MathF.Abs(start.X - end.X) > MathF.Abs(start.Y - end.Y))
            {
                end = ModifySize(start, end, count, 1);
            }
            else
            {
                end = ModifySize(start, end, 1, count);
            }
            Rectangle rect = new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y),
                 (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1);
            return rect;
        }

        // 获取背包中 选中物块类型 的数量
        public bool TileCount(Item[] inventory, out int count)
        {
            return GetItemCount(inventory, GetCondition(), out count);
        }

        // 获取使用的条件
        public Func<Item, bool> GetCondition()
        {
            return placeType switch
            {
                PlaceType.platform => (item) => item.createTile > -1 && TileID.Sets.Platforms[item.createTile],
                PlaceType.soild => (item) => item.tileWand < 0 && item.createTile > -1 && !Main.tileSolidTop[item.createTile] && Main.tileSolid[item.createTile] && !GrassSeed.Contains(item.createTile),
                PlaceType.rope => (item) => item.createTile > -1 && Main.tileRope[item.createTile],
                PlaceType.rail => (item) => item.createTile == TileID.MinecartTrack,
                PlaceType.grassSeed => (item) => GrassSeed.Contains(item.createTile),
                PlaceType.plantPot => (item) => item.createTile == TileID.PlanterBox,
                _ => (item) => false,
            };
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("placeType", out byte _placeType)) placeType = (PlaceType)_placeType;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("placeType", (byte)placeType);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddRecipeGroup(RecipeSystem.AnyDemoniteBar, 8)
                .AddIngredient(ItemID.Amethyst, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
