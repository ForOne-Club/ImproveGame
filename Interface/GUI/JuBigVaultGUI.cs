using ImproveGame.Common.Systems;
using ImproveGame.Interface.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ImproveGame.Interface.GUI
{
    public class JuBigVaultGUI : UIState
    {
        public static bool _visible = true;
        public static Vector2 offset = Vector2.Zero;
        public static Vector2 position = Vector2.Zero;
        public bool dragging;
        public static bool Visible {
            get {
                return Main.playerInventory && _visible;
            }
            set {
                _visible = value;
            }
        }

        public UIPanel mainPanel;
        public JuItemGrid mainGrid;
        public UIImageButton closeButton;

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="SuperVault"></param>
        /// <param name="SuperVaultOffset"></param>
        public void JuInitialize(Item[] SuperVault, Vector2 SuperVaultOffset, bool Visible) {
            RemoveAllChildren();
            _visible = Visible;
            mainPanel = new UIPanel() {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            mainPanel.OnMouseDown += MainPanel_OnMouseDown;
            mainPanel.OnMouseUp += MainPanel_OnMouseUp;
            Append(mainPanel);

            closeButton = new(MyUtils.GetTexture("Close")) {
                HAlign = 1f
            };
            closeButton.Left.Set(-2, 0);
            closeButton.OnClick += CloseButton_OnClick;
            mainPanel.Append(closeButton);

            /*UIText uIText = new("整理");
            uIText.OnClick += UIText_OnClick;
            mainPanel.Append(uIText);*/

            // 加载数据
            mainGrid = new JuItemGrid(SuperVault);
            mainGrid.Top.Set(30, 0f);
            mainPanel.Append(mainGrid);
            mainPanel.Width.Set(mainGrid.Width.Pixels + mainPanel.PaddingLeft + mainPanel.PaddingRight, 0f);
            mainPanel.Height.Set(30 + mainGrid.Height.Pixels + mainPanel.PaddingTop + mainPanel.PaddingBottom, 0f);
            Recalculate();
            SetPosition(SuperVaultOffset);
        }

        public override void OnInitialize() {

        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (dragging) {
                position.X = Main.mouseX - offset.X;
                position.Y = Main.mouseY - offset.Y;
                mainPanel.Left.Set(position.X, 0f);
                mainPanel.Top.Set(position.Y, 0f);
                mainPanel.Recalculate();
            }

            if (mainPanel.IsMouseHovering) {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public void SetPosition(Vector2 pos) {
            position = pos;
            mainPanel.Left.Set(position.X, 0f);
            mainPanel.Top.Set(position.Y, 0f);
            mainPanel.Recalculate();
        }

        private void MainPanel_OnMouseDown(UIMouseEvent evt, UIElement listeningElement) {
            if (!mainGrid.ContainsPoint(evt.MousePosition) && !closeButton.ContainsPoint(evt.MousePosition)) {
                dragging = true;
                offset = new Vector2(evt.MousePosition.X - mainPanel.Left.Pixels, evt.MousePosition.Y - mainPanel.Top.Pixels);
            }
        }

        private void MainPanel_OnMouseUp(UIMouseEvent evt, UIElement listeningElement) {
            dragging = false;
        }

        private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
            Visible = false;
        }
    }
}
