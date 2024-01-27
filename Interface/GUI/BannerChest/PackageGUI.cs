using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI.BannerChest.Elements;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI.BannerChest;

public enum StorageType { Banners, Potions }

public class PackageGUI : BaseBody
{
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

    public IPackageItem Package;

    private SUIPanel _mainPanel;
    private View _titlePanel;
    private SUITitle _title;
    private SUISwitch _autoStorageSwitch, _autoSortSwitch;
    private SUICross _cross;
    private PackageGrid _grid;

    public override void OnInitialize()
    {
        // 窗口
        _mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            Shaded = true,
            Draggable = true
        };
        _mainPanel.SetPadding(0);
        _mainPanel.Join(this);

        // 标题容器
        _titlePanel = new View
        {
            DragIgnore = true,
            BgColor = UIColor.TitleBg2,
            Border = 2f,
            BorderColor = UIColor.PanelBorder,
            Rounded = new Vector4(10f, 10f, 0f, 0f),
        };
        _titlePanel.SetPadding(0);
        _titlePanel.Width.Precent = 1f;
        _titlePanel.Height.Pixels = 50f;
        _titlePanel.Join(_mainPanel);

        // 标题
        _title = new SUITitle("中文|Chinese", 0.5f)
        {
            VAlign = 0.5f
        };
        _title.SetPadding(20f, 0f, 10f, 0f);
        _title.SetInnerPixels(_title.TextSize);
        _title.Join(_titlePanel);

        // 关闭按钮
        _cross = new SUICross
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Rounded = new Vector4(0f, 10f, 0f, 0f)
        };
        _cross.Height.Set(0f, 1f);
        _cross.OnLeftMouseDown += (_, _) => Close();
        _cross.Join(_titlePanel);

        _autoStorageSwitch = new SUISwitch(() => Package.AutoStorage, state => Package.AutoStorage = state,
            GetText("PackageGUI.AutoStorage"), 0.8f);
        _autoStorageSwitch.SetPosPixels(10, _titlePanel.Bottom() + 8f).Join(_mainPanel);

        _autoSortSwitch = new SUISwitch(() => Package.AutoSort, state => Package.AutoSort = state,
            GetText("PackageGUI.AutoSort"), 0.8f);
        _autoSortSwitch.SetPosPixels(_autoStorageSwitch.Right() + 8f, _autoStorageSwitch.Top.Pixels)
            .Join(_mainPanel);

        _grid = new PackageGrid();
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
                    Package.PutInPackage(ref Main.mouseItem);
                    break;
            }
        };
        _grid.Join(_mainPanel);

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

    public void Open(List<Item> items, string title, StorageType storageType, IPackageItem package)
    {
        StorageType = storageType;
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Main.playerInventory = true;
        Visible = true;
        _grid.SetInventory(items);
        _grid.Scrollbar.BarTop = 0;
        _title.Text = title;
        _title.SetInnerPixels(_title.TextSize);
        Package = package;
        Recalculate();
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Visible = false;
    }

    public override bool CanPriority(UIElement target) => target != this;

    public override bool CanDisableMouse(UIElement target)
    {
        return target != this && _mainPanel.IsMouseHovering || _mainPanel.KeepPressed;
    }
}