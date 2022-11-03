using ImproveGame.Common.Configs;
using ImproveGame.Content.Items;
using ImproveGame.Interface.UIElements_Shader;
using System.Data;
using Terraria.GameInput;

namespace ImproveGame.Interface.BannerChestUI
{
    public class PackageGUI : UIState
    {
        private static bool visible;
        public static StorageType storageType;
        public IPackage package;

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

        private Vector2 offset;
        public bool dragging;

        public SUIPanel mainPanel;
        public UITitle title;
        public Checkbox checkbox;
        public Checkbox checkbox2;
        public UIFork close;
        public SUIPanel gridPanel;
        public PackageGrid grid;

        private readonly Color background = new(44, 57, 105, 160);
        public override void OnInitialize()
        {
            Append(mainPanel = new(Color.Black, background) { HAlign = 0.5f, VAlign = 0.5f });
            mainPanel.SetPadding(12f);
            mainPanel.Append(title = new("中文|Chinese", 0.5f));
            mainPanel.OnMouseDown += (uie, evt) =>
            {
                if (!(close.IsMouseHovering || grid.Parent.IsMouseHovering))
                {
                    dragging = true;
                    offset = Main.MouseScreen - mainPanel.GetPPos();
                }
            };
            mainPanel.OnMouseUp += (_, _) => dragging = false;
            mainPanel.OnUpdate += (_) =>
            {
                if (dragging)
                {
                    mainPanel.SetPos(Main.MouseScreen - offset).Recalculate();
                }
            };

            mainPanel.Append(checkbox = new(() => package.AutoStorage, state => package.AutoStorage = state, GetText("PackageGUI.AutoStorage"), 0.8f));
            checkbox.Top.Pixels = title.Bottom() + 8f;

            mainPanel.Append(checkbox2 = new(() => package.AutoSort, state => package.AutoSort = state, GetText("PackageGUI.AutoSort"), 0.8f));
            checkbox2.Top.Pixels = checkbox.Top();
            checkbox2.Left.Pixels = checkbox.Right() + 8f;

            mainPanel.Append(close = new(30) { HAlign = 1f });
            close.Height.Pixels = title.Height();
            close.OnMouseDown += (_, _) =>
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Visible = false;
            };

            mainPanel.Append(gridPanel = new(Color.Black, new(35, 40, 83, 160), 12, 3, false));
            gridPanel.Append(grid = new());
            gridPanel.OnMouseDown += GridPanel_OnMouseDown;
            gridPanel.Top.Pixels = checkbox.Bottom() + 8f;
            gridPanel.Width.Pixels = grid.Width.Pixels + gridPanel.HPadding();
            gridPanel.Height.Pixels = grid.Height.Pixels + gridPanel.VPadding();

            mainPanel.Width.Pixels = gridPanel.Width.Pixels + mainPanel.HPadding();
            mainPanel.Height.Pixels = gridPanel.Bottom() + mainPanel.VPadding();
        }

        private void GridPanel_OnMouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.mouseItem.IsAir)
            {
                return;
            }
            List<Item> items = grid.items;
            // 旗帜收纳箱
            if (storageType is StorageType.Banners && ItemToBanner(Main.mouseItem) != -1)
            {
                BannerChest.PutInBannerChest(items, ref Main.mouseItem, package.AutoSort);
            }
            // 药水袋子
            else if (storageType is StorageType.Potions && Main.mouseItem.buffType > 0 && Main.mouseItem.consumable)
            {
                PotionBag.PutInPotionBag(items, ref Main.mouseItem, package.AutoSort);
            }
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

        public void Open(List<Item> items, string title, StorageType STOType, IPackage package)
        {
            storageType = STOType;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            grid.SetInventory(items);
            grid.scrollbar.ViewPosition = 0;
            this.title.Text = title;
            this.package = package;
            Recalculate();
        }

        public enum StorageType
        {
            Banners,
            Potions
        }
    }
}
