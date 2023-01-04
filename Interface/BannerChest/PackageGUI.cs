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

        public IPackageItem package;

        public SUIPanel mainPanel;
        public SUITitle title;
        public SUISwitch switch1;
        public SUISwitch switch2;
        public SUICross cross;
        public View gridView;
        public PackageGrid grid;

        public override void OnInitialize()
        {
            mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Shaded = true,
                Draggable = true
            };
            mainPanel.SetPadding(0);
            mainPanel.Join(this);

            SUIPanel titlePanel = new SUIPanel(UIColor.TitleBg2, new Vector4(10, 10, 0, 0));
            titlePanel.SetPadding(0);
            titlePanel.Width.Precent = 1f;
            titlePanel.Height.Pixels = 50f;
            titlePanel.Join(mainPanel);

            title = new SUITitle("中文|Chinese", 0.5f)
            {
                VAlign = 0.5f,
                background = Color.Transparent
            };
            title.SetPadding(20, 0, 10, 0);
            title.SetInnerPixels(title.textSize);
            title.Join(titlePanel);

            cross = new SUICross(30)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                // borderColor = Color.White,
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(0, 10, 0, 0)
            };
            cross.Height.Set(0f, 1f);
            cross.OnMouseDown += (_, _) => Close();
            cross.Join(titlePanel);

            switch1 = new SUISwitch(() => package.AutoStorage, state => package.AutoStorage = state, GetText("PackageGUI.AutoStorage"), 0.8f);
            switch1.SetPosPixels(10, titlePanel.Bottom() + 8f).Join(mainPanel);

            switch2 = new SUISwitch(() => package.AutoSort, state => package.AutoSort = state, GetText("PackageGUI.AutoSort"), 0.8f);
            switch2.SetPosPixels(switch1.Right() + 8f, switch1.Top.Pixels).Join(mainPanel);

            gridView = new View() { DragIgnore = true };
            gridView.Top.Pixels = switch1.Bottom() + 8f;
            gridView.SetPadding(14f, 0f, 14f, 14f);
            gridView.OnMouseDown += DownGrid;
            grid = new PackageGrid() { DragIgnore = true };
            grid.Join(gridView);
            gridView.SetInnerPixels(grid.Width.Pixels, grid.Height.Pixels).Join(mainPanel);

            mainPanel.SetInnerPixels(gridView.Width.Pixels, gridView.Bottom());
        }

        private void DownGrid(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.mouseItem.IsAir)
                return;
            // 旗帜收纳箱
            if (storageType is StorageType.Banners && ItemToBanner(Main.mouseItem) != -1)
                package.PutInPackage(ref Main.mouseItem);
            // 药水袋子
            else if (storageType is StorageType.Potions && Main.mouseItem.buffType > 0 && Main.mouseItem.consumable)
                package.PutInPackage(ref Main.mouseItem);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public void Open(List<Item> items, string title, StorageType storageType, IPackageItem package)
        {
            PackageGUI.storageType = storageType;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            grid.SetInventory(items);
            grid.scrollbar.ViewPosition = 0;
            this.title.Text = title;
            this.title.SetInnerPixels(this.title.textSize);
            this.package = package;
            Recalculate();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}
