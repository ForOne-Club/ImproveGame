using ImproveGame.Interface.SUIElements;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace ImproveGame.GlobalGUI.FunctionList;

[AutoCreateGUI(VanillaLayer.RadialHotbars, "Control Center GUI")]
public class ControlCenterGUI : BaseBody
{
    public readonly AnimationTimer OOHover = new();
    public override bool Enabled { get => Keyboard.GetState().IsKeyDown(Keys.OemTilde) || OOHover.AnyOpen || OOHover.Closing; set { } }
    public override bool CanSetFocusTarget(UIElement target) => Window.IsMouseHovering;

    public SUIPanel Window { get; init; } = new(Color.Transparent, Color.Transparent);
    public SUIScrollView2 ListView { get; init; } = new(ScrollType.Vertical);

    public override void OnInitialize()
    {
        // BgColor = Color.White * 0.25f;

        Window.SetPadding(0f);
        Window.IsAdaptiveWidth = Window.IsAdaptiveHeight = true;
        Window.SetAlign(0.5f, 0.5f);
        Window.FinallyDrawBorder = true;
        Window.SetRoundedRectProperties(UIStyle.PanelBg, 2f, UIStyle.PanelBorder, 12);
        Window.JoinParent(this);

        var title = CreateTitle(Color.Black * 0.3f, 45f, 12f);
        title.JoinParent(Window);

        var titleText = new SUIText
        {
            TextOrKey = "更好的体验 控制中心",
            TextAlign = new Vector2(0.5f),
            TextBorder = 1.5f,
        };
        titleText.Width.Percent = titleText.Height.Percent = 1f;
        titleText.JoinParent(title);

        ListView.RelativeMode = RelativeMode.Horizontal;
        ListView.PreventOverflow = true;
        ListView.Spacing = new Vector2(4f);
        ListView.SetPadding(6f, 0f);
        ListView.SetSizePixels(500, 300);
        ListView.JoinParent(Window);

        foreach (var cci in CCIManager.Instance.ControlCenterItems)
        {
            var button = new SUIText
            {
                UseKey = true,
                TextOrKey = cci.NameKey,
                TextAlign = new Vector2(0.5f),

                RelativeMode = RelativeMode.Horizontal,
                PreventOverflow = true,
                Spacing = new Vector2(4f),

                Border = 2f,
                Rounded = new Vector4(12f),
            };

            button.OnLeftMouseDown += (_, _) =>
            {
                cci.MouseDown(button);
            };

            button.OnUpdate += (_) =>
            {
                cci.Update(button);

                button.BgColor = cci.GetBgColor(button);
                button.BorderColor = cci.GetBorderColor(button);
            };

            button.Width.Pixels = (500 - 12 /* padding */ - 12 /*scrollbar*/ - 2 - 8) / 3f;
            button.Height.Pixels = 45f;

            button.JoinParent(ListView.AdaptiveView);
        }

        var tail = CreateTail(Color.Black * 0.3f, 35f, 12f);
        tail.JoinParent(Window);

        var version = new SUIText
        {
            TextScale = 0.9f,
            TextOrKey = $"版本号: {ImproveGame.Instance.Version}",
            TextAlign = new Vector2(0.5f),
            TextBorder = 1.5f,
            HAlign = 1f,
        };
        version.Width.Pixels = (version.TextSize.X + 30f) * version.TextScale;
        version.Height.Percent = 1f;
        version.JoinParent(tail);
    }

    public override void CheckWhetherRecalculate(out bool recalculate)
    {
        base.CheckWhetherRecalculate(out recalculate);

        OOHover.Update();

        if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
        {
            if (!OOHover.AnyOpen)
            {
                OOHover.OpenAndResetTimer();
            }
        }
        else
        {
            if (!OOHover.AnyClose)
            {
                OOHover.CloseAndResetTimer();
            }
        }

        Opacity.Type = OpacityType.Own;
        Opacity.SetValue(OOHover.Lerp(0, 1));

        recalculate = true;
    }

    public static View CreateTitle(Color bgColor, float height, float rounded)
    {
        var view = new View
        {
            BgColor = bgColor,
            Rounded = new Vector4(rounded, rounded, 0, 0),
            PaddingTop = 1f,
        };
        view.Width.Percent = 1f;
        view.Height.Pixels = height;

        return view;
    }

    public static View CreateTail(Color bgColor, float height, float rounded)
    {
        var view = new View
        {
            BgColor = bgColor,
            Rounded = new Vector4(0, 0, rounded, rounded),
            RelativeMode = RelativeMode.Horizontal,
            PreventOverflow = true,
            Spacing = new Vector2(4f),
            PaddingBottom = 1f,
        };
        view.Width.Percent = 1f;
        view.Height.Pixels = height;

        return view;
    }

    public override void Update(GameTime gameTime)
    {
        EventTriggerManager.SetHeadEventTigger(EventTriggerManager.CurrentEventTrigger);

        base.Update(gameTime);

        if (Window.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ImproveGame: Ach Panel GUI");
        }
    }

    public static SUIText MimicSettingsButton(string text, Color noHoverColor, Color hoverColor)
    {
        var button = new SUIText
        {
            RelativeMode = RelativeMode.Horizontal,
            Spacing = new Vector2(8f, 0f),
            TextOrKey = text,
            TextPercentOffset = Vector2.One / 2f,
            TextOrigin = Vector2.One / 2f,
            IsLarge = true,
        };
        button.SetSize(button.TextSize);
        button.SetAlign(0f, 0.5f);

        button.OnMouseOver += (_, _) =>
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        };

        button.OnUpdate += _ =>
        {
            button.TextColor = button.HoverTimer.Lerp(noHoverColor, hoverColor);
            if (Language.ActiveCulture.Name is "zh-Hans")
                button.TextScale = button.HoverTimer.Lerp(0.85f, 1f);
            else
                button.TextScale = button.HoverTimer.Lerp(0.65f, 0.8f);
        };

        return button;
    }
}
