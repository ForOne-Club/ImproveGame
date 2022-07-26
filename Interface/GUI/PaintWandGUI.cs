using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI
{
    public class PaintWandGUI : UIState
    {
        public static bool Visible;
        public UIElement MainPanel;
        public RoundButton[] RoundButtons;

        public override void OnInitialize()
        {
            MainPanel = new();
            MainPanel.SetPadding(0);
            MainPanel.Width.Pixels = 120;
            MainPanel.Height.Pixels = 120;
            Append(MainPanel);

            Main.instance.LoadItem(ItemID.Paintbrush);
            Main.instance.LoadItem(ItemID.PaintRoller);
            Main.instance.LoadItem(ItemID.PaintScraper);

            RoundButtons = new RoundButton[] {
                new(TextureAssets.Item[ItemID.Paintbrush])
                {
                    OnGetColor = GetColor(() => WandSystem.PaintWandMode == WandSystem.PaintMode.Tile)
                },
                new(TextureAssets.Item[ItemID.PaintRoller])
                {
                    OnGetColor = GetColor(() => WandSystem.PaintWandMode == WandSystem.PaintMode.Wall)
                },
                new(TextureAssets.Item[ItemID.PaintScraper])
                {
                    OnGetColor = GetColor(() => WandSystem.PaintWandMode == WandSystem.PaintMode.Remove)
                }
            };

            RoundButtons[0].OnClick += (_, _) => WandSystem.PaintWandMode = WandSystem.PaintMode.Tile;
            RoundButtons[1].OnClick += (_, _) => WandSystem.PaintWandMode = WandSystem.PaintMode.Wall;
            RoundButtons[2].OnClick += (_, _) => WandSystem.PaintWandMode = WandSystem.PaintMode.Remove;
            MainPanel.Append(RoundButtons[0]);
            MainPanel.Append(RoundButtons[1]);
            MainPanel.Append(RoundButtons[2]);
        }

        private Func<Color> GetColor(Func<bool> white)
        {
            return () =>
            {
                Color color = white.Invoke() ? Color.White : Color.Gray;
                if (Mode == UIMode.Open)
                {
                    color *= 1 - animationTimer / animationTimerMax;
                }
                else if (Mode == UIMode.Close)
                {
                    color *= animationTimer / animationTimerMax;
                }
                return color;
            };
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
            var position = Main.MouseScreen + new Vector2(15);
            var borderColor = new Color(135, 0, 180);
            var textColor = Color.White;
            if (RoundButtons[0].IsMouseHovering)
            {
                DrawString(position, GetText("PaintWandGUI.Paintbrush"), textColor, borderColor);
            }
            else if (RoundButtons[1].IsMouseHovering)
            {
                DrawString(position, GetText("PaintWandGUI.PaintRoller"), textColor, borderColor);
            }
            else if (RoundButtons[2].IsMouseHovering)
            {
                DrawString(position, GetText("PaintWandGUI.PaintScraper"), textColor, borderColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<PaintWand>() && Mode is not UIMode.Close)
            {
                Close();
            }
            bool flag = false;
            foreach (var button in RoundButtons)
            {
                if (button.IsMouseHovering)
                    flag = true;
            }
            if (Mode is not UIMode.Close && flag)
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
                float length = 34f;
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
        public void Open()
        {
            Mode = UIMode.Open;
            MainPanel.SetCenter(TransformToUIPosition(Main.MouseScreen));
            MainPanel.Recalculate();
            animationTimer = animationTimerMax;

            UpdateButton();
            Visible = true;
        }

        public void Close()
        {
            Mode = UIMode.Close;
            animationTimer = animationTimerMax;
        }

        public void ToggleMode()
        {
            if (Visible && Mode != UIMode.Close)
                Close();
            else
                Open();
        }
    }
}
