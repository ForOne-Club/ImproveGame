using ImproveGame.Interface.SUIElements;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace ImproveGame.GlobalGUI.ControlCenter;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Control Center GUI", 100)]
public class ControlCenterGUI : BaseBody
{
    public readonly AnimationTimer OOTimer = new();
    public override bool Enabled { get => Keyboard.GetState().IsKeyDown(Keys.OemTilde) || OOTimer.AnyOpen || OOTimer.Closing; set { } }
    public override bool CanSetFocusTarget(UIElement target) => Window.IsMouseHovering;
    public override bool IsNotSelectable => OOTimer.AnyClose;

    public SUIPanel Window { get; init; } = new(Color.Transparent, Color.Transparent);
    public SUIScrollView2 ListView { get; init; } = new(ScrollType.Vertical);

    public override void OnInitialize()
    {
        // BgColor = Color.White * 0.25f;
        Opacity.Type = OpacityType.Own;

        Window.SetPadding(0f);
        Window.IsAdaptiveWidth = Window.IsAdaptiveHeight = true;
        Window.SetAlign(0.5f, 0.5f);
        Window.FinallyDrawBorder = true;
        Window.SetRoundedRectProperties(UIStyle.PanelBg, 2f, UIStyle.PanelBorder, 12);
        Window.JoinParent(this);

        var title = ViewHelper.CreateHead(Color.Black * 0.3f, 45f, 12f);
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

        var tail = ViewHelper.CreateTail(Color.Black * 0.3f, 35f, 12f);
        tail.JoinParent(Window);

        var version = new SUIText
        {
            TextScale = 0.9f,
            TextOrKey = $"版本号: {ImproveGame.Instance.Version}",
            TextAlign = new Vector2(0.5f),
            TextBorder = 1.5f,
            HAlign = 1f,
        };
        version.Width.Pixels = version.TextSize.X * version.TextScale;
        version.Height.Percent = 1f;
        version.JoinParent(tail);
    }

    public override void CheckWhetherRecalculate(out bool recalculate)
    {
        base.CheckWhetherRecalculate(out recalculate);

        if (OOTimer.Lerp(0, 1) != Opacity.Value)
        {
            Opacity.SetValue(OOTimer.Lerp(0, 1));
            recalculate = true;
        }
    }

    public override void Update(GameTime gameTime)
    {
        OOTimer.Update();

        base.Update(gameTime);

        if (Window.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll($"{ImproveGame.Instance.Name}: Control Center GUI");
        }

        if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
        {
            if (!OOTimer.AnyOpen)
            {
                OOTimer.OpenAndResetTimer();
            }
        }
        else
        {
            if (!OOTimer.AnyClose)
            {
                OOTimer.CloseAndResetTimer();
            }
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
