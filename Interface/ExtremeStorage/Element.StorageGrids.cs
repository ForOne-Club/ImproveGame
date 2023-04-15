using ImproveGame.Common.Utils.Extensions;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.ExtremeStorage;

public class StorageGrids : ModItemGrid
{
    public bool ScrollBarFilled => Scrollbar.InnerFilled;
    public string SearchContent => _searchBar.SearchContent;
    private float SearchBarHeight => _searchBar.Visible ? _searchBar.Height() + 6 : 0;

    /// <summary> 缓存一个所有被使用的箱子的列表，用于物品栏检测是否可用 </summary>
    internal static HashSet<int> ChestsThatBeingUsedCache;
    internal bool Hide;
    private readonly AddChestButton _addChestButton;
    private readonly SUISearchBar _searchBar;

    public StorageGrids()
    {
        RemoveAllChildren(); // 将父类实例化添加的统统移除

        ShowSize = ModItemList.GetSize(10, 4, 40, 4f);

        ItemList = new ItemSlotList {DragIgnore = true};
        ItemList.Join(this);

        Scrollbar = new SUIScrollbar
        {
            HAlign = 1f,
            Height = new StyleDimension(0f, 1f),
            HideIfFilled = true
        };
        Scrollbar.Join(this);

        _searchBar = new SUISearchBar(true)
        {
            Width = StyleDimension.FromPixels(ShowSize.X - 4f),
            Left = StyleDimension.FromPixels(2f)
        };
        _searchBar.OnSearchContentsChanged += _ => UISystem.Instance.ExtremeStorageGUI.FindChestsAndPopulate(true);
        _searchBar.Join(this);

        Append(_addChestButton = new AddChestButton());

        Width.Pixels = ShowSize.X + Scrollbar.Width.Pixels + 11f;
        Height = new StyleDimension(0f, 1f);

        OnLeftMouseDown += (_, _) => AttemptStoppingUsingSearchbar();
    }

    public override void Update(GameTime gameTime)
    {
        if (Hide) return;

        ChestsThatBeingUsedCache = new HashSet<int>();
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
        if (-Scrollbar.ViewPosition + SearchBarHeight != ItemList.Top.Pixels)
        {
            ItemList.Top.Pixels = -Scrollbar.ViewPosition + SearchBarHeight;
            ItemList.Recalculate();
        }

        if (-Scrollbar.ViewPosition + 2 != _searchBar.Top.Pixels)
        {
            _searchBar.Top.Pixels = -Scrollbar.ViewPosition + 2;
            _searchBar.Recalculate();
        }

        float chestButtonY = ItemList.Top.Pixels + ItemList.Height.Pixels + 8;
        if (_addChestButton.Top.Pixels != chestButtonY)
        {
            _addChestButton.Top.Pixels = chestButtonY;
            _addChestButton.Recalculate();
        }
    }

    public void SetInventory(List<ChestItemsInfo> items)
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

    public void RecalculateScrollBar()
    {
        float totalHeight = _addChestButton.Height() + ItemList.Height() + SearchBarHeight + 8;
        Scrollbar.SetView(GetDimensions().Height, totalHeight);
    }

    public void AttemptStoppingUsingSearchbar() => _searchBar.AttemptStoppingUsingSearchbar();
}