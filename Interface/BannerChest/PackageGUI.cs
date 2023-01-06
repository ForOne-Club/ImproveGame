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

            SUIPanel titlePanel = new SUIPanel(UIColor.TitleBg2, UIColor.PanelBorder, new Vector4(10, 10, 0, 0), 2);
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

            cross = new SUICross(24)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(0, 10, 0, 0),
                beginBg = UIColor.TitleBg2 * 0.5f,
                endBg = UIColor.TitleBg2,
            };
            cross.Height.Set(0f, 1f);
            cross.OnMouseDown += (_, _) => Close();
            cross.Join(titlePanel);

            switch1 = new SUISwitch(() => package.AutoStorage, state => package.AutoStorage = state, GetText("PackageGUI.AutoStorage"), 0.8f);
            switch1.SetPosPixels(10, titlePanel.Bottom() + 8f).Join(mainPanel);

            switch2 = new SUISwitch(() => package.AutoSort, state => package.AutoSort = state, GetText("PackageGUI.AutoSort"), 0.8f);
            switch2.SetPosPixels(switch1.Right() + 8f, switch1.Top.Pixels).Join(mainPanel);

            // gridView = new View() { DragIgnore = true };
            grid = new PackageGrid() { DragIgnore = true };
            grid.Top.Pixels = switch1.Bottom() + 8f;
            grid.SetPadding(10f, 0f, 9f, 9f).SetInnerPixels(grid.Width.Pixels, grid.Height.Pixels);
            grid.OnMouseDown += DownGrid;
            grid.Join(mainPanel);
            // gridView.SetInnerPixels(grid.Width.Pixels, grid.Height.Pixels).Join(mainPanel);

            mainPanel.SetInnerPixels(grid.Width.Pixels, grid.Bottom());
        }

        /*public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            PixelShader.DrawRoundRect(Main.MouseScreen, new Vector2(200, 100), new Vector4(10, 0, 10, 0),
                UIColor.PanelBg, new Vector4(2, 3, 4, 5), UIColor.PanelBorder, 0f);
        }*/

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
