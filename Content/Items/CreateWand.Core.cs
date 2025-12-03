using ImproveGame.Common;
using ImproveGame.Common.Configs;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions.Construction;
using ImproveGame.UIFramework;
using Terraria.ModLoader.IO;
using Terraria.Utilities.FileBrowser;

namespace ImproveGame.Content.Items;

public partial class CreateWand
{
    #region 辅助类型
    private enum TileSort : byte
    {
        None,
        Block,
        Platform,
        Workbench,
        Table,
        Chair,
        Door,
        Chest,
        Bed,
        Bookcase,
        Bathtub,
        Candelabra,
        Candle,
        Chandelier,
        Clock,
        Dresser,
        Lamp,
        Lantern,
        Piano,
        Sink,
        Sofa,
        Toilet,
        Torch,
        Campfire
    }
    private struct TileInfo(TileSort sort, bool hasWall)
    {
        public TileSort Sort { get; set; } = sort;
        public bool HasWall { get; set; } = hasWall;
    }
    private record TileData(TileInfo Info, int X, int Y);
    #endregion

    #region 建筑信息
    private static List<Texture2D> _prisons;
    private static List<Texture2D> _prisonsPreView;
    private static List<Color[]> _colors;

    private static bool _colorsLoaded;
    private static Queue<string> _customAutoLoadQueue = [];
    private static int _styleIndex;

    private static bool _isWaitingPreview;
    private static string _tempFilePath;

    public static Texture2D Prison => _prisons[_styleIndex];
    public static Texture2D PrisonsPreView => _prisonsPreView[_styleIndex];
    public static Color[] Colors => _colors[_styleIndex];
    #endregion

    #region 加载数据
    public static void AddNewPrisonStyle(Texture2D dataTexture, Texture2D previewTexture, bool useRunOnMainThread = true)
    {
        _prisons.Add(dataTexture);
        _prisonsPreView.Add(previewTexture);
        if (useRunOnMainThread)
            Main.RunOnMainThread(() =>
            {
                _colors.Add(GetColors(dataTexture));
            });
        else
            _colors.Add(GetColors(dataTexture));
    }

    public override void Load()
    {
        if (!Main.dedServ)
        {
            _colorsLoaded = false;
            _customAutoLoadQueue.Clear();
            // 把读取放到主线程上
            Main.QueueMainThreadAction(() =>
            {
                if (_colorsLoaded)
                    return;

                _prisons =
                [
                    ModAsset.Prison1.Value, ModAsset.Prison2.Value, ModAsset.Prison3.Value
                ];

                _prisonsPreView =
                [
                    ModAsset.PrisonPreview1.Value, ModAsset.PrisonPreview2.Value, ModAsset.PrisonPreview3.Value
                ];

                _colors = [GetColors(_prisons[0]), GetColors(_prisons[1]), GetColors(_prisons[2])];
                _colorsLoaded = true;

                string directory = Path.Combine(Main.SavePath, "Mods", "ImproveGame", "CreateWand");
                if (!Directory.Exists(directory)) 
                {
                    Directory.CreateDirectory(directory);

                    var building = ModAsset.PrisonComplex.Value;
                    using FileStream fs = new FileStream(Path.Combine(directory, "buildingShowcase.png"), FileMode.CreateNew);
                    building.SaveAsPng(
                        fs,
                        building.Width,
                        building.Height
                        );
                }

                foreach (var file in Directory.GetFiles(directory, "*.png"))
                    _customAutoLoadQueue.Enqueue(file);
            }
            );
        }
        else
        {
            _colorsLoaded = true;
        }
    }

    public override void Unload()
    {
        if (!Main.dedServ)
        {
            _prisons = null;
            _prisonsPreView = null;
            _colors = null;
        }

        _colorsLoaded = false;
    }
    #endregion

    #region 辅助函数
    /// <summary>
    /// 颜色对应的物块类型
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static TileInfo Color2TileInfo(Color color)
    {
        int index = (color.R + 1) / 64 * 5 + (color.G + 1) / 64;
        if (index is > 23 or < 0) index = 0;
        TileSort sort = (TileSort)index;
        bool hasWall = color.B > 127;
        return new TileInfo(sort, hasWall);
    }


    /// <summary>
    /// 从<see cref="string"/>类型的名称获取对应的物品实例，为了方便而设置
    /// </summary>
    /// <param name="itemType">物品实例名称</param>
    /// <param name="item">物品实例</param>
    internal void GetStoredItemInstance(string itemType, out Item item)
    {
        item = itemType switch
        {
            nameof(Block) => Block,
            nameof(Platform) => Platform,
            nameof(Workbench) => Workbench,
            nameof(Table) => Table,
            nameof(Chair) => Chair,
            nameof(Door) => Door,
            nameof(Chest) => Chest,
            nameof(Bed) => Bed,
            nameof(Bookcase) => Bookcase,
            nameof(Bathtub) => Bathtub,
            nameof(Candelabra) => Candelabra,
            nameof(Candle) => Candle,
            nameof(Chandelier) => Chandelier,
            nameof(Clock) => Clock,
            nameof(Dresser) => Dresser,
            nameof(Lamp) => Lamp,
            nameof(Lantern) => Lantern,
            nameof(Piano) => Piano,
            nameof(Sink) => Sink,
            nameof(Sofa) => Sofa,
            nameof(Toilet) => Toilet,
            nameof(Torch) => Torch,
            nameof(Campfire) => Campfire,
            nameof(Wall) => Wall,
            _ => null
        };
    }

    /// <summary>
    /// 设置物品，用于UI和物品存储数据间的同步
    /// </summary>
    /// <param name="itemType">物品存储类型</param>
    /// <param name="item">物品实例</param>
    internal void SetItem(string itemType, Item item)
    {
        switch (itemType)
        {
            case nameof(Block):
                Block = item;
                break;
            case nameof(Platform):
                Platform = item;
                break;
            case nameof(Wall):
                Wall = item;
                break;
            case nameof(Torch):
                Torch = item;
                break;
            case nameof(Workbench):
                Workbench = item;
                break;
            case nameof(Chair):
                Chair = item;
                break;
            case nameof(Bed):
                Bed = item;
                break;
            case nameof(Table):
                Table = item;
                break;
            case nameof(Door):
                Door = item;
                break;
            case nameof(Chest):
                Chest = item;
                break;
            case nameof(Bookcase):
                Bookcase = item;
                break;
            case nameof(Bathtub):
                Bathtub = item;
                break;
            case nameof(Candelabra):
                Candelabra = item;
                break;
            case nameof(Candle):
                Candle = item;
                break;
            case nameof(Chandelier):
                Chandelier = item;
                break;
            case nameof(Clock):
                Clock = item;
                break;
            case nameof(Dresser):
                Dresser = item;
                break;
            case nameof(Lamp):
                Lamp = item;
                break;
            case nameof(Lantern):
                Lantern = item;
                break;
            case nameof(Piano):
                Piano = item;
                break;
            case nameof(Sink):
                Sink = item;
                break;
            case nameof(Sofa):
                Sofa = item;
                break;
            case nameof(Toilet):
                Toilet = item;
                break;
            case nameof(Campfire):
                Campfire = item;
                break;
        }
    }

    // 切换样式
    internal static void NextStyle()
    {
        _styleIndex++;
        _styleIndex %= _prisons.Count;
    }

    #region 中键添加自定义样式支持
    private static void HandleMiddleClick(Player player)
    {
        // 需要按下特殊物品交互键(默认中键)并且没在使用物品
        if (!KeybindSystem.ItemInteractKeybind.JustPressed || player.itemAnimation != 0) return;

        // 设置使用物品时间防止后续放置
        player.itemAnimation = player.itemAnimationMax = 5;
        player.altFunctionUse = 1;

        //var coord = Main.MouseWorld.ToTileCoordinates();
        //var tile = Framing.GetTileSafely(coord);

        //Main.NewText((tile.TileFrameX / 18, tile.TileFrameY / 18, tile.WallFrameX / 18, tile.WallFrameY / 18));
        //WorldGen.TileFrame(coord.X, coord.Y, true);
        //return;

        // 筛选png文件
        ExtensionFilter[] extensions = [
             new ExtensionFilter("png files", "png")
        ];

        // 打开文件选择窗口
        string path = FileBrowser.OpenFilePanel("Select config image", extensions);

        if (path == null) return;

        // 进行注册
        HandleRegister(path);
    }

    private static void HandleRegister(string path)
    {

        using FileStream fileStream = new FileStream(path, FileMode.Open);
        using Texture2D texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, fileStream);

        /*
        int w = texture.Width * 16;
        int h = texture.Height * 16;
        Texture2D previewTexture = new Texture2D(Main.graphics.GraphicsDevice, w, h);
        Color[] datas = new Color[texture.Width * texture.Height];
        texture.GetData(datas);
        Color[] colors = new Color[w * h];
        for (int i = 0; i < w; i++)
        {
            int x = i / 16;
            for (int j = 0; j < h; j++)
            {
                int y = j / 16;
                colors[j * w + i] = datas[y * texture.Width + x];
            }
        }
        previewTexture.SetData(colors);
        AddNewPrisonStyle(texture, previewTexture, false);
        */
        var colors = GetColors(texture);
        _prisons.Add(texture);
        _colors.Add(colors);

        _isWaitingPreview = true;

        var tag = CreateStructureTagFromColors(colors, texture.Width, texture.Height);
        var tagPath = Path.Combine(ModLoader.ModPath, nameof(ImproveGame), "tempStructure.qotstruct");
        TagIO.ToFile(tag, tagPath);
        WandSystem.ConstructFilePath = tagPath;
        _tempFilePath = tagPath;
        PreviewRenderer.ResetPreviewTarget = PreviewRenderer.ResetState.WaitReset;
        int width = texture.Width;
        int height = texture.Height;
        PreviewRenderer.PreviewTarget =
            new RenderTarget2D(
                Main.graphics.GraphicsDevice,
                width * 16 + 20,
                height * 16 + 20,
                false,
                default,
                default,
                default,
                RenderTargetUsage.PreserveContents);
    }

    private static void HandlePreviewRegister()
    {
        if (!_isWaitingPreview || PreviewRenderer.ResetPreviewTarget != PreviewRenderer.ResetState.Finished) return;

        _isWaitingPreview = false;
        if (!string.IsNullOrEmpty(_tempFilePath))
            File.Delete(_tempFilePath);
        FileOperator.CachedStructureDatas.Remove(_tempFilePath);
        var pvRender = PreviewRenderer.PreviewTarget;
        int width = pvRender.Width;
        int height = pvRender.Height;
        Texture2D previewTexture = new Texture2D(Main.graphics.GraphicsDevice, width, height);
        previewTexture.SetData(GetColors(pvRender));
        _prisonsPreView.Add(previewTexture);
    }
    #endregion

    #endregion

    #region 放置函数
    /// <summary>
    /// 为了避免代码过长和减少重复工作，设置的放置判断“总开关”
    /// </summary>
    /// <param name="storedItem">指向仓库物品，即可能的存储物</param>
    /// <param name="player">玩家，一般应该是<see cref="Main.LocalPlayer"></param>
    /// <param name="x">放置目标X坐标</param>
    /// <param name="y">放置目标Y坐标</param>
    /// <param name="tryMethod">进行放置尝试的方法，只有符合条件的才会放置</param>
    private static void TryPlace(ref Item storedItem, Player player, int x, int y, Func<Item, bool> tryMethod)
    {
        // 没有存储物品，在物品栏里面找
        if (storedItem.IsAir || storedItem.createTile < TileID.Dirt)
        {
            PickItemInInventory(player, item =>
                    item is not null && tryMethod(item) &&
                    BongBongPlace(x, y, item, player, true, true, !_playedSound),
                true, out int index);
            if (index != -1)
            {
                _playedSound = true;
            }
        }
        // 进行存储物品的放置尝试
        else if (storedItem is not null && tryMethod(storedItem) &&
                 BongBongPlace(x, y, storedItem, player, true, true, !_playedSound))
        {
            TryConsumeItem(ref storedItem, player);
            _playedSound = true;
        }
    }

    private static bool TryPlacePlatform(Item item) =>
        item.createTile >= TileID.Dirt && TileID.Sets.Platforms[item.createTile];

    private static bool TryPlaceTile(Item item) =>
        item.createTile >= TileID.Dirt && Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile];

    private static bool TryPlaceWall(Item item, Player player, int x, int y)
    {
        if (item.createWall > -1)
        {
            TryKillTile(x, y, player);
            if (UIConfigs.Instance.ExplosionEffect)
            {
                BongBong(new Vector2(x, y) * 16f, 16, 16);
            }

            WorldGen.KillWall(x, y);
            if (Main.tile[x, y].WallType == 0)
            {
                WorldGen.PlaceWall(x, y, item.createWall, true);
                return true;
            }
        }

        return false;
    }
    #endregion

    #region 原版重写函数
    /// <summary>
    /// 就为了实现一个“如果不放东西就没爆炸声音”的功能
    /// </summary>
    private static bool _playedSound;

    public override bool? UseItem(Player player)
    {
        if (!_colorsLoaded || _colors is null)
        {
            ImproveGame.Instance.Logger.Error("Create Wand Colors didn't load. Please report to mod developers.");
            return base.UseItem(player);
        }

        if (!Main.dedServ && Main.myPlayer == player.whoAmI && player.altFunctionUse == 0)
        {
            Point position = Main.MouseWorld.ToTileCoordinates() - (Prison.Size() / 2f).ToPoint();

            List<TileData> tileDatas = new();

            for (int i = 0; i < Colors.Length; i++) // 不会放置椅子和工作台
            {
                int x = position.X + i % Prison.Width; // 物块在图片中的 X 坐标
                int y = position.Y + i / Prison.Width; // Y 坐标

                TileInfo tileSort = Color2TileInfo(Colors[i]);

                // 墙体
                if (tileSort.HasWall)
                {
                    if (Wall.IsAir || Wall.createWall <= WallID.None)
                    {
                        PickItemInInventory(player, item => TryPlaceWall(item, player, x, y), true, out _);
                    }
                    else if (TryPlaceWall(Wall, player, x, y))
                    {
                        TryConsumeItem(ref Wall, player);
                    }
                }

                switch (tileSort.Sort)
                {
                    case TileSort.Block:
                        TryPlace(ref Block, player, x, y, TryPlaceTile);
                        break;
                    case TileSort.Platform:
                        TryPlace(ref Platform, player, x, y, TryPlacePlatform);
                        break;
                    default:
                        tileDatas.Add(new(tileSort, x, y));
                        break;
                }
            }

            for (int i = 0; i < tileDatas.Count; i++) // 火把，椅子，工作台，桌子，床
            {
                int x = tileDatas[i].X;
                int y = tileDatas[i].Y;
                if (Main.tile[x, y].HasTile)
                {
                    continue;
                }

                // 进行其他的放置尝试
                switch (tileDatas[i].Info.Sort)
                {
                    case TileSort.Torch:
                        TryPlace(ref Torch, player, x, y,
                            item => item.createTile >= TileID.Dirt && TileID.Sets.Torch[item.createTile]);
                        break;
                    case TileSort.Workbench:
                        TryPlace(ref Workbench, player, x, y, item => item.createTile == TileID.WorkBenches);
                        break;
                    case TileSort.Chair:
                        TryPlace(ref Chair, player, x, y, item => item.createTile == TileID.Chairs && item.placeStyle is not 1 and not 20);
                        Main.tile[tileDatas[i].X, tileDatas[i].Y].TileFrameX += 18;
                        Main.tile[tileDatas[i].X, tileDatas[i].Y - 1].TileFrameX += 18;
                        break;
                    case TileSort.Table:
                        TryPlace(ref Table, player, x, y, item => item.createTile is TileID.Tables or TileID.Tables2);
                        break;
                    case TileSort.Door:
                        TryPlace(ref Door, player, x, y, item => item.createTile == TileID.ClosedDoor);
                        break;
                    case TileSort.Bed:
                        TryPlace(ref Bed, player, x, y, item => item.createTile == TileID.Beds);
                        break;
                    case TileSort.Chest:
                        TryPlace(ref Chest, player, x, y, item => item.createTile is TileID.Containers or TileID.Containers2);
                        break;
                    case TileSort.Bookcase:
                        TryPlace(ref Bookcase, player, x, y, item => item.createTile == TileID.Bookcases);
                        break;
                    case TileSort.Bathtub:
                        TryPlace(ref Bathtub, player, x, y, item => item.createTile == TileID.Bathtubs);
                        break;
                    case TileSort.Candelabra:
                        TryPlace(ref Candelabra, player, x, y, item => item.createTile == TileID.Candelabras);
                        break;
                    case TileSort.Candle:
                        TryPlace(ref Candle, player, x, y, item => item.createTile == TileID.Candles);
                        break;
                    case TileSort.Chandelier:
                        TryPlace(ref Chandelier, player, x, y, item => item.createTile == TileID.Chandeliers);
                        break;
                    case TileSort.Clock:
                        TryPlace(ref Clock, player, x, y, item => item.createTile == TileID.GrandfatherClocks);
                        break;
                    case TileSort.Dresser:
                        TryPlace(ref Dresser, player, x, y, item => item.createTile == TileID.Dressers);
                        break;
                    case TileSort.Lamp:
                        TryPlace(ref Lamp, player, x, y, item => item.createTile == TileID.Lamps);
                        break;
                    case TileSort.Lantern:
                        TryPlace(ref Lantern, player, x, y, item => item.createTile == TileID.HangingLanterns);
                        break;
                    case TileSort.Piano:
                        TryPlace(ref Piano, player, x, y, item => item.createTile == TileID.Pianos);
                        break;
                    case TileSort.Sink:
                        TryPlace(ref Sink, player, x, y, item => item.createTile == TileID.Sinks);
                        break;
                    case TileSort.Sofa:
                        TryPlace(ref Sofa, player, x, y, item => item.createTile == TileID.Benches);
                        break;
                    case TileSort.Toilet:
                        TryPlace(ref Toilet, player, x, y, item => item.createTile == TileID.Toilets || (item.createTile == TileID.Chairs && item.placeStyle is 1 or 20));
                        var tile = Main.tile[tileDatas[i].X, tileDatas[i].Y];
                        tile.TileFrameX += 18;
                        Main.tile[tileDatas[i].X, tileDatas[i].Y - 1].TileFrameX += 18;
                        break;
                    case TileSort.Campfire:
                        TryPlace(ref Campfire, player, x, y, item => item.createTile == TileID.Campfire);
                        break;
                }
            }
            
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(player.whoAmI, position.X, position.Y, Prison.Width, Prison.Height);

            // 重新刷新合成配方，这样如果一个物品没了就可以把它的合成配方刷新掉
            Recipe.FindRecipes();
            // 同步UI物品
            UISystem.Instance.ArchitectureGUI.RefreshSlots(this);
        }

        if (!_playedSound && player.altFunctionUse == 0)
            CombatText.NewText(player.getRect(), new Color(225, 0, 0), GetText("CombatText.Item.CreateWand_NotEnough"),
                true);

        _playedSound = false;
        return true;
    }

    public override void HoldItem(Player player)
    {
        // if (player.itemAnimation == 0)
        //     Item.mana = 40;
        if (!Main.dedServ && Main.myPlayer == player.whoAmI)
        {
            Point point = Main.MouseWorld.ToTileCoordinates() - (Prison.Size() / 2f).ToPoint(); // 鼠标位置
            int boxIndex = GameRectangle.Create(this, () => false,
                new Rectangle(point.X, point.Y, Prison.Width, Prison.Height), Color.Yellow * 0f, Color.Yellow * 0f);
            if (GameRectangleSystem.GameRectangles.IndexInRange(boxIndex))
            {
                GameRectangle box = GameRectangleSystem.GameRectangles[boxIndex];
                box.Texture2D = PrisonsPreView;
            }

            if (!_isWaitingPreview) 
            {
                if (_customAutoLoadQueue.TryDequeue(out string path))
                    HandleRegister(path);
                else
                    HandleMiddleClick(player);
            }


            HandlePreviewRegister();
        }
    }
    #endregion

}
