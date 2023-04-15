using ImproveGame.Common.GlobalItems;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.SUIElements;
using System.Reflection;
using Terraria.GameInput;
using Terraria.UI.Chat;
using Terraria.ID;
using UIImage = Terraria.GameContent.UI.Elements.UIImage;

namespace ImproveGame.Interface.GUI;

public class PrefixRecallGUI : UIState
{
    public static bool Visible =>
        Config.ImprovePrefix && !Main.reforgeItem.IsAir && Main.reforgeItem.TryGetGlobalItem<ImprovePrefixItem>(out var modItem) && modItem.Prefixs.Count > 0;
    private static int _oldItemType; // 用于监测type以及时更新uiList
    private static int _oldPrefixCount; // 用于监测词缀数量以及时更新uiList

    private const float PanelLeft = 54f;
    private const float PanelTop = 340f;
    private const float PanelHeight = 330f;
    private const float PanelWidth = 388f;

    private SUIPanel _basePanel; // 背景板
    public SUIScrollbar Scrollbar; // 拖动条
    public UIList UIList; // 明细列表

    public override void OnInitialize()
    {
        _basePanel = new SUIPanel(new Color(29, 34, 70), new Color(44, 57, 105, 160));
        _basePanel.Left.Set(PanelLeft, 0f);
        _basePanel.Top.Set(PanelTop, 0f);
        _basePanel.Width.Set(PanelWidth, 0f);
        _basePanel.Height.Set(PanelHeight, 0f);
        Append(_basePanel);

        UIList = new UIList
        {
            Width = new(-28f, 1f),
            Height = new(0, 1f),
            ListPadding = 4f,
            ManualSortMethod = _ => { }
        };
        _basePanel.Append(UIList);

        Scrollbar = new SUIScrollbar
        {
            HAlign = 1f,
            Height = new(0, 1f)
        };
        Scrollbar.SetView(100f, 1000f);
        SetupScrollBar();
        _basePanel.Append(Scrollbar);
    }

    private void SetupScrollBar(bool resetViewPosition = true)
    {
        float height = UIList.GetInnerDimensions().Height;
        Scrollbar.SetView(height, UIList.GetTotalHeight());
        if (resetViewPosition)
            Scrollbar.ViewPosition = 0f;
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
        if (_basePanel.GetOuterDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint()))
            Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
    }

    public override void Update(GameTime gameTime)
    {
        if (!Visible)
        {
            _oldItemType = ItemID.None;
            return;
        }

        base.Update(gameTime);

        if (Scrollbar.IsMouseHovering) // 不知道为啥默认没有
            Main.LocalPlayer.mouseInterface = true;

        if (_oldItemType != Main.reforgeItem.type || _oldPrefixCount != Main.reforgeItem.GetGlobalItem<ImprovePrefixItem>().Prefixs.Count)
        {
            SetupList();
            _oldItemType = Main.reforgeItem.type;
            _oldPrefixCount = Main.reforgeItem.GetGlobalItem<ImprovePrefixItem>().Prefixs.Count;
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Scrollbar is not null)
        {
            UIList._innerList.Top.Set(-Scrollbar.ViewPosition, 0);
        }
        UIList.Recalculate();

        if (_basePanel.IsMouseHovering || Scrollbar.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Prefix Recall GUI");
        }

        base.DrawSelf(spriteBatch);
    }

    public void SetupList()
    {
        UIList.Clear();

        var prefixes = Main.reforgeItem.GetGlobalItem<ImprovePrefixItem>().Prefixs.ToArray();
        var sortedResult = from pair in prefixes orderby pair.Cost descending select pair; // 排序
        foreach (var prefix in sortedResult)
        {
            UIList.Add(new PrefixTab(prefix.PrefixId, prefix.Cost));
        }

        Recalculate();
        SetupScrollBar();
    }
}

public class PrefixTab : SUIPanel
{
    private static readonly Dictionary<int, Color> RarityColors = new()
    {
        [-11] = Colors.RarityAmber,
        [-1] = Colors.RarityTrash,
        [0] = Colors.RarityNormal,
        [1] = Colors.RarityBlue,
        [2] = Colors.RarityGreen,
        [3] = Colors.RarityOrange,
        [4] = Colors.RarityRed,
        [5] = Colors.RarityPink,
        [6] = Colors.RarityPurple,
        [7] = Colors.RarityLime,
        [8] = Colors.RarityYellow,
        [9] = Colors.RarityCyan,
        [10] = new Color(255, 40, 100),
        [11] = new Color(180, 40, 255)
    };

    public ref Item Item => ref Main.reforgeItem; // 向原物品的链接
    public ModItemSlot DummyItem; // 当前物品展示
    internal int PrefixId;
    internal int Price;

    public PrefixTab(int prefixId, int value) : base(new Color(89, 116, 213), new Color(44, 57, 105, 160))
    {
        PrefixId = prefixId;
        CalculatePrice(value);
        this.SetSize(new Vector2(0f, 80f), 1f);

        DummyItem = new(1f)
        {
            Interactable = false,
            RoundBorder = true,
            VAlign = .5f,
            Item = new(Item.type)
        };
        DummyItem.Item.Prefix(prefixId);
        Append(DummyItem);

        UIHorizontalSeparator separator = new()
        {
            Top = StyleDimension.FromPixels(20),
            Left = StyleDimension.FromPixels(70),
            Width = StyleDimension.FromPixels(240),
            Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.7f
        };
        Append(separator);

        var reforgeButton = new UIImage(TextureAssets.Reforge[0])
        {
            Top = new StyleDimension(separator.Height.Pixels + separator.Top.Pixels + 6f, 0f),
            Left = new StyleDimension(-30f, 1f)
        };
        reforgeButton.SetSize(24f, 24f);
        reforgeButton.OnLeftClick += (_, _) => Reforge(reforgeButton);
        reforgeButton.OnUpdate += _ =>
        {
            string str = Lang.inter[19].Value;
            reforgeButton.Color = Color.White;
            if (PrefixId == Item.prefix)
            {
                reforgeButton.Color = Color.Gray;
                return;
            }
            if (reforgeButton.IsMouseHovering)
            {
                Main.instance.MouseText(str);
            }
        };
        reforgeButton.OnMouseOver += (_, _) =>
        {
            if (PrefixId != Item.prefix)
                reforgeButton.SetImage(TextureAssets.Reforge[1]);
        };
        reforgeButton.OnMouseOut += (_, _) =>
        {
            reforgeButton.SetImage(TextureAssets.Reforge[0]);
        };
        Append(reforgeButton);
    }

    private void CalculatePrice(int value)
    {
        Price = value * Item.stack; // TML: #StackablePrefixWeapons: scale with current stack size
        bool canApplyDiscount = true;
        if (!ItemLoader.ReforgePrice(Item, ref Price, ref canApplyDiscount))
            return;

        if (canApplyDiscount && Main.LocalPlayer.discountAvailable)
            Price = (int)(Price * 0.8);

        Price = (int)(Price * Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);
        Price = (int)(Price * 0.6f);
    }

    private void Reforge(UIImage ui)
    {
        if (PrefixId == Item.prefix || !Main.LocalPlayer.CanBuyItem(Price) || !ItemLoader.PreReforge(Item))
        {
            return;
        }

        Main.LocalPlayer.BuyItem(Price);
        bool favorited = Item.favorited;
        int stack = Item.stack;  //#StackablePrefixWeapons: keep the stack, (i.e. light discs)
        /*Item r = new();
        r.netDefaults(Item.netID);
        r = r.CloneWithModdedDataFrom(Item)*//* tModPorter Note: Removed. Use Clone, ResetPrefix or Refresh *//*;
        r.Prefix(PrefixId);
        Item = r.Clone();*/
        Item.ResetPrefix();
        Item.Prefix(PrefixId);
        Item.position = Main.LocalPlayer.Center;
        Item.favorited = favorited;
        Item.stack = stack;
        ItemLoader.PostReforge(Item);
        PopupText.NewText(PopupTextContext.ItemReforge, Item, Item.stack, noStack: true);
        SoundEngine.PlaySound(SoundID.Item37);
        ui.SetImage(TextureAssets.Reforge[0]);
    }

    public override void DrawSelf(SpriteBatch sb)
    {
        base.DrawSelf(sb);
        var dimensions = GetDimensions();
        var position = dimensions.Center();
        position.Y -= 36f;
        position.X -= 82f;
        ItemSlot.DrawMoney(sb, Lang.inter[46].Value, position.X, position.Y, Utils.CoinsSplit(Price), true);

        var textColor = Color.White;
        var item = DummyItem.Item;
        if (item.master || item.rare == ItemRarityID.Master)
            textColor = new Color(255, (int)(Main.masterColor * 200f), 0);
        else if (item.expert || item.rare == ItemRarityID.Expert)
            textColor = Main.DiscoColor;
        else if (RarityColors.ContainsKey(item.rare))
            textColor = RarityColors[item.rare];
        if (item.rare >= ItemRarityID.Count)
            textColor = RarityLoader.GetRarity(item.rare).RarityColor;
        position.Y += 6f;
        string str = PrefixId < 0 || PrefixId >= Lang.prefix.Length
            ? GetText("Config.ImprovePrefix.Unknown")
            : Lang.prefix[PrefixId].Value;
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, str, position, textColor, 0f,
            Vector2.Zero, Vector2.One);
    }
}