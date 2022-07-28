using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using ImproveGame.Interface.UIElements;
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

        private RoundButton modeButton;
        private RoundButton tileButton;
        private RoundButton wallButton;

        public override void OnInitialize()
        {
            base.OnInitialize();

            fixedModeButton = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/FixedMode");
            freeModeButton = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/FreeMode");

            modeButton = new(fixedModeButton);
            modeButton.OnMouseDown += SwitchMode;
            modeButton.OnMouseOver += MouseOver;
            modeButton.OnMouseOut += MouseOut;
            modeButton.OnGetColor += GetColor(() => true);
            Append(modeButton);

            tileButton = new(ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/TileMode"));
            tileButton.OnMouseOver += MouseOver;
            tileButton.OnMouseOut += MouseOut;
            tileButton.OnGetColor += GetColor(() => WandSystem.TileMode);
            tileButton.OnMouseDown += (_, _) => WandSystem.TileMode = !WandSystem.TileMode;
            Append(tileButton);

            wallButton = new(ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/WallMode"));
            wallButton.OnMouseOver += MouseOver;
            wallButton.OnMouseOut += MouseOut;
            wallButton.OnGetColor += GetColor(() => WandSystem.WallMode);
            wallButton.OnMouseDown += (_, _) => WandSystem.WallMode = !WandSystem.WallMode;
            Append(wallButton);
        }

        private void MouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            MouseOnMenu = false;
        }

        private void MouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            MouseOnMenu = true;
        }

        private Func<Color> GetColor(Func<bool> white)
        {
            var inactiveColor = new Color(120, 120, 120);
            return () =>
            {
                Color color = white.Invoke() ? Color.White : inactiveColor;
                if (Visible)
                {
                    color *= 1 - AnimationTimer / AnimationTimerMax;
                }
                else
                {
                    color *= AnimationTimer / AnimationTimerMax;
                }
                return color;
            };
        }

        public override void Update(GameTime gameTime)
        {
            // 与蓝图相同的UI关闭机制
            if (Main.LocalPlayer.mouseInterface && !MouseOnMenu)
            {
                Close();
            }

            if (Main.LocalPlayer.dead || Main.mouseItem.type > ItemID.None || Main.LocalPlayer.HeldItem?.ModItem is not MagickWand)
            {
                Close();
            }

            if (!_mouseRightPrev && Main.mouseRight)
            {
                Close();
            }

            UpdateAnimation();

            _mouseRightPrev = Main.mouseRight;

            base.Update(gameTime);
            if (wallButton.IsMouseHovering || tileButton.IsMouseHovering || modeButton.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        // 更细动画
        public void UpdateAnimation()
        {
            if (AnimationTimer > 1f)
            {
                AnimationTimer -= AnimationTimer / 5f;
                if (AnimationTimer < 1f)
                {
                    AnimationTimer = 0f;
                }
                UpdateButton();
            }
        }

        // 更新按钮
        public void UpdateButton()
        {
            Vector2 center = modeButton.GetDimensions().Center();
            float length = 44f;
            if (Visible)
            {
                length += AnimationTimer / 4f;
            }
            else
            {
                length += (AnimationTimerMax - AnimationTimer) / 4f;
            }
            tileButton.SetCenter(center + new Vector2(-1, 0) * length).Recalculate();
            wallButton.SetCenter(center + new Vector2(1, 0) * length).Recalculate();
        }

        private void SwitchMode(UIMouseEvent evt, UIElement listeningElement)
        {
            WandSystem.FixedMode = !WandSystem.FixedMode;
            modeButton.mainImage = WandSystem.FixedMode ? fixedModeButton : freeModeButton;
        }

        public readonly float AnimationTimerMax = 100;
        public float AnimationTimer;

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public void Open()
        {
            bool center = PlayerInput.UsingGamepad && Main.SmartCursorWanted;
            int x = center ? Main.screenWidth / 2 : Main.mouseX;
            int y = center ? Main.screenHeight / 2 - 60 : Main.mouseY;
            MyUtils.TransformToUIPosition(ref x, ref y);
            AnimationTimer = AnimationTimerMax;
            modeButton.SetCenter(x, y);
            modeButton.mainImage = WandSystem.FixedMode ? fixedModeButton : freeModeButton;
            Visible = true;
            _mouseRightPrev = true; // 防止一打开就关闭
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public void Close()
        {
            if (!Visible)
                return;
            AnimationTimer = AnimationTimerMax;
            Visible = false;
            Main.blockInput = false;
        }
    }
}
