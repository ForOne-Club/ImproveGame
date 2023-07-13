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
        public SpaceWand SpaceWand;
        public AnimationTimer timer; // 这是一个计时器哦~

        // 这么写能剩下很多重复的代码, 但是你必须保证他们长度是相同的.
        public readonly RoundButton[] RoundButtons = new RoundButton[6];
        public readonly int[] itemTypes = new int[] { 94, 9, 2996, 2340, 62, 3215 };
        public readonly PlaceType[] placeTypes = new PlaceType[] { PlaceType.Platform, PlaceType.Soild, PlaceType.Rope, PlaceType.Rail, PlaceType.GrassSeed, PlaceType.PlantPot };
        public override void OnInitialize()
        {
            timer = new() { OnClosed = () => Visible = false };

            Append(MainPanel = new());
            MainPanel.SetSize(200f, 200f).SetPadding(0);

            for (int i = 0; i < RoundButtons.Length; i++)
            {
                int itemType = itemTypes[i];
                PlaceType placeType = placeTypes[i];
                Main.instance.LoadItem(itemType);
                MainPanel.Append(RoundButtons[i] = new(TextureAssets.Item[itemType])
                {
                    text = () => GetText($"SpaceWandGUI.{placeType}"),
                    Selected = () => SpaceWand.PlaceType == placeType
                });
                RoundButtons[i].OnLeftMouseDown += (_, _) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    SpaceWand.PlaceType = placeType;
                };
            }
        }

        private Color textColor = new(135, 0, 180);

        public override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
            if (!timer.AnyClose)
            {
                foreach (RoundButton button in RoundButtons)
                {
                    // 悬浮文本
                    if (button.IsMouseHovering)
                    {
                        DrawString(MouseScreenOffset(20), button.Text, Color.White, textColor);
                        Main.LocalPlayer.cursorItemIconEnabled = false;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update();
            base.Update(gameTime);
            if (Main.LocalPlayer.HeldItem != SpaceWand.Item && !timer.AnyClose)
            {
                Close();
            }
            UpdateButton();
        }

        public void UpdateButton()
        {
            Vector2 center = MainPanel.GetInnerSizePixels() / 2f;
            float includedAngle = MathF.PI * 2 / RoundButtons.Length; // 夹角
            float startAngle = -MathF.PI / 2 - includedAngle / 2; // 起始角度
            for (int i = 0; i < RoundButtons.Length; i++)
            {
                if (RoundButtons[i].IsMouseHovering)
                    Main.LocalPlayer.mouseInterface = true;
                float angle = startAngle + includedAngle * i;
                float length = 48 + (1 - timer.Schedule) * 25f;
                RoundButtons[i].Opacity = timer.Schedule;
                RoundButtons[i].SetCenterPixels(center + angle.ToRotationVector2() * length).Recalculate();
            }
        }

        public void Open(SpaceWand SpaceWand)
        {
            this.SpaceWand = SpaceWand;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            timer.OpenAndReset();
            MainPanel.SetCenterPixels(MouseScreenUI).Recalculate();
            UpdateButton();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            timer.CloseAndReset();
        }
    }
}
