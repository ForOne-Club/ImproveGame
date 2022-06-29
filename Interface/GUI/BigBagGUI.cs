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

            string titleText = MyUtils.GetText("Keybind.SuperVault");
            UIText title = new(titleText, 0.5f, large: true);
            title.Left.Set(4, 0f);
            title.Top.Set(4, 0f);
            title.Width.Set(FontAssets.DeathText.Value.MeasureString(titleText).X * 0.5f, 0f);
            title.Height.Set(30, 0f);
            MainPanel.Append(title);

            CloseButton = new(MyUtils.GetTexture("Close")) {
                HAlign = 1f
            };
            CloseButton.Left.Set(-2, 0);
            CloseButton.OnClick += (evt, uie) => Visible = false;
            MainPanel.Append(CloseButton);

            ItemGrid = new JuItemGrid();
            ItemGrid.Top.Pixels = 30f;
            MainPanel.Append(ItemGrid);
        }

        public void Open() {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            _visible = true;
        }

        public void Close() {
            SoundEngine.PlaySound(SoundID.MenuClose);
            _visible = false;
        }
    }
}
