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
        public RoundButton[] RoundButtons;

        public SpaceWand SpaceWand;

        public override void OnInitialize()
        {
            MainPanel = new();
            MainPanel.SetPadding(0);
            MainPanel.Width.Pixels = 120;
            MainPanel.Height.Pixels = 120;
            Append(MainPanel);

            RoundButtons = new RoundButton[6];

            Main.instance.LoadItem(94);
            RoundButtons[0] = new(TextureAssets.Item[94]);
            RoundButtons[0].getColor = GetColor(SpaceWand.PlaceType.platform);
            RoundButtons[0].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.platform);
            MainPanel.Append(RoundButtons[0]);

            Main.instance.LoadItem(9);
            RoundButtons[1] = new(TextureAssets.Item[9]);
            RoundButtons[1].getColor = GetColor(SpaceWand.PlaceType.soild);
            RoundButtons[1].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.soild);
            MainPanel.Append(RoundButtons[1]);

            Main.instance.LoadItem(2996);
            RoundButtons[2] = new(TextureAssets.Item[2996]);
            RoundButtons[2].getColor = GetColor(SpaceWand.PlaceType.rope);
            RoundButtons[2].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.rope);
            MainPanel.Append(RoundButtons[2]);

            Main.instance.LoadItem(2340);
            RoundButtons[3] = new(TextureAssets.Item[2340]);
            RoundButtons[3].getColor = GetColor(SpaceWand.PlaceType.rail);
            RoundButtons[3].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.rail);
            MainPanel.Append(RoundButtons[3]);

            Main.instance.LoadItem(62);
            RoundButtons[4] = new(TextureAssets.Item[62]);
            RoundButtons[4].getColor = GetColor(SpaceWand.PlaceType.grassSeed);
            RoundButtons[4].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.grassSeed);
            MainPanel.Append(RoundButtons[4]);

            Main.instance.LoadItem(3215);
            RoundButtons[5] = new(TextureAssets.Item[3215]);
            RoundButtons[5].getColor = GetColor(SpaceWand.PlaceType.plantPot);
            RoundButtons[5].OnClick += (evt, uie) => ModifyPlaceType(SpaceWand.PlaceType.plantPot);
            MainPanel.Append(RoundButtons[5]);
        }

        private Func<Color> GetColor(SpaceWand.PlaceType placeType)
        {
            return () =>
            {
                Color color = SpaceWand.placeType == placeType ? Color.White : Color.Gray;
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
        }

        public void ModifyPlaceType(SpaceWand.PlaceType placeType)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            SpaceWand.placeType = placeType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SpaceWand is not null)
            {
                if (Main.LocalPlayer.HeldItem != SpaceWand.Item && mode is not UIMode.close)
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
            if (mode is not UIMode.close && flag)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            UpdateAnimation();
        }

        // 更细动画
        public void UpdateAnimation()
        {
            if (isOpen || isClose)
            {
                animationTimer -= animationTimer / 5f;
                if (animationTimer < 1f)
                {
                    animationTimer = 0f;
                    if (mode == UIMode.open) mode = UIMode.normal;
                    if (mode == UIMode.close) Visible = false;
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
                if (isOpen)
                {
                    // angle -= animationTimer / 60f;
                    length += animationTimer / 4f;
                }
                else if (isClose)
                {
                    // angle -= (animationTimerMax - animationTimer) / 60f;
                    length += (animationTimerMax - animationTimer) / 4f;
                }
                RoundButton button = RoundButtons[i];
                button.SetCenter(center + angle.ToRotationVector2() * length);
                button.Recalculate();
            }
        }

        public bool isOpen => mode == UIMode.open;
        public bool isClose => mode == UIMode.close;
        public enum UIMode { open, close, normal }
        public UIMode mode;
        public readonly float animationTimerMax = 100;
        public float animationTimer;
        public void Open()
        {
            mode = UIMode.open;
            MainPanel.SetCenter(TransformToUIPosition(Main.MouseScreen));
            MainPanel.Recalculate();
            animationTimer = animationTimerMax;

            UpdateButton();
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Visible = true;
        }

        public void Close()
        {
            mode = UIMode.close;
            animationTimer = animationTimerMax;

            SoundEngine.PlaySound(SoundID.MenuClose);
        }

        public void ToggleMode(SpaceWand spaceWand)
        {
            SpaceWand = spaceWand;
            if (Visible && mode != UIMode.close)
                Close();
            else
                Open();
        }
    }
}
