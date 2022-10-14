using ImproveGame.Content.Items;
using ImproveGame.Interface.UIElements_Shader;
using System.Collections.Generic;
using Terraria.GameInput;

namespace ImproveGame.Interface.BannerChestUI
{
    public class PackageGUI : UIState
    {
        private static bool visible;
        public static StorageType storageType;

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
        public BackgroundImage button;
        public SUIPanel gridPanel;
        public PackageGrid grid;

        private readonly Color background = new(44, 57, 105, 160);
        public override void OnInitialize()
        {
            Append(mainPanel = new(Color.Black, background) { HAlign = 0.5f, VAlign = 0.5f });
            mainPanel.Append(title = new("Title", 0.5f));
            mainPanel.OnMouseDown += (uie, evt) =>
            {
                if (!(button.IsMouseHovering || grid.Parent.IsMouseHovering))
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

            mainPanel.Append(button = new(GetTexture("Close").Value) { HAlign = 1f });
            button.Height.Pixels = title.Height.Pixels;
            button.OnMouseDown += (_, _) =>
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Visible = false;
            };

            mainPanel.Append(gridPanel = new(Color.Black, new(35, 40, 83, 160), 12, 3, false));
            gridPanel.Append(grid = new());
            gridPanel.OnMouseDown += GridPanel_OnMouseDown;
            gridPanel.Top.Pixels = title.Bottom() + 10;
            gridPanel.Width.Pixels = grid.Width.Pixels + gridPanel.HPadding();
            gridPanel.Height.Pixels = grid.Height.Pixels + gridPanel.VPadding();

            mainPanel.Width.Pixels = gridPanel.Width.Pixels + mainPanel.HPadding();
            mainPanel.Height.Pixels = gridPanel.Bottom() + mainPanel.VPadding() - 1;
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
                BannerChest.PutInBannerChest(items, ref Main.mouseItem);
            }
            // 药水袋子
            else if (storageType is StorageType.Potions && Main.mouseItem.buffType > 0)
            {
                PotionBag.PutInPotionBag(items, ref Main.mouseItem);
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

        public void Open(List<Item> items, string title, StorageType STOType)
        {
            storageType = STOType;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            grid.SetInventory(items);
            grid.scrollbar.ViewPosition = 0;
            this.title.Text = title;
            Recalculate();
        }

        public enum StorageType
        {
            Banners,
            Potions
        }
    }
}
