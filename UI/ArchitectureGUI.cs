#if false
using ImproveGame.Content.Items;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework.UIElements;

namespace ImproveGame.UI;

public class ArchitectureGUI : BaseBody
{
    private static bool _visible;

    public static bool Visible
    {
        get
        {
            return _visible && Main.playerInventory && Main.LocalPlayer.HeldItem is not null;
        }
        private set => _visible = value;
    }

    public override bool Enabled { get => Visible; set => Visible = value; }

    public override bool CanSetFocusTarget(UIElement target)
        => (target != this && basePanel.IsMouseHovering) || basePanel.IsLeftMousePressed;

    private static float panelLeft;
    private static float panelWidth;
    private static float panelTop;
    private static float panelHeight;

    public Item CurrentItem;
    public CreateWand CurrentWand => CurrentItem.ModItem as CreateWand;

    public Dictionary<string, ModItemSlot> ItemSlot => itemSlot;

    private static bool PrevMouseRight;
    private static bool HoveringOnSlots;

    private SUIPanel basePanel;
    private Dictionary<string, ModItemSlot> itemSlot = new();
    private UIText materialTitle;
    private ModIconTextButton styleButton;

    public override void OnInitialize()
    {
        panelLeft = 590f;
        panelTop = 120f;
        panelHeight = 550f;
        panelWidth = 190f;

        Append(basePanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            ShadowThickness = UIStyle.ShadowThicknessThinnerer,
            Draggable = true,
            Left = { Pixels = panelLeft },
            Top = { Pixels = panelTop },
            Width = { Pixels = panelWidth },
            Height = { Pixels = panelHeight },
        });

        // 排布
        // O O O
        // O O O
        // O O O
        // 切 换
        // 排布如上
        const float slotFirst = 0f;
        const float slotSecond = 60f;
        const float slotThird = 120f;
        const float slotFourth = 180;
        const float slotFifth = 240;
        const float slotSixth = 300;
        const float slotSeventh = 360;
        const float slotEighth = 420;

        itemSlot = new()
        {
            [nameof(CreateWand.Block)] = CreateItemSlot(slotFirst, slotFirst, nameof(CreateWand.Block),
                (i, item) => SlotPlace(i, item) || (item.createTile > -1 && Main.tileSolid[item.createTile] && !Main.tileSolidTop[item.createTile]),
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Block), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Block)}")),

            [nameof(CreateWand.Wall)] = CreateItemSlot(slotSecond, slotFirst, nameof(CreateWand.Wall),
                (i, item) => SlotPlace(i, item) || item.createWall > -1,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Wall), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Wall)}")),

            [nameof(CreateWand.Platform)] = CreateItemSlot(slotThird, slotFirst, nameof(CreateWand.Platform),
                (i, item) => SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Platforms[item.createTile]),
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Platform), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Platform)}")),

            [nameof(CreateWand.Torch)] = CreateItemSlot(slotFirst, slotSecond, nameof(CreateWand.Torch),
                (i, item) => SlotPlace(i, item) || (item.createTile > -1 && TileID.Sets.Torch[item.createTile]),
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Torch), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Torch)}")),

            [nameof(CreateWand.Chair)] = CreateItemSlot(slotSecond, slotSecond, nameof(CreateWand.Chair),
                (i, item) => SlotPlace(i, item) || (item.createTile == TileID.Chairs && item.placeStyle is not 1 and not 20),
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Chair), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Chair)}")),

            [nameof(CreateWand.Workbench)] = CreateItemSlot(slotThird, slotSecond, nameof(CreateWand.Workbench),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.WorkBenches,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Workbench), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Workbench)}")),

            [nameof(CreateWand.Bed)] = CreateItemSlot(slotFirst, slotThird, nameof(CreateWand.Bed),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Beds,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Bed), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Bed)}")),

            [nameof(CreateWand.Table)] = CreateItemSlot(slotSecond, slotThird, nameof(CreateWand.Table),
                (i, item) => SlotPlace(i, item) || item.createTile is TileID.Tables or TileID.Tables2,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Table), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Table)}")),

            [nameof(CreateWand.Door)] = CreateItemSlot(slotThird, slotThird, nameof(CreateWand.Door),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.ClosedDoor,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Door), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Door)}")),

            [nameof(CreateWand.Chest)] = CreateItemSlot(slotFirst, slotFourth, nameof(CreateWand.Chest),
                (i, item) => SlotPlace(i, item) || item.createTile is TileID.Containers or TileID.Containers2,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Chest), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Chest)}")),

            [nameof(CreateWand.Bookcase)] = CreateItemSlot(slotSecond, slotFourth, nameof(CreateWand.Bookcase),
                (i, item) => SlotPlace(i, item) || item.createTile is TileID.Bookcases,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Bookcase), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Bookcase)}")),

            [nameof(CreateWand.Bathtub)] = CreateItemSlot(slotThird, slotFourth, nameof(CreateWand.Bathtub),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Bathtubs,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Bathtub), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Bathtub)}")),

            [nameof(CreateWand.Candelabra)] = CreateItemSlot(slotFirst, slotFifth, nameof(CreateWand.Candelabra),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Candelabras,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Candelabra), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Candelabra)}")),

            [nameof(CreateWand.Candle)] = CreateItemSlot(slotSecond, slotFifth, nameof(CreateWand.Candle),
                (i, item) => SlotPlace(i, item) || item.createTile is TileID.Candles,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Candle), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Candle)}")),

            [nameof(CreateWand.Chandelier)] = CreateItemSlot(slotThird, slotFifth, nameof(CreateWand.Chandelier),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Chandeliers,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Chandelier), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Chandelier)}")),

            [nameof(CreateWand.Clock)] = CreateItemSlot(slotFirst, slotSixth, nameof(CreateWand.Clock),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.GrandfatherClocks,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Clock), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Clock)}")),

            [nameof(CreateWand.Dresser)] = CreateItemSlot(slotSecond, slotSixth, nameof(CreateWand.Dresser),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Dressers,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Dresser), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Dresser)}")),

            [nameof(CreateWand.Lamp)] = CreateItemSlot(slotThird, slotSixth, nameof(CreateWand.Lamp),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Lamps,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Lamp), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Lamp)}")),

            [nameof(CreateWand.Lantern)] = CreateItemSlot(slotFirst, slotSeventh, nameof(CreateWand.Lantern),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.HangingLanterns,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Lantern), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Lantern)}")),

            [nameof(CreateWand.Piano)] = CreateItemSlot(slotSecond, slotSeventh, nameof(CreateWand.Piano),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Pianos,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Piano), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Piano)}")),

            [nameof(CreateWand.Sink)] = CreateItemSlot(slotThird, slotSeventh, nameof(CreateWand.Sink),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Sinks,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Sink), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Sink)}")),

            [nameof(CreateWand.Sofa)] = CreateItemSlot(slotFirst, slotEighth, nameof(CreateWand.Sofa),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Benches,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Sofa), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Sofa)}")),

            [nameof(CreateWand.Toilet)] = CreateItemSlot(slotSecond, slotEighth, nameof(CreateWand.Toilet),
                (i, item) => SlotPlace(i, item) || (item.createTile == TileID.Toilets || (item.createTile == TileID.Chairs && item.placeStyle is 1 or 20)),
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Toilet), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Toilet)}")),

            [nameof(CreateWand.Campfire)] = CreateItemSlot(slotThird, slotEighth, nameof(CreateWand.Campfire),
                (i, item) => SlotPlace(i, item) || item.createTile == TileID.Campfire,
                (item, _) => CurrentWand.SetItem(nameof(CreateWand.Campfire), item.Clone()),
                () => GetText($"Architecture.{nameof(CreateWand.Campfire)}")),
        };

        // 头顶大字
        materialTitle = new("Materials", 0.5f, large: true)
        {
            HAlign = 0.5f
        };
        materialTitle.Left.Set(0, 0f);
        materialTitle.Top.Set(-40, 0f);
        materialTitle.Width.Set(panelWidth, 0f);
        materialTitle.Height.Set(40, 0f);
        basePanel.Append(materialTitle);

        // 房屋样式修改按钮
        styleButton = new(Language.GetText("Mods.ImproveGame.Common.Switch"), Color.White, "Images/UI/DisplaySlots_5");
        styleButton.Left.Set(slotFirst, 0f);
        styleButton.Top.Set(slotEighth + slotSecond, 0f);
        styleButton.Width.Set(164f, 0f);
        styleButton.Height.Set(42f, 0f);
        styleButton.OnLeftClick += (_, _) => CreateWand.NextStyle();
        basePanel.Append(styleButton);
    }

    public ModItemSlot CreateItemSlot(float x, float y, string iconTextureName, Func<Item, Item, bool> canPlace = null, Action<Item, bool> onItemChanged = null, Func<string> emptyText = null)
    {
        ModItemSlot slot = MyUtils.CreateItemSlot(x, y, iconTextureName, 0.85f, canPlace, onItemChanged, emptyText, basePanel, "Architecture");
        slot.OnUpdate += _ => HoveringOnSlots |= slot.IsMouseHovering;
        return slot;
    }

    // 主要是可拖动和一些判定吧
    public override void Update(GameTime gameTime)
    {
        if (CurrentItem?.ModItem is not CreateWand)
        {
            Close();
            return;
        }

        HoveringOnSlots = false;

        base.Update(gameTime);

        if (!Main.playerInventory)
        {
            Close();
            return;
        }

        // 右键点击空白直接关闭
        if (Main.mouseRight && !PrevMouseRight && basePanel.IsMouseHovering && !HoveringOnSlots)
        {
            Close();
            return;
        }

        PrevMouseRight = Main.mouseRight;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Player player = Main.LocalPlayer;

        base.Draw(spriteBatch);

        if (basePanel.ContainsPoint(Main.MouseScreen))
        {
            player.mouseInterface = true;
        }
    }

    /// <summary>
    /// 更新GUI物品槽与物品的同步
    /// </summary>
    /// <param name="createWand">建筑魔杖<see cref="CreateWand"/>实例</param>
    public void RefreshSlots(CreateWand createWand)
    {
        itemSlot[nameof(CreateWand.Block)].Item = createWand.Block;
        itemSlot[nameof(CreateWand.Platform)].Item = createWand.Platform;
        itemSlot[nameof(CreateWand.Workbench)].Item = createWand.Workbench;
        itemSlot[nameof(CreateWand.Table)].Item = createWand.Table;
        itemSlot[nameof(CreateWand.Chair)].Item = createWand.Chair;
        itemSlot[nameof(CreateWand.Door)].Item = createWand.Door;
        itemSlot[nameof(CreateWand.Chest)].Item = createWand.Chest;
        itemSlot[nameof(CreateWand.Bed)].Item = createWand.Bed;
        itemSlot[nameof(CreateWand.Bookcase)].Item = createWand.Bookcase;
        itemSlot[nameof(CreateWand.Bathtub)].Item = createWand.Bathtub;
        itemSlot[nameof(CreateWand.Candelabra)].Item = createWand.Candelabra;
        itemSlot[nameof(CreateWand.Candle)].Item = createWand.Candle;
        itemSlot[nameof(CreateWand.Chandelier)].Item = createWand.Chandelier;
        itemSlot[nameof(CreateWand.Clock)].Item = createWand.Clock;
        itemSlot[nameof(CreateWand.Dresser)].Item = createWand.Dresser;
        itemSlot[nameof(CreateWand.Lamp)].Item = createWand.Lamp;
        itemSlot[nameof(CreateWand.Lantern)].Item = createWand.Lantern;
        itemSlot[nameof(CreateWand.Piano)].Item = createWand.Piano;
        itemSlot[nameof(CreateWand.Sink)].Item = createWand.Sink;
        itemSlot[nameof(CreateWand.Sofa)].Item = createWand.Sofa;
        itemSlot[nameof(CreateWand.Toilet)].Item = createWand.Toilet;
        itemSlot[nameof(CreateWand.Torch)].Item = createWand.Torch;
        itemSlot[nameof(CreateWand.Campfire)].Item = createWand.Campfire;
        itemSlot[nameof(CreateWand.Wall)].Item = createWand.Wall;
    }

    /// <summary>
    /// 打开GUI界面
    /// </summary>
    public void Open(CreateWand wand)
    {
        OperateInventory(true);
        PrevMouseRight = true; // 防止一打开就关闭
        basePanel.Dragging = false;
        Visible = true;
        SoundEngine.PlaySound(SoundID.MenuOpen);

        CurrentItem = wand.Item;
        RefreshSlots(CurrentWand);

        // UI刚加载（即OnInit）时还未加载翻译，因此我们要在这里设置一遍文本
        materialTitle.SetText(Language.GetText("Mods.ImproveGame.Architecture.Materials"));
        styleButton.SetText(Language.GetText("Mods.ImproveGame.Common.Switch"), 1f, Color.White);
    }

    /// <summary>
    /// 关闭GUI界面
    /// </summary>
    public void Close()
    {
        CurrentItem = new Item();
        Visible = false;
        PrevMouseRight = false;
        Main.blockInput = false;
        SoundEngine.PlaySound(SoundID.MenuClose);
    }
}
#endif