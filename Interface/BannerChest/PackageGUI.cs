using ImproveGame.Common.Animations;
using ImproveGame.Interface.BannerChest.Elements;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.BannerChest
{
    public class PackageGUI : UIState
    {
        public enum StorageType { Banners, Potions }

        internal static StorageType storageType;

        private static bool visible;

        public static bool Visible
        {
            get
            {
                if (!Main.playerInventory)
                    visible = false;
                return visible;
            }
            set => visible = value;
        }

        public IPackageItem Package;

        public SUIPanel MainPanel, TitlePanel;
        public SUITitle Title;
        public SUISwitch AutoStorageSwitch, AutoSortSwitch;
        public SUICross Cross;
        public PackageGrid Grid;

        public override void OnInitialize()
        {
            MainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Shaded = true,
                Draggable = true
            };
            MainPanel.OnUpdate += _ =>
            {
                if (MainPanel.IsMouseHovering)
                {
                    EventTrigger.LockCursor();
                }
            };
            MainPanel.SetPadding(0);
            MainPanel.Join(this);

            TitlePanel = new SUIPanel(UIColor.TitleBg2, UIColor.PanelBorder, new Vector4(10, 10, 0, 0), 2);
            TitlePanel.SetPadding(0);
            TitlePanel.Width.Precent = 1f;
            TitlePanel.Height.Pixels = 50f;
            TitlePanel.Join(MainPanel);

            Title = new SUITitle("中文|Chinese", 0.5f)
            {
                VAlign = 0.5f,
                background = Color.Transparent
            };
            Title.SetPadding(20, 0, 10, 0);
            Title.SetInnerPixels(Title.textSize);
            Title.Join(TitlePanel);

            Cross = new SUICross(24)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(0, 10, 0, 0),
                beginBg = UIColor.TitleBg2 * 0.5f,
                endBg = UIColor.TitleBg2,
            };
            Cross.Height.Set(0f, 1f);
            Cross.OnMouseDown += (_, _) => Close();
            Cross.Join(TitlePanel);

            AutoStorageSwitch = new SUISwitch(() => Package.AutoStorage, state => Package.AutoStorage = state,
                GetText("PackageGUI.AutoStorage"), 0.8f);
            AutoStorageSwitch.SetPosPixels(10, TitlePanel.Bottom() + 8f).Join(MainPanel);

            AutoSortSwitch = new SUISwitch(() => Package.AutoSort, state => Package.AutoSort = state,
                GetText("PackageGUI.AutoSort"), 0.8f);
            AutoSortSwitch.SetPosPixels(AutoStorageSwitch.Right() + 8f, AutoStorageSwitch.Top.Pixels).Join(MainPanel);

            Grid = new PackageGrid();
            Grid.Top.Pixels = AutoStorageSwitch.Bottom() + 8f;
            Grid.SetPadding(10f, 0f, 9f, 9f).SetInnerPixels(Grid.Width.Pixels, Grid.Height.Pixels);
            Grid.OnMouseDown += (_, _) =>
            {
                if (Main.mouseItem.IsAir)
                    return;
                switch (storageType)
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
            Grid.Join(MainPanel);

            MainPanel.SetInnerPixels(Grid.Width.Pixels, Grid.Bottom());
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
            if (!MainPanel.IsMouseHovering)
            {
                return;
            }

            PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
            Main.LocalPlayer.mouseInterface = true;
        }

        public void Open(List<Item> items, string title, StorageType storageType, IPackageItem package)
        {
            PackageGUI.storageType = storageType;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            Grid.SetInventory(items);
            Grid.Scrollbar.ViewPosition = 0;
            this.Title.Text = title;
            this.Title.SetInnerPixels(this.Title.textSize);
            this.Package = package;
            Recalculate();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}