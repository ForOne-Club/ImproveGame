using ImproveGame.Common.Systems;
using ImproveGame.Entitys;
using ImproveGame.Interface.GUI;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public class SpaceWand : ModItem
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

        public PlaceType placeType;
        public int[] GrassSeed = new int[] { 2, 23, 60, 70, 199, 109 };

        // 准备加上一个纵向的
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems;
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
            Item.rare = ItemRarityID.Lime;
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
                UISystem.Instance.SpaceWandGUI.ToggleMode(this);
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
                int box = Box.NewBox(GetRectangle(player), color * 0.35f, color);
                DrawSystem.boxs[box].ShowWidth = true;
                DrawSystem.boxs[box].ShowHeight = true;
            }
        }

        public void UseItem_LeftMouseUp(Player player)
        {
            // 放置平台
            if (CanPlaceTile && player.whoAmI == Main.myPlayer && !Main.dedServ)
            {
                bool playSound = false;
                Rectangle platfromRect = GetRectangle(player);
                ForeachTile(platfromRect, (x, y) =>
                {
                    Func<Item, bool> condition = GetCondition();
                    int oneIndex = EnoughItem(player, condition);
                    if (oneIndex > -1)
                    {
                        Item item = player.inventory[oneIndex];
                        if (GrassSeed.Contains(item.createTile))
                        {
                            if (WorldGen.PlaceTile(x, y, item.createTile, true, false, player.whoAmI, item.placeStyle))
                            {
                                playSound = true;
                                PickItemInInventory(player, GetCondition(), true);
                            }
                        }
                        else
                        {
                            if (NeedKillTile(player, item, x, y))
                            {
                                TryKillTile(x, y, player);
                            }
                            if (!Main.tile[x, y].HasTile)
                            {
                                if (WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                                {
                                    playSound = true;
                                    PickItemInInventory(player, GetCondition(), true);
                                }
                            }
                        }
                    }
                }, (x, y, width, height) =>
                {
                    if (playSound) SoundEngine.PlaySound(SoundID.Dig);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.TileSquare, player.whoAmI, -1, null, x, y, width, height);
                    }
                });
            }
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
    }
}
