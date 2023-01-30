using ImproveGame.Interface.BannerChest.Elements;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.BannerChest
{
    public enum StorageType { Banners, Potions }

    public class PackageGUI : ViewBody
    {
        public static StorageType StorageType { get; private set; }

        private static bool _visible;
        public override bool Display { get => Visible; set => Visible = value; }

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

        private SUIPanel _mainPanel, _titlePanel;
        private SUITitle _title;
        private SUISwitch _autoStorageSwitch, _autoSortSwitch;
        private SUICross _cross;
        private PackageGrid _grid;

        public override void OnInitialize()
        {
            _mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Shaded = true,
                Draggable = true
            };
            _mainPanel.SetPadding(0);
            _mainPanel.Join(this);

            _titlePanel = new SUIPanel(UIColor.TitleBg2, UIColor.PanelBorder, new Vector4(10, 10, 0, 0), 2);
            _titlePanel.SetPadding(0);
            _titlePanel.Width.Precent = 1f;
            _titlePanel.Height.Pixels = 50f;
            _titlePanel.Join(_mainPanel);

            _title = new SUITitle("中文|Chinese", 0.5f)
            {
                VAlign = 0.5f
            };
            _title.SetPadding(20, 0, 10, 0);
            _title.SetInnerPixels(_title.textSize);
            _title.Join(_titlePanel);

            _cross = new SUICross(24)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Rounded = new Vector4(0, 10, 0, 0),
                beginBg = UIColor.TitleBg2 * 0.5f,
                endBg = UIColor.TitleBg2,
            };
            _cross.Height.Set(0f, 1f);
            _cross.OnMouseDown += (_, _) => Close();
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
            _grid.OnMouseDown += (_, _) =>
            {
                if (Main.mouseItem.IsAir)
                    return;
                switch (StorageType)
                {
                    // 旗帜收纳箱, 药水袋子.
                    case StorageType.Banners when ItemToBanner(Main.mouseItem) != -1:
                    case StorageType.Potions when Main.mouseItem.buffType > 0 && Main.mouseItem.consumable:
                        Package.PutInPackage(ref Main.mouseItem);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
            if (!_mainPanel.IsMouseHovering)
            {
                return;
            }

            PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
            Main.LocalPlayer.mouseInterface = true;
        }

        public void Open(List<Item> items, string title, StorageType storageType, IPackageItem package)
        {
            PackageGUI.StorageType = storageType;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            _grid.SetInventory(items);
            _grid.Scrollbar.ViewPosition = 0;
            this._title.Text = title;
            this._title.SetInnerPixels(this._title.textSize);
            this.Package = package;
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
            return (target != this && _mainPanel.IsMouseHovering) || _mainPanel.KeepPressed;
        }
    }
}