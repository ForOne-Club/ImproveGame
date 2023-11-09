using ImproveGame.Common.Animations;
using ImproveGame.Content.Items;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI
{
    public class SpaceWandGUI : UIState
    {
        private static bool _secondPage;
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
        public readonly int[] ItemTypes = {94, 9, 2996, 2340, 62, 3215};

        public readonly PlaceType[] PlaceTypes =
        {
            PlaceType.Platform, PlaceType.Soild, PlaceType.Rope, PlaceType.Rail, PlaceType.GrassSeed, PlaceType.PlantPot
        };

        public readonly BlockType[] BlockTypes =
        {
            BlockType.SlopeDownRight, BlockType.SlopeDownLeft, BlockType.HalfBlock, BlockType.SlopeUpLeft,
            BlockType.SlopeUpRight, BlockType.Solid
        };

        public override void OnInitialize()
        {
            timer = new() {OnClosed = () => Visible = false};

            Append(MainPanel = new());
            MainPanel.SetSize(200f, 200f).SetPadding(0);

            for (int i = 0; i < RoundButtons.Length; i++)
            {
                int itemType = ItemTypes[i];
                PlaceType placeType = PlaceTypes[i];
                Main.instance.LoadItem(itemType);
                MainPanel.Append(RoundButtons[i] = new(TextureAssets.Item[itemType])
                {
                    text = () => GetText($"SpaceWandGUI.{placeType}"),
                    Selected = () => SpaceWand.PlaceType == placeType
                });
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

        /// <summary>
        /// 我执行右键了，你看着办吧！
        /// </summary>
        public void ProcessRightClick(SpaceWand SpaceWand)
        {
            if (Visible && timer.AnyOpen)
            {
                if (_secondPage)
                {
                    Close();
                }
                else
                {
                    SetupSecondPage();
                }
            }
            else
            {
                Open(SpaceWand);
            }
        }

        private void SetupSecondPage()
        {
            _secondPage = true;

            MainPanel.RemoveAllChildren();

            for (int i = 0; i < RoundButtons.Length; i++)
            {
                BlockType blockType = BlockTypes[i];
                string path = $"UI/SpaceWand/{blockType}";
                MainPanel.Append(RoundButtons[i] = new RoundButton(GetTexture(path))
                {
                    text = () => "",
                    Selected = () => SpaceWand.BlockType == blockType
                });
                RoundButtons[i].OnLeftMouseDown += (_, _) =>
                {
                    SpaceWand.BlockType = blockType;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                };
            }

            Recalculate();
            UpdateButton();
        }

        private void SetupFirstPage()
        {
            _secondPage = false;
            MainPanel.RemoveAllChildren();

            for (int i = 0; i < RoundButtons.Length; i++)
            {
                int itemType = ItemTypes[i];
                PlaceType placeType = PlaceTypes[i];
                Main.instance.LoadItem(itemType);
                MainPanel.Append(RoundButtons[i] = new(TextureAssets.Item[itemType])
                {
                    text = () => GetText($"SpaceWandGUI.{placeType}"),
                    Selected = () => SpaceWand.PlaceType == placeType
                });
                RoundButtons[i].OnLeftMouseDown += (_, _) =>
                {
                    SpaceWand.PlaceType = placeType;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                };
            }
        }

        public void Open(SpaceWand SpaceWand)
        {
            SetupFirstPage();
            this.SpaceWand = SpaceWand;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            timer.OpenAndResetTimer();
            MainPanel.SetCenterPixels(MouseScreenUI).Recalculate();
            UpdateButton();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            timer.CloseAndResetTimer();
        }
    }
}