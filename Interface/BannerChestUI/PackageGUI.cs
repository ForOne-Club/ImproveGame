using ImproveGame.Interface.UIElements_Shader;
using System.Collections.Generic;
using Terraria.GameInput;

namespace ImproveGame.Interface.BannerChestUI
{
    public class PackageGUI : UIState
    {
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

            mainPanel.Append(gridPanel = new(title.background, title.background, 12, 0, false));
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
            List<Item> items = grid.items;
            int bannerID = ItemToBanner(Main.mouseItem);
            if (bannerID == -1) return;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsAir)
                {
                    items.RemoveAt(i);
                    i--;
                    continue;
                }
                if (items[i].type == Main.mouseItem.type && items[i].stack < items[i].maxStack && ItemLoader.CanStack(items[i], Main.mouseItem))
                {
                    int stackAvailable = items[i].maxStack - items[i].stack;
                    int stackAddition = Math.Min(Main.mouseItem.stack, stackAvailable);
                    Main.mouseItem.stack -= stackAddition;
                    items[i].stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                    Recipe.FindRecipes();
                    if (Main.mouseItem.stack <= 0)
                        Main.mouseItem.TurnToAir();
                }
            }
            if (!Main.mouseItem.IsAir && items.Count < 200)
            {
                items.Add(Main.mouseItem.Clone());
                Main.mouseItem.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
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

        public void Open(List<Item> items, string title)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            grid.SetInventory(items);
            grid.scrollbar.ViewPosition = 0;
            this.title.Text = title;
            Recalculate();
        }
    }
}
