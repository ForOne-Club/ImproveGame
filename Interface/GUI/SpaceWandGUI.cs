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

        private Vector2 offset;
        public bool dragging;

        public UIPanel MainPanel;
        public UIText text;
        public JuButton platform;
        public JuButton soild;
        public JuButton rope;
        public SpaceWand SpaceWand;


        public override void OnInitialize()
        {
            MainPanel = new() { VAlign = 0.5f, HAlign = 0.5f };
            MainPanel.SetPadding(20f);
            MainPanel.OnMouseDown += (evt, uie) =>
            {
                if (!platform.IsMouseHovering && !soild.IsMouseHovering && !rope.IsMouseHovering)
                {
                    dragging = true;
                    offset = evt.MousePosition - uie.GetPPos();
                }
            };
            MainPanel.OnMouseUp += (evt, uie) => dragging = false;
            MainPanel.OnUpdate += (uie) =>
            {
                if (dragging)
                {
                    uie.SetPPos(Main.MouseScreen - offset);
                    uie.Recalculate();
                }
                if (!Collision.CheckAABBvAABBCollision(uie.GetDimensions().Position(), uie.GetDimensions().ToRectangle().Size(), Vector2.Zero, Main.ScreenSize.ToVector2()))
                {
                    uie.SetPPos(Vector2.Zero);
                    uie.Recalculate();
                }
            };

            text = new("Hello, World!") { HAlign = 0.5f };
            text.SetSize(MyUtils.GetTextSize("Hello, World!"));

            platform = new(MyUtils.GetTexture("UI/SpaceWand/platform").Value, "") { HAlign = 0.5f };
            platform.OnClick += (evt, uie) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                SpaceWand.placeType = SpaceWand.PlaceType.platform;
                UpdateText();
            };
            platform.Top.Pixels = text.Bottom() + 5;

            soild = new(MyUtils.GetTexture("UI/SpaceWand/soild").Value, "") { HAlign = 0.5f };
            soild.OnClick += (evt, uie) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                SpaceWand.placeType = SpaceWand.PlaceType.soild;
                UpdateText();
            };
            soild.Top.Pixels = platform.Bottom() + 10f;

            rope = new(MyUtils.GetTexture("UI/SpaceWand/rope").Value, "") { HAlign = 0.5f };
            rope.OnClick += (evt, uie) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                SpaceWand.placeType = SpaceWand.PlaceType.rope;
                UpdateText();
            };
            rope.Top.Pixels = soild.Bottom() + 10f;

            MainPanel.Width.Pixels = 175 + MainPanel.HPadding();
            MainPanel.Height.Pixels = rope.Bottom() + MainPanel.VPadding();
            MainPanel.AppendS(text, platform, soild, rope);
            Append(MainPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SpaceWand is not null)
            {
                if (Main.LocalPlayer.HeldItem != SpaceWand.Item)
                {
                    Close();
                }
            }

            if (MainPanel.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public float GetWidestUI() => MyUtils.GetWidestUI(text, platform, soild, rope);

        public void UpdateText()
        {
            string str = $"{MyUtils.GetText("SpaceWand.placeType")}: {MyUtils.GetText($"SpaceWand.{SpaceWand.placeType}")}";
            text.SetText(str);
            text.SetSize(MyUtils.GetTextSize(str));
            text.Recalculate();
        }

        public void UpdateButton()
        {
            platform.SetText($"{MyUtils.GetText("SpaceWand.platform")}");
            soild.SetText($"{MyUtils.GetText("SpaceWand.soild")}");
            rope.SetText($"{MyUtils.GetText("SpaceWand.rope")}");
            platform.Width.Pixels = 175;
            soild.Width.Pixels = 175;
            rope.Width.Pixels = 175;
            platform.TextAlignCenter();
            soild.TextAlignCenter();
            rope.TextAlignCenter();
            MyUtils.RecalculateS(platform, soild, rope);
        }

        public void Open(SpaceWand spaceWand)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            SpaceWand = spaceWand;
            Visible = true;
            UpdateText();
            UpdateButton();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}
