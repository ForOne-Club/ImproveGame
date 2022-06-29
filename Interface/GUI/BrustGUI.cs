using ImproveGame.Common.Systems;
using ImproveGame.Interface.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class BrustGUI : UIState
    {
        public static bool Visible { get; private set; }
        internal static bool MouseOnMenu;
        private static bool _mouseRightPrev;

        private Asset<Texture2D> fixedModeButton;
        private Asset<Texture2D> freeModeButton;

        private ModImageButton modeButton;
        private ModImageButton tileButton;
        private ModImageButton wallButton;

        public override void OnInitialize() {
            base.OnInitialize();

            fixedModeButton = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/FixedMode");
            freeModeButton = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/FreeMode");
            Asset<Texture2D> hoverImage = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/Hover");
            Asset<Texture2D> backgroundImage = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/Background");
            var inactiveColor = new Color(120, 120, 120);
            var activeColor = Color.White;

            modeButton = new ModImageButton(
                fixedModeButton,
                activeColor: activeColor, inactiveColor: inactiveColor);
            modeButton.SetHoverImage(hoverImage);
            modeButton.SetBackgroundImage(backgroundImage);
            modeButton.Width.Set(40, 0f);
            modeButton.Height.Set(40, 0f);
            modeButton.DrawColor += () => Color.White;
            modeButton.OnMouseDown += SwitchMode;
            modeButton.OnMouseOver += MouseOver;
            modeButton.OnMouseOut += MouseOut;
            Append(modeButton);

            tileButton = new ModImageButton(
                ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/TileMode"),
                activeColor: activeColor, inactiveColor: inactiveColor);
            tileButton.SetHoverImage(hoverImage);
            tileButton.SetBackgroundImage(backgroundImage);
            tileButton.Width.Set(40, 0f);
            tileButton.Height.Set(40, 0f);
            tileButton.OnMouseOver += MouseOver;
            tileButton.OnMouseOut += MouseOut;
            tileButton.DrawColor += () => WandSystem.TileMode ? Color.White : inactiveColor;
            tileButton.OnMouseDown += (_, _) => WandSystem.TileMode = !WandSystem.TileMode;
            Append(tileButton);

            wallButton = new ModImageButton(
                ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/WallMode"),
                activeColor: activeColor, inactiveColor: inactiveColor);
            wallButton.SetHoverImage(hoverImage);
            wallButton.SetBackgroundImage(backgroundImage);
            wallButton.Width.Set(40, 0f);
            wallButton.Height.Set(40, 0f);
            wallButton.OnMouseOver += MouseOver;
            wallButton.OnMouseOut += MouseOut;
            wallButton.DrawColor += () => WandSystem.WallMode ? Color.White : inactiveColor;
            wallButton.OnMouseDown += (_, _) => WandSystem.WallMode = !WandSystem.WallMode;
            Append(wallButton);
        }

        private void MouseOut(UIMouseEvent evt, UIElement listeningElement) {
            MouseOnMenu = false;
        }

        private void MouseOver(UIMouseEvent evt, UIElement listeningElement) {
            MouseOnMenu = true;
        }

        public override void Update(GameTime gameTime) {
            // 与蓝图相同的UI关闭机制
            if (Main.LocalPlayer.mouseInterface && !MouseOnMenu) {
                Close();
                return;
            }

            if (Main.LocalPlayer.dead || Main.mouseItem.type > ItemID.None) {
                Close();
                return;
            }

            if (!_mouseRightPrev && Main.mouseRight) {
                Close();
                return;
            }

            _mouseRightPrev = Main.mouseRight;

            base.Update(gameTime);
        }

        private void SwitchMode(UIMouseEvent evt, UIElement listeningElement) {
            WandSystem.FixedMode = !WandSystem.FixedMode;
            modeButton.SetImage(WandSystem.FixedMode ? fixedModeButton : freeModeButton);
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open() {
            bool center = PlayerInput.UsingGamepad && Main.SmartCursorWanted;
            int x = center ? Main.screenWidth / 2 : Main.mouseX;
            int y = center ? Main.screenHeight / 2 - 60 : Main.mouseY;
            MyUtils.TransformToUIPosition(ref x, ref y);
            modeButton.SetCenter(x, y);
            tileButton.SetCenter(x - 44, y);
            wallButton.SetCenter(x + 44, y);
            modeButton.SetImage(WandSystem.FixedMode ? fixedModeButton : freeModeButton);
            Visible = true;
            _mouseRightPrev = true; // 防止一打开就关闭
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close() {
            Visible = false;
            Main.blockInput = false;
        }
    }
}
