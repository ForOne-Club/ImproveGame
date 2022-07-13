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

        public UIPanel MainPanel;
        public UIText text;
        public JuButton platform;
        public JuButton soild;
        public JuButton rope;
        public SpaceWand SpaceWand;


        public override void OnInitialize()
        {
            MainPanel = new() { VAlign = 0.5f, HAlign = 0.5f };
            MainPanel.SetVPadding(20f);

            text = new("Hello, World!") { HAlign = 0.5f };
            text.SetSize(MyUtils.GetTextSize("Hello, World!"));

            platform = new(MyUtils.GetTexture("UI/SpaceWand/platform").Value, "") { HAlign = 0.5f };
            platform.OnClick += (evt, uie) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                SpaceWand.placeType = SpaceWand.PlaceType.platform;
            };
            platform.Top.Pixels = text.Bottom() + 5;

            soild = new(MyUtils.GetTexture("UI/SpaceWand/soild").Value, "") { HAlign = 0.5f };
            soild.OnClick += (evt, uie) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                SpaceWand.placeType = SpaceWand.PlaceType.soild;
            };
            soild.Top.Pixels = platform.Bottom() + 10f;

            rope = new(MyUtils.GetTexture("UI/SpaceWand/rope").Value, "") { HAlign = 0.5f };
            rope.OnClick += (evt, uie) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                SpaceWand.placeType = SpaceWand.PlaceType.rope;
            };
            rope.Top.Pixels = soild.Bottom() + 10f;

            MainPanel.Width.Pixels = 180 + MainPanel.HPadding();
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

        public override void MouseDown(UIMouseEvent evt)
        {
            text.SetText($"{MyUtils.GetText("SpaceWand.placeType")}: {MyUtils.GetText($"SpaceWand.{SpaceWand.placeType}")}");
            text.Recalculate();
            base.MouseDown(evt);
        }

        public void Open(SpaceWand spaceWand)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            SpaceWand = spaceWand;
            Visible = true;
            text.SetText($"{MyUtils.GetText("SpaceWand.placeType")}: {MyUtils.GetText($"SpaceWand.{SpaceWand.placeType}")}");
            platform.SetText($"{MyUtils.GetText("SpaceWand.platform")}");
            soild.SetText($"{MyUtils.GetText("SpaceWand.soild")}");
            rope.SetText($"{MyUtils.GetText("SpaceWand.rope")}");
            text.Recalculate();
            platform.Recalculate();
            soild.Recalculate();
            rope.Recalculate();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}
