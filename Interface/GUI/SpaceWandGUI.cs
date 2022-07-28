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
        public AnimationTimer timer;

        // 这么写能剩下很多重复的代码, 但是你必须保证他们长度是相同的.
        public readonly RoundButton[] RoundButtons = new RoundButton[6];
        public readonly int[] itemTypes = new int[] { 94, 9, 2996, 2340, 62, 3215 };
        public readonly PlaceType[] placeTypes = new PlaceType[] { PlaceType.platform, PlaceType.soild, PlaceType.rope, PlaceType.rail, PlaceType.grassSeed, PlaceType.plantPot };
        public override void OnInitialize()
        {
            timer = new() { OnCloseComplete = () => Visible = false };

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
                    OnGetColor = () =>
                    {
                        Color color = SpaceWand.placeType == placeType ? Color.White : Color.Gray;
                        color *= timer.Schedule;
                        return color;
                    }
                });
                RoundButtons[i].OnMouseDown += (_, _) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    SpaceWand.placeType = placeType;
                };
            }
        }

        private Color textColor = new(135, 0, 180);
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (!timer.AnyClose) // 确保在关闭的时候不显示
            {
                foreach (RoundButton button in RoundButtons) button.round.Draw();
            }
            base.DrawChildren(spriteBatch);
            if (!timer.AnyClose)
            {
                foreach (RoundButton button in RoundButtons)
                {
                    // 悬浮文本
                    if (button.IsMouseHovering)
                        DrawString(MouseScreenOffset(20), button.Text, Color.White, textColor);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update();
            base.Update(gameTime);
            if (Main.LocalPlayer.HeldItem != SpaceWand?.Item && !timer.AnyClose)
            {
                Close();
            }
            foreach (var button in RoundButtons)
            {
                if (button.IsMouseHovering && timer.AnyOpen)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
            UpdateButton();
        }

        // 更新按钮
        public void UpdateButton()
        {
            Vector2 center = MainPanel.GetSizeInside() / 2f;
            float includedAngle = MathF.PI * 2 / RoundButtons.Length; // 夹角
            float startAngle = -MathF.PI / 2 - includedAngle / 2; // 起始角度
            for (int i = 0; i < RoundButtons.Length; i++)
            {
                float angle = startAngle + includedAngle * i;
                float length = 48 + (1 - timer.Schedule) * 25f;
                RoundButtons[i].SetCenter(center + angle.ToRotationVector2() * length).Recalculate();
            }
        }

        public void Open(SpaceWand SpaceWand)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            timer.Open();
            this.SpaceWand = SpaceWand;
            MainPanel.SetCenter(MouseScreenUI).Recalculate();
            UpdateButton();
            Visible = true;
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            timer.Close();
        }
    }
}
