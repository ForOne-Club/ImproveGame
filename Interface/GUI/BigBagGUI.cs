using ImproveGame.Interface.UIElements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace ImproveGame.Interface.GUI
{
    public class BigBagGUI : UIState
    {
        private static bool _visible = true;
        private Vector2 offset = Vector2.Zero;
        public bool dragging;
        public static bool Visible {
            get => Main.playerInventory && _visible;
            set => _visible = value;
        }

        public UIPanel MainPanel;
        public UIImageButton CloseButton;
        public JuItemGrid ItemGrid;

        public void SetSuperVault(Item[] items, Vector2 SuperVaultPos) {
            ItemGrid.SetInventory(items);

            MainPanel.SetPos(SuperVaultPos);
            MainPanel.Width.Pixels = MainPanel.HPadding() + ItemGrid.Width();
            MainPanel.Height.Pixels = MainPanel.VPadding() + ItemGrid.Height() + ItemGrid.Top();
            _visible = Visible;
            Recalculate();
        }

        public override void OnInitialize() {
            MainPanel = new UIPanel();
            MainPanel.OnMouseDown += (evt, uie) => {
                if (!ItemGrid.IsMouseHovering && !CloseButton.IsMouseHovering) {
                    dragging = true;
                    offset = evt.MousePosition - uie.GetDimensions().Position();
                }
            };
            MainPanel.OnMouseUp += (evt, uie) => dragging = false;
            MainPanel.OnUpdate += (uie) => {
                if (dragging) {
                    uie.SetPos(Main.MouseScreen - offset);
                    uie.Recalculate();
                }
                if (!Collision.CheckAABBvAABBCollision(uie.GetDimensions().Position(), uie.GetDimensions().ToRectangle().Size(), Vector2.Zero, Main.ScreenSize.ToVector2())) {
                    uie.SetPos(Vector2.Zero);
                    uie.Recalculate();
                }
                if (uie.IsMouseHovering) {
                    Main.LocalPlayer.mouseInterface = true;
                }
            };
            Append(MainPanel);

            UIText title = new(MyUtils.GetText("Keybind.SuperVault"), 0.5f, true);
            title.Top.Pixels = 10f;
            title.SetSize(MyUtils.GetBigTextSize(MyUtils.GetText("Keybind.SuperVault")) * 0.5f);
            MainPanel.Append(title);

            JuButton button1 = new(MyUtils.GetTexture("UI/Quick").Value, Lang.inter[29].Value);
            button1.Top.Pixels = title.Bottom() - 10f;
            button1.Width.Pixels = MyUtils.GetTextSize(Lang.inter[29].Value).X + button1.HPadding() + 75;
            button1.Height.Pixels = 40f;
            button1.OnClick += (evt, uie) => {
                QuickTakeOutToPlayerInventory();
            };
            MainPanel.Append(button1);

            JuButton button2 = new(MyUtils.GetTexture("UI/Put").Value, Lang.inter[30].Value);
            button2.Top.Pixels = button1.Top();
            button2.Width.Pixels = MyUtils.GetTextSize(Lang.inter[30].Value).X + button1.HPadding() + 75;
            button2.Height.Pixels = 40f;
            button2.Left.Pixels = button1.Right() + 10f;
            button2.OnClick += (evt, uie) => {
                PutAll();
            };
            MainPanel.Append(button2);

            JuButton button3 = new(MyUtils.GetTexture("UI/Put").Value, Lang.inter[31].Value);
            button3.Top.Pixels = button1.Top();
            button3.Width.Pixels = MyUtils.GetTextSize(Lang.inter[31].Value).X + button1.HPadding() + 75;
            button3.Height.Pixels = 40f;
            button3.Left.Pixels = button2.Right() + 10f;
            button3.OnClick += (evt, uie) => {
                Replenish();
            };
            MainPanel.Append(button3);

            CloseButton = new(MyUtils.GetTexture("Close")) {
                HAlign = 1f
            };
            CloseButton.Left.Set(-2, 0);
            CloseButton.OnClick += (evt, uie) => Visible = false;
            MainPanel.Append(CloseButton);

            ItemGrid = new JuItemGrid();
            ItemGrid.Top.Pixels = button1.Top() + button1.Height() + 10f;
            MainPanel.Append(ItemGrid);
        }

        public void Replenish() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 58; i++) {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin) {
                    if (MyUtils.HasItem(BigBag, -1, inventory[i].type)) {
                        inventory[i] = MyUtils.ItemStackToInventory(BigBag, inventory[i], false);
                    }
                }
            }
        }

        public void PutAll() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 10; i < 50; i++) {
                if (!inventory[i].IsAir && !inventory[i].favorited && !inventory[i].IsACoin)
                    inventory[i] = MyUtils.ItemStackToInventory(BigBag, inventory[i], false);
            }
        }

        public void QuickTakeOutToPlayerInventory() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item[] inventory = Main.LocalPlayer.inventory;
            Item[] BigBag = ItemGrid.ItemList.items;
            for (int i = 0; i < BigBag.Length; i++) {
                if (!BigBag[i].IsAir && !BigBag[i].favorited && !BigBag[i].IsACoin) {
                    BigBag[i] = MyUtils.ItemStackToInventory(inventory, BigBag[i], false, 50);
                }
            }
        }

        public void Open() {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            dragging = false;
            _visible = true;
        }

        public void Close() {
            SoundEngine.PlaySound(SoundID.MenuClose);
            _visible = false;
        }
    }
}
