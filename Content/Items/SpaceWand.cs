using Humanizer;
using ImproveGame.Common.ModSystems;
using ImproveGame.Common.ModSystems.MarqueeSystem;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;

public enum PlaceType : byte
{
    Platform, Soild, Rope, Rail, GrassSeed, PlantPot
}

public class SpaceWand : ModItem, IMarqueeItem
{
    #region IMarqueeItem 实现

    private bool _shouldDrawing;
    private Rectangle _marquee;
    private Color _backgroundColor;
    private Color _borderColor;

    bool IMarqueeItem.ShouldDrawing => _shouldDrawing;
    Rectangle IMarqueeItem.Marquee => _marquee;
    Color IMarqueeItem.BorderColor => _borderColor;
    Color IMarqueeItem.BackgroundColor => _backgroundColor;

    void IMarqueeItem.PostDraw(Rectangle marquee, Color backgroundColor, Color borderColor)
    {
        Vector2 size = new Vector2(marquee.Width >> 4, marquee.Height >> 4);
        DrawString(Main.MouseScreen + new Vector2(18f), $"{size.MaxXY()}", Color.White, borderColor);
    }

    #endregion

    #region Item 基础设置
    public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.SpaceWand;

    public override bool AltFunctionUse(Player player) => true;

    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.SetBaseValues(30, 30, ItemRarityID.Red, Item.sellPrice(0, 0, 50));
        Item.SetUseValues(ItemUseStyleID.Shoot, SoundID.Item1, 15, 15, mana: 20);
        Item.channel = true;
    }

    #endregion

    public PlaceType PlaceType;
    public int[] GrassSeeds = new int[] { 2, 23, 60, 70, 199, 109, 82 };

    public Vector2 StartingPoint;
    public static Vector2 MousePosition => Main.MouseWorld.ToTileCoordinates().ToVector2() * 16f;

    public override void UpdateInventory(Player player)
    {
        _shouldDrawing = false;
    }

    public override bool CanUseItem(Player player)
    {
        if (!player.noBuilding)
        {
            // 右键
            if (player.altFunctionUse == 2)
            {
                if (SpaceWandGUI.Visible && UISystem.Instance.SpaceWandGUI.timer.AnyOpen)
                {
                    UISystem.Instance.SpaceWandGUI.Close();
                }
                else
                {
                    UISystem.Instance.SpaceWandGUI.Open(this);
                }
            }
            else
            {
                ItemCount(player.inventory, GetConditions(), out int count);

                if (count > 0)
                {
                    CanPlaceTiles = true;
                    ItemRotation(player);
                    StartingPoint = Main.MouseWorld.ToTileCoordinates().ToVector2() * 16f;
                    RefreshMarquee(player);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 能否放置
    /// </summary>
    private bool CanPlaceTiles;
    public override bool? UseItem(Player player)
    {
        UseItem_PreUpdate(player);

        if (!Main.mouseLeft)
        {
            player.itemAnimation = 0;
            TryPlaceTiles(player);
            return true;
        }
        player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation - player.direction * MathHelper.ToRadians(90f));

        return false;
    }

    public void UseItem_PreUpdate(Player player)
    {
        _shouldDrawing = true;

        RefreshMarquee(player);

        player.itemAnimation = player.itemAnimationMax;

        ItemRotation(player);

        if (Main.dedServ || Main.myPlayer != player.whoAmI)
        {
            return;
        }

        if (Main.mouseRight && CanPlaceTiles)
        {
            CanPlaceTiles = false;
            CombatText.NewText(player.getRect(), new Color(250, 40, 80), GetText("CombatText.Item.SpaceWand_Cancel"));
        }

        Color color = CanPlaceTiles ? new Color(135, 0, 180) : new Color(250, 40, 80);
        _borderColor = color;
        _backgroundColor = color * 0.35f;
    }

    // 学单词
    // Judge v: 法官、判断、判定。n: 法官、裁判、裁判员

    // 学单词
    // Enough: 足够的
    // Enough!: 够了！

    public void TryPlaceTiles(Player player)
    {
        // 放置平台
        if (!CanPlaceTiles || player.whoAmI != Main.myPlayer && Main.dedServ)
        {
            return;
        }
        bool playSound = false;
        Rectangle platfromRect = _marquee;
        platfromRect.X >>= 4;
        platfromRect.Y >>= 4;
        platfromRect.Width >>= 4;
        platfromRect.Height >>= 4;

        ForeachTile(platfromRect, (int x, int y) =>
        {
            int oneIndex = EnoughItem(player, GetConditions());
            if (oneIndex > -1)
            {
                Item item = player.inventory[oneIndex];

                if (!TileLoader.CanPlace(x, y, item.createTile))
                {
                    return;
                }

                if (GrassSeeds.Contains(item.createTile))
                {
                    if (WorldGen.PlaceTile(x, y, item.createTile, true, false, player.whoAmI, item.placeStyle))
                    {
                        playSound = true;
                        PickItemInInventory(player, GetConditions(), true, out _);
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
                                PickItemInInventory(player, GetConditions(), true, out _);
                            }
                            else
                            {
                                // 尝试破坏
                                TryKillTile(x, y, player);
                                if (!Main.tile[x, y].HasTile && WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                                {
                                    playSound = true;
                                    PickItemInInventory(player, GetConditions(), true, out _);
                                }
                            }
                        }
                    }
                    else if (WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                    {
                        playSound = true;
                        PickItemInInventory(player, GetConditions(), true, out _);
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

    /// <summary>
    /// 刷新选框
    /// </summary>
    public void RefreshMarquee(Player player)
    {
        int layingQuantity = Main.netMode == NetmodeID.MultiplayerClient ? 244 : 500;

        int layingLength = 16 * (
            ItemCount(player.inventory, GetConditions(), out int tileCount) ?
            layingQuantity : Math.Min(layingQuantity, tileCount));

        Vector2 startingPoint = StartingPoint;
        Vector2 nowPoint = Vector2.Clamp(MousePosition,
            startingPoint - new Vector2(layingLength - 16f),
            startingPoint + new Vector2(layingLength - 16f));

        Point position;
        Point size = (startingPoint - nowPoint).Abs().ToPoint();

        if (size.X > size.Y)
        {
            position = new Vector2(Math.Min(startingPoint.X, nowPoint.X), startingPoint.Y).ToPoint();
            size.X = Math.Clamp(size.X + 16, 16, layingLength);
            size.Y = 16;
        }
        else
        {
            position = new Vector2(startingPoint.X, Math.Min(startingPoint.Y, nowPoint.Y)).ToPoint();
            size.X = 16;
            size.Y = Math.Clamp(size.Y + 16, 16, layingLength);
        }

        _marquee = new Rectangle(position.X, position.Y, size.X, size.Y);
    }

    /// <summary>
    /// 获取使用的条件
    /// </summary>
    public Func<Item, bool> GetConditions()
    {
        return PlaceType switch
        {
            PlaceType.Platform => item => item.createTile > -1 && TileID.Sets.Platforms[item.createTile],
            PlaceType.Soild => item => item.tileWand < 0 && item.createTile > -1 && !Main.tileSolidTop[item.createTile] && Main.tileSolid[item.createTile] && !GrassSeeds.Contains(item.createTile) && TileID.ClosedDoor != item.createTile,
            PlaceType.Rope => item => item.createTile > -1 && Main.tileRope[item.createTile],
            PlaceType.Rail => item => item.createTile == TileID.MinecartTrack,
            PlaceType.GrassSeed => item => GrassSeeds.Contains(item.createTile),
            PlaceType.PlantPot => item => item.createTile == TileID.PlanterBox,
            _ => _ => false,
        };
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("PlaceType", out byte placeType))
        {
            PlaceType = (PlaceType)placeType;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("PlaceType", (byte)PlaceType);
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
