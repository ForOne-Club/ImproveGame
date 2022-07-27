using ImproveGame.Common.Animations;
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
        public SpaceWand SpaceWand;
        public AnimationTimer timer;

        public SpaceWandGUI()
        {
            RoundButtons = new RoundButton[6];
        }

        public override void OnInitialize()
        {
            timer = new()
            {
                OnCloseComplete = () => Visible = false
            };

            MainPanel = new();
            MainPanel.SetPadding(0);
            MainPanel.Width.Pixels = 200;
            MainPanel.Height.Pixels = 200;
            Append(MainPanel);

            Main.instance.LoadItem(94);
            RoundButtons[0] = new(TextureAssets.Item[94]);
            RoundButtons[0].text = () => GetText("SpaceWandGUI.platform");
            RoundButtons[0].OnGetColor = GetColor(PlaceType.platform);
            RoundButtons[0].OnClick += (evt, uie) => ModifyPlaceType(PlaceType.platform);

            Main.instance.LoadItem(9);
            RoundButtons[1] = new(TextureAssets.Item[9]);
            RoundButtons[1].text = () => GetText("SpaceWandGUI.soild");
            RoundButtons[1].OnGetColor = GetColor(PlaceType.soild);
            RoundButtons[1].OnClick += (evt, uie) => ModifyPlaceType(PlaceType.soild);

            Main.instance.LoadItem(2996);
            RoundButtons[2] = new(TextureAssets.Item[2996]);
            RoundButtons[2].text = () => GetText("SpaceWandGUI.rope");
            RoundButtons[2].OnGetColor = GetColor(PlaceType.rope);
            RoundButtons[2].OnClick += (evt, uie) => ModifyPlaceType(PlaceType.rope);

            Main.instance.LoadItem(2340);
            RoundButtons[3] = new(TextureAssets.Item[2340]);
            RoundButtons[3].text = () => GetText("SpaceWandGUI.rail");
            RoundButtons[3].OnGetColor = GetColor(PlaceType.rail);
            RoundButtons[3].OnClick += (evt, uie) => ModifyPlaceType(PlaceType.rail);

            Main.instance.LoadItem(62);
            RoundButtons[4] = new(TextureAssets.Item[62]);
            RoundButtons[4].text = () => GetText("SpaceWandGUI.grassSeed");
            RoundButtons[4].OnGetColor = GetColor(PlaceType.grassSeed);
            RoundButtons[4].OnClick += (evt, uie) => ModifyPlaceType(PlaceType.grassSeed);

            Main.instance.LoadItem(3215);
            RoundButtons[5] = new(TextureAssets.Item[3215])
            {
                text = () => GetText($"SpaceWandGUI.{PlaceType.plantPot}"),
                OnGetColor = GetColor(PlaceType.plantPot)
            };
            RoundButtons[5].OnClick += (evt, uie) => ModifyPlaceType(PlaceType.plantPot);

            MainPanel.AppendS(RoundButtons);
        }

        private Func<Color> GetColor(PlaceType placeType)
        {
            return () =>
            {
                Color color = SpaceWand.placeType == placeType ? Color.White : Color.Gray;
                color *= timer.Schedule;
                return color;
            };
        }

        public void ModifyPlaceType(PlaceType placeType)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            SpaceWand.placeType = placeType;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (!timer.IsInitial)
                foreach (RoundButton button in RoundButtons)
                {
                    // 一对一动画
                    AnimationTimer timer = button.timer;
                    Round round = button.round;
                    round.Draw();
                    if (button.IsMouseHovering && !timer.IsClose)
                    {
                        timer.Close();
                    }
                    else if (!button.IsMouseHovering && timer.InCloseComplete)
                    {
                        timer.State = AnimationState.Initial;
                    }
                }
            base.DrawChildren(spriteBatch);
            if (!timer.IsInitial)
                foreach (RoundButton button in RoundButtons)
                {
                    // 悬浮文本
                    if (button.IsMouseHovering)
                        DrawString(Main.MouseScreen + new Vector2(20), button.text?.Invoke(), Color.White, new Color(135, 0, 180));
                }
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update();
            base.Update(gameTime);
            if (SpaceWand is not null)
            {
                if (Main.LocalPlayer.HeldItem != SpaceWand.Item && timer.IsOpen)
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
            if (timer.IsOpen && (flag /*|| ModeButton.IsMouseHovering*/))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            UpdateButton();
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
                float length = 48 + (1 - timer.Schedule) * 25f;
                RoundButton button = RoundButtons[i];
                button.SetCenter(center + angle.ToRotationVector2() * length);
                button.Recalculate();
            }
        }

        public void Open(SpaceWand spaceWand)
        {
            timer.Open();
            SpaceWand = spaceWand;
            MainPanel.SetCenter(TransformToUIPosition(Main.MouseScreen));
            MainPanel.Recalculate();
            UpdateButton();
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Visible = true;
        }

        public void Close()
        {
            timer.Close();
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}
