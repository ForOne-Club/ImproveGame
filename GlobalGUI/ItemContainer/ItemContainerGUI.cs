using ImproveGame.GlobalGUI.ItemContainer.Elements;
using ImproveGame.Interface.Attributes;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.GlobalGUI.ItemContainer;

public enum StorageType { Banners, Potions }

[AutoCreateGUI("Radial Hotbars", "Item Container GUI")]
public class ItemContainerGUI : BaseBody
{
    public static ItemContainerGUI Instace { get; private set; }
    public ItemContainerGUI() => Instace = this;

    public static StorageType StorageType { get; private set; }

    private static bool _visible;
    public override bool Enabled { get => Visible; set => Visible = value; }

    public static bool Visible
    {
        get
        {
            if (!Main.playerInventory)
                _visible = false;
            return _visible;
        }
        set => _visible = value;
    }

    public IItemContainer Container;

    private SUIPanel _mainPanel;
    private View _titlePanel;
    private SUITitle _title;
    private SUISwitch _autoStorageSwitch, _autoSortSwitch;
    private SUICross _cross;
    private ItemContainerGrid _grid;

    public override void OnInitialize()
    {
        // 窗口
        _mainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            Shaded = true,
            Draggable = true
        };
        _mainPanel.SetPadding(0);
        _mainPanel.JoinParent(this);

        // 标题容器
        _titlePanel = new View
        {
            DragIgnore = true,
            BgColor = UIStyle.TitleBg2,
            Border = 2f,
            BorderColor = UIStyle.PanelBorder,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
        };
        _titlePanel.SetPadding(0);
        _titlePanel.Width.Precent = 1f;
        _titlePanel.Height.Pixels = 50f;
        _titlePanel.JoinParent(_mainPanel);

        // 标题
        _title = new SUITitle("中文|Chinese", 0.5f)
        {
            VAlign = 0.5f
        };
        _title.SetPadding(20f, 0f, 10f, 0f);
        _title.SetInnerPixels(_title.TextSize);
        _title.JoinParent(_titlePanel);

        // 关闭按钮
        _cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Rounded = new Vector4(0f, 10f, 0f, 0f)
        };
        _cross.Height.Set(0f, 1f);
        _cross.OnLeftMouseDown += (_, _) => Close();
        _cross.JoinParent(_titlePanel);

        _autoStorageSwitch = new SUISwitch(() => Container.AutoStorage, state => Container.AutoStorage = state,
            GetText("PackageGUI.AutoStorage"), 0.8f);
        _autoStorageSwitch.SetPosPixels(10, _titlePanel.Bottom() + 8f).JoinParent(_mainPanel);

        _autoSortSwitch = new SUISwitch(() => Container.AutoSort, state => Container.AutoSort = state,
            GetText("PackageGUI.AutoSort"), 0.8f);
        _autoSortSwitch.SetPosPixels(_autoStorageSwitch.Right() + 8f, _autoStorageSwitch.Top.Pixels)
            .JoinParent(_mainPanel);

        _grid = new ItemContainerGrid();
        _grid.Top.Pixels = _autoStorageSwitch.Bottom() + 8f;
        _grid.SetPadding(10f, 0f, 9f, 9f).SetInnerPixels(_grid.Width.Pixels, _grid.Height.Pixels);
        _grid.OnLeftMouseDown += (_, _) =>
        {
            if (Main.mouseItem.IsAir || Main.LocalPlayer.ItemAnimationActive)
                return;
            switch (StorageType)
            {
                // 旗帜收纳箱, 药水袋子.
                case StorageType.Banners when ItemToBanner(Main.mouseItem) != -1:
                case StorageType.Potions when Main.mouseItem.buffType > 0 && Main.mouseItem.consumable:
                    Container.PutInPackage(ref Main.mouseItem);
                    break;
            }
        };
        _grid.JoinParent(_mainPanel);

        _mainPanel.SetInnerPixels(_grid.Width.Pixels, _grid.Bottom());
    }

    /*public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        PixelShader.DrawRoundRect(Main.MouseScreen, new Vector2(200, 100), new Vector4(10, 0, 10, 0),
            UIColor.PanelBg, new Vector4(2, 3, 4, 5), UIColor.PanelBorder, 0f);
    }*/

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (_mainPanel.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    public void Open(List<Item> items, string title, StorageType storageType, IItemContainer container)
    {
        StorageType = storageType;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Main.playerInventory = true;
        Visible = true;
        _grid.SetInventory(items);
        _grid.Scrollbar.BarTop = 0;
        _title.Text = title;
        _title.SetInnerPixels(_title.TextSize);
        Container = container;
        Recalculate();
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Visible = false;
    }

    public override bool CanSetFocusTarget(UIElement target)
    {
        return target != this && _mainPanel.IsMouseHovering || _mainPanel.IsLeftMousePressed;
    }
}