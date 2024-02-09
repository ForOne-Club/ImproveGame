using ImproveGame.UI.ExtremeStorage.ToolButtons;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using ImproveGame.UIFramework.UIElements;

namespace ImproveGame.UI.ExtremeStorage;

public class StorageGrids : ModItemGrid
{
    public bool ScrollBarFilled => Scrollbar.InnerFilled;
    public string SearchContent => _searchBar.SearchContent ?? "";
    private float SearchBarHeight => _searchBar.Visible ? _searchBar.Height() + 6 : 0;

    /// <summary> 缓存一个所有被使用的箱子的列表，用于物品栏检测是否可用 </summary>
    internal static HashSet<int> ChestsThatBeingUsedCache;

    internal bool Hide;
    private readonly SUISearchBar _searchBar;
    private readonly View _toolSection;
    private readonly View _contentSection;

    public StorageGrids()
    {
        RemoveAllChildren(); // 将父类实例化添加的统统移除

        ShowSize = ModItemList.GetSize(10, 4, 40, 4f);

        Scrollbar = new SUIScrollBar
        {
            HAlign = 1f,
            Height = new StyleDimension(0f, 1f),
            HideIfFilled = true
        };
        Scrollbar.JoinParent(this);
        
        _contentSection = new View
        {
            Width = StyleDimension.FromPixels(ShowSize.X),
            Left = StyleDimension.FromPixels(0f),
            IsAdaptiveHeight = true,
            PaddingTop = 4
        };
        _contentSection.JoinParent(this);

        _searchBar = new SUISearchBar(true)
        {
            Width = StyleDimension.FromPixels(ShowSize.X - 4f),
            Left = StyleDimension.FromPixels(2f),
            RelativeMode = RelativeMode.Vertical
        };
        _searchBar.OnSearchContentsChanged += _ => UISystem.Instance.ExtremeStorageGUI.FindChestsAndPopulate(true);
        _searchBar.JoinParent(_contentSection);

        _toolSection = new View
        {
            Width = StyleDimension.FromPixels(ShowSize.X - 4f),
            Left = StyleDimension.FromPixels(2f),
            Height = StyleDimension.FromPixels(32f),
            Spacing = new Vector2(0f, 4f),
            RelativeMode = RelativeMode.Vertical
        };
        _toolSection.SetPadding(0f);
        _toolSection.JoinParent(_contentSection);

        MakeSeparator();

        ItemList = new ItemSlotList
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical
        };
        ItemList.JoinParent(_contentSection);

        Width.Pixels = ShowSize.X + Scrollbar.Width.Pixels + 11f;
        Height = new StyleDimension(0f, 1f);

        OnLeftMouseDown += (_, _) => AttemptStoppingUsingSearchbar();
        return;

        void MakeSeparator()
        {
            View separatorArea = new()
            {
                Height = new StyleDimension(10f, 0f),
                Width = new StyleDimension(-12f, 1f),
                HAlign = 0.5f,
                DragIgnore = true,
                RelativeMode = RelativeMode.Vertical,
                Spacing = new Vector2(0, 6)
            };
            separatorArea.JoinParent(_contentSection);
            separatorArea.Append(new UIHorizontalSeparator
            {
                Width = StyleDimension.FromPercent(1f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            });
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (Hide) return;

        ChestsThatBeingUsedCache = [];
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var plr = Main.player[i];
            if (plr is null || !plr.active || plr.dead || plr.chest < 0) continue;
            ChestsThatBeingUsedCache.Add(plr.chest);
        }

        base.Update(gameTime);

        // 移出范围以免影响其他UI
        if (Scrollbar.InnerFilled && Scrollbar.Left.Pixels is not 30f)
        {
            Scrollbar.Left.Pixels = 30f;
            Scrollbar.Recalculate();
        }

        if (!Scrollbar.InnerFilled && Scrollbar.Left.Pixels is not 0f)
        {
            Scrollbar.Left.Pixels = 0f;
            Scrollbar.Recalculate();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!Hide) base.Draw(spriteBatch);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (-Scrollbar.BarTop == _contentSection.Top.Pixels)
            return;

        _contentSection.Top.Pixels = -Scrollbar.BarTop;
        _contentSection.Recalculate();
    }

    public void Reset(List<ChestItemsInfo> items, ItemGroup group)
    {
        SetInventory(items);
        SetupTools(group);
        Recalculate();
    }

    private void SetInventory(List<ChestItemsInfo> items)
    {
        if (ItemList is not ItemSlotList itemList) return;

        // 初始化 ItemList 的时候会计算高度, 但是计算的是显示的高度.
        // 在 SetInventory 之后还会再计算一次, 计算的是 添加 items 之后的实际高度.
        itemList.SetInventory(items, 40, 4f);
        ShowSize = itemList.GetSize();
        RecalculateScrollBar();

        // 搜索栏
        _searchBar.Visible = items.Count is not 0;
    }

    private void SetupTools(ItemGroup group)
    {
        _toolSection.RemoveAllChildren();

        // 啥时候都有的按钮
        AddToolButton<RecipeToggleButton>();
        // 要顺序对，所以这里只能写得难看点了
        if (group is ItemGroup.Everything or ItemGroup.Furniture or ItemGroup.Alchemy)
            AddToolButton<AddChestButton>();
        AddToolButton<StackToInventoryButton>();
        AddToolButton<StackToStorageButton>();

        switch (group)
        {
            case ItemGroup.Everything:
                AddToolButton<SortButton>();
                AddToolButton<DepositAllButton>();
                break;
            case ItemGroup.Weapon:
                break;
            case ItemGroup.Tool:
                break;
            case ItemGroup.Ammo:
                break;
            case ItemGroup.Armor:
                break;
            case ItemGroup.Accessory:
                break;
            case ItemGroup.Block:
                break;
            case ItemGroup.Misc:
                break;
            case ItemGroup.Alchemy:
                AddToolButton<SortButton>();
                AddToolButton<DepositAllButton>();
                break;
            case ItemGroup.Furniture:
                AddToolButton<SortButton>();
                AddToolButton<DepositAllButton>();
                break;
            case ItemGroup.Setting:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(group), group, null);
        }
    }

    private void AddToolButton<T>() where T : ToolButtonBase
    {
        if (!ToolHandler.TryGetButton<T>(out var button))
            throw new InvalidOperationException($"ToolButtonBase {typeof(T).Name} is not registered.");
        _toolSection.Append(button);
    }

    public void RecalculateScrollBar()
    {
        float highestY = ItemList.BottomPixels + 8;
        float lowestY = _searchBar.Top();
        float totalHeight = highestY - lowestY;
        Scrollbar.SetView(GetDimensions().Height, totalHeight);
    }

    public void AttemptStoppingUsingSearchbar() => _searchBar.AttemptStoppingUsingSearchbar();
}