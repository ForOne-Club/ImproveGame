using ImproveGame.Common.ModSystems;
using ImproveGame.Entitys;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;

public enum PlaceType : byte
{
    Platform, Soild, Rope, Rail, GrassSeed, PlantPot
}

public class SpaceWand : ModItem
{
    public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.SpaceWand;

    public PlaceType PlaceType;
    public int[] GrassSeeds = new int[] { 2, 23, 60, 70, 199, 109, 82 };

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

    private Point _beginMousePos;
    private Point _endMousePos;

    public override bool CanUseItem(Player player)
    {
        if (player.noBuilding)
        {
            return false;
        }

        if (player.altFunctionUse == 2)
        {
            if (SpaceWandGUI.Visible && UISystem.Instance.SpaceWandGUI.timer.AnyOpen)
                UISystem.Instance.SpaceWandGUI.Close();
            else
                UISystem.Instance.SpaceWandGUI.Open(this);
            return false;
        }

        CountTheNumberOfTiles(player.inventory, out int count);

        if (count < 1)
        {
            return false;
        }
        ItemRotation(player);
        _beginMousePos = Main.MouseWorld.ToTileCoordinates();
        _canPlaceTiles = true;
        return true;
    }

    /// <summary>
    /// 能否放置
    /// </summary>
    private bool _canPlaceTiles;
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
        _endMousePos = Main.MouseWorld.ToTileCoordinates();
    }

    public void UseItem_PostUpdate(Player player)
    {
        // 开启绘制
        if (!Main.dedServ && Main.myPlayer == player.whoAmI)
        {
            if (Main.mouseRight && _canPlaceTiles)
            {
                _canPlaceTiles = false;
                CombatText.NewText(player.getRect(), new Color(250, 40, 80), GetText("CombatText.Item.SpaceWand_Cancel"));
            }
            Color color;
            if (_canPlaceTiles)
                color = new Color(135, 0, 180);
            else
                color = new Color(250, 40, 80);
            Rectangle rectangle = GetRectangle(player);
            GameRectangle.Create(this, () => !Main.mouseLeft, rectangle, color * 0.35f, color, JudgeTextDisplayMode(rectangle));
        }
    }

    // 学单词
    // Judge v: 法官、判断、判定。n: 法官、裁判、裁判员

    /// <summary>
    /// 判断文字显示模式
    /// </summary>
    private static TextDisplayType JudgeTextDisplayMode(Rectangle rectangle)
    {
        return rectangle.Width >= rectangle.Height ? TextDisplayType.Width : TextDisplayType.Height;
    }

    // 学单词
    // Enough: 足够的
    // Enough!: 够了！

    public void UseItem_LeftMouseUp(Player player)
    {
        // 放置平台
        if (_canPlaceTiles && player.whoAmI == Main.myPlayer && !Main.dedServ)
        {
            bool playSound = false;
            Rectangle platfromRect = GetRectangle(player);

            ForeachTile(platfromRect, (x, y) =>
            {
                int oneIndex = EnoughItem(player, GetConditions());
                if (oneIndex > -1)
                {
                    Item item = player.inventory[oneIndex];
                    // 是否允许放置瓷砖，TML 的一个判定
                    if (!TileLoader.CanPlace(x, y, item.createTile)) return;
                    // 环境种子
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
                        else
                        {
                            if (WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                            {
                                playSound = true;
                                PickItemInInventory(player, GetConditions(), true, out _);
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

    public Rectangle GetRectangle(Player player)
    {
        int maxWidth = Main.netMode == NetmodeID.MultiplayerClient ? 244 : 500;
        // 平台数量
        if (!CountTheNumberOfTiles(player.inventory, out int count) || count > maxWidth)
        {
            count = maxWidth;
        }
        if (MathF.Abs(_beginMousePos.X - _endMousePos.X) > MathF.Abs(_beginMousePos.Y - _endMousePos.Y))
        {
            _endMousePos = ModifySize(_beginMousePos, _endMousePos, count, 1);
        }
        else
        {
            _endMousePos = ModifySize(_beginMousePos, _endMousePos, 1, count);
        }
        Rectangle rect = new((int)MathF.Min(_beginMousePos.X, _endMousePos.X), (int)MathF.Min(_beginMousePos.Y, _endMousePos.Y),
             (int)MathF.Abs(_beginMousePos.X - _endMousePos.X) + 1, (int)MathF.Abs(_beginMousePos.Y - _endMousePos.Y) + 1);
        return rect;
    }

    // 获取背包中 选中物块类型 的数量
    public bool CountTheNumberOfTiles(Item[] inventory, out int count)
    {
        return GetItemCount(inventory, GetConditions(), out count);
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
        if (tag.TryGet("placeType", out byte _placeType)) PlaceType = (PlaceType)_placeType;
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("placeType", (byte)PlaceType);
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
