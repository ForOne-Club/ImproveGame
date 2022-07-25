using ImproveGame.Content.Items;
using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI
{
    public class SpaceWandGUI : UIState
    {
        private static bool visible;
        public static bool Visible
        {
            get => visible;
            set => visible = value;
        }

        public UIElement MainPanel;
        public readonly RoundButton[] RoundButtons;
        // public RoundButton ModeButton;
        public SpaceWand SpaceWand;

        public SpaceWandGUI()
        {
            RoundButtons = new RoundButton[6];
        }

        public override void OnInitialize()
        {
            MainPanel = new();
            MainPanel.SetPadding(0);
            MainPanel.Width.Pixels = 200;
            MainPanel.Height.Pixels = 200;
            Append(MainPanel);

            Main.instance.LoadItem(94);
            RoundButtons[0] = new(TextureAssets.Item[94]);
            RoundButtons[0].OnGetColor = GetColor(SpaceWand.PlaceType.platform);
            RoundButtons[0].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.platform);
            MainPanel.Append(RoundButtons[0]);

            Main.instance.LoadItem(9);
            RoundButtons[1] = new(TextureAssets.Item[9]);
            RoundButtons[1].OnGetColor = GetColor(SpaceWand.PlaceType.soild);
            RoundButtons[1].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.soild);
            MainPanel.Append(RoundButtons[1]);

            Main.instance.LoadItem(2996);
            RoundButtons[2] = new(TextureAssets.Item[2996]);
            RoundButtons[2].OnGetColor = GetColor(SpaceWand.PlaceType.rope);
            RoundButtons[2].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.rope);
            MainPanel.Append(RoundButtons[2]);

            Main.instance.LoadItem(2340);
            RoundButtons[3] = new(TextureAssets.Item[2340]);
            RoundButtons[3].OnGetColor = GetColor(SpaceWand.PlaceType.rail);
            RoundButtons[3].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.rail);
            MainPanel.Append(RoundButtons[3]);

            Main.instance.LoadItem(62);
            RoundButtons[4] = new(TextureAssets.Item[62]);
            RoundButtons[4].OnGetColor = GetColor(SpaceWand.PlaceType.grassSeed);
            RoundButtons[4].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.grassSeed);
            MainPanel.Append(RoundButtons[4]);

            Main.instance.LoadItem(3215);
            RoundButtons[5] = new(TextureAssets.Item[3215]);
            RoundButtons[5].OnGetColor = GetColor(SpaceWand.PlaceType.plantPot);
            RoundButtons[5].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.plantPot);
            MainPanel.Append(RoundButtons[5]);

            /*ModeButton = new(GetTexture("UI/SpaceWand/FreeMode"));
            ModeButton.SetCenter(MainPanel.GetSizeInside() / 2f);
            ModeButton.getColor = () =>
            {
                Color color = Color.White;
                if (mode == UIMode.open)
                {
                    color *= 1 - animationTimer / animationTimerMax;
                }
                else if (mode == UIMode.close)
                {
                    color *= animationTimer / animationTimerMax;
                }
                return color;
            };
            MainPanel.Append(ModeButton);*/
        }

        private Func<Color> GetColor(SpaceWand.PlaceType placeType)
        {
            return () =>
            {
                Color color = SpaceWand.placeType == placeType ? Color.White : Color.Gray;
                if (IsOpen)
                    color *= 1 - animationTimer / animationTimerMax;
                else if (IsClose)
                    color *= animationTimer / animationTimerMax;
                return color;
            };
        }

        public void ModifyPlaceType(SpaceWand.PlaceType placeType)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            SpaceWand.placeType = placeType;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible) base.Draw(spriteBatch);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
            if (RoundButtons[0].IsMouseHovering)
            {
                MyUtils.DrawString(Main.MouseScreen + new Vector2(20), GetText("SpaceWandGUI.platform"), Color.White, new Color(135, 0, 180));
            }
            else if (RoundButtons[1].IsMouseHovering)
            {
                MyUtils.DrawString(Main.MouseScreen + new Vector2(20), GetText("SpaceWandGUI.soild"), Color.White, new Color(135, 0, 180));
            }
            else if (RoundButtons[2].IsMouseHovering)
            {
                MyUtils.DrawString(Main.MouseScreen + new Vector2(20), GetText("SpaceWandGUI.rope"), Color.White, new Color(135, 0, 180));
            }
            else if (RoundButtons[3].IsMouseHovering)
            {
                MyUtils.DrawString(Main.MouseScreen + new Vector2(20), GetText("SpaceWandGUI.rail"), Color.White, new Color(135, 0, 180));
            }
            else if (RoundButtons[4].IsMouseHovering)
            {
                MyUtils.DrawString(Main.MouseScreen + new Vector2(20), GetText("SpaceWandGUI.grassSeed"), Color.White, new Color(135, 0, 180));
            }
            else if (RoundButtons[5].IsMouseHovering)
            {
                MyUtils.DrawString(Main.MouseScreen + new Vector2(20), GetText("SpaceWandGUI.plantPot"), Color.White, new Color(135, 0, 180));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SpaceWand is not null)
            {
                if (Main.LocalPlayer.HeldItem != SpaceWand.Item && Mode is not UIMode.Close)
                {
                    Close();
                }
            }

            bool flag = false;
            foreach (var button in RoundButtons)
            {
                if (button.IsMouseHovering)
                    flag = true;
            }
            if (Mode is not UIMode.Close && (flag /*|| ModeButton.IsMouseHovering*/))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            UpdateAnimation();
        }

        // 更细动画
        public void UpdateAnimation()
        {
            if (IsOpen || IsClose)
            {
                animationTimer -= animationTimer / 5f;
                if (animationTimer < 1f)
                {
                    animationTimer = 0f;
                    if (Mode == UIMode.Open) Mode = UIMode.Normal;
                    if (Mode == UIMode.Close) Visible = false;
                }
                UpdateButton();
            }
        }

        // 更新按钮
        public void UpdateButton()
        {
            float includedAngle = MathF.PI * 2 / RoundButtons.Length; // 夹角
            float startAngle = -MathF.PI / 2 - includedAngle / 2; // 起始角度
            for (int i = 0; i < RoundButtons.Length; i++)
            {
                Vector2 center = MainPanel.GetSizeInside() / 2f;
                float angle = startAngle + includedAngle * i;
                float length = 48f;
                if (IsOpen)
                {
                    // angle -= animationTimer / 60f;
                    length += animationTimer / 4f;
                }
                else if (IsClose)
                {
                    // angle -= (animationTimerMax - animationTimer) / 60f;
                    length += (animationTimerMax - animationTimer) / 4f;
                }
                RoundButton button = RoundButtons[i];
                button.SetCenter(center + angle.ToRotationVector2() * length);
                button.Recalculate();
            }
        }

        public bool IsOpen => Mode == UIMode.Open;
        public bool IsClose => Mode == UIMode.Close;
        public enum UIMode { Open, Close, Normal }
        public UIMode Mode;
        public readonly float animationTimerMax = 100;
        public float animationTimer;
        public void Open(SpaceWand spaceWand)
        {
            SpaceWand = spaceWand;
            Mode = UIMode.Open;
            MainPanel.SetCenter(TransformToUIPosition(Main.MouseScreen));
            MainPanel.Recalculate();
            animationTimer = animationTimerMax;
            UpdateButton();
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Visible = true;
        }

        public void Close()
        {
            Mode = UIMode.Close;
            animationTimer = animationTimerMax;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
