using ImproveGame.Common.Configs;
using ImproveGame.Interface;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.GlobalGUI.FunctionList;

[AutoCreateGUI(VanillaLayer.RadialHotbars, "Function List GUI")]
public class FunctionListGUI : BaseBody
{
    public override bool Enabled { get => Main.playerInventory && _enabled; set => _enabled = value; }
    private bool _enabled = true;
    public override bool CanSetFocusTarget(UIElement target) => false;

    public SUIPanel Window { get; private set; }
    public SUIText BigBagButton { get; private set; }
    public SUIText SettingButton { get; private set; }

    public override void OnInitialize()
    {
        Window = new SUIPanel(Color.Transparent, Color.Transparent);
        Window.SetPadding(0f);
        Window.IsAdaptiveWidth = Window.IsAdaptiveHeight = true;
        // Window.Height.Pixels = 80f;
        Window.SetAlign(1f, 1f);
        Window.SetPosPixels(-new Vector2(55f, ModLoader.IsEnabled("DragonLens") ? 35f : 15f));
        Window.JoinParent(this);

        BigBagButton = MimicSettingsButton(GetText("SuperVault.Name"), Color.White, Color.Yellow);
        BigBagButton.OnLeftMouseDown += (_, _) =>
        {
            if (!Config.SuperVault)
            {
                if (BigBagGUI.Visible)
                    BigBagGUI.Instance.Close();
                return;
            }

            if (BigBagGUI.Visible)
                BigBagGUI.Instance.Close();
            else
                BigBagGUI.Instance.Open();
        };
        BigBagButton.JoinParent(Window);


        SettingButton = MimicSettingsButton(Lang.inter[62].Value, Color.White, Color.Yellow);
        SettingButton.OnLeftMouseDown += (_, _) =>
        {
            IngameOptions.Open();
        };
        SettingButton.JoinParent(Window);

    }

    public override void CheckWhetherRecalculate(out bool recalculate)
    {
        base.CheckWhetherRecalculate(out recalculate);

        if (Config.SuperVault && UIConfigs.Instance.BigBackpackButton)
        {
            if (!Window.HasChild(BigBagButton))
            {
                recalculate = true;

                Window.RemoveAllChildren();
                Window.Append(BigBagButton);
                Window.Append(SettingButton);
            }
        }
        else
        {
            if (Window.HasChild(BigBagButton))
            {
                recalculate = true;
                Window.RemoveChild(BigBagButton);
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

        button.OnUpdate += (_) =>
        {
            button.TextColor = button.HoverTimer.Lerp(noHoverColor, hoverColor);
            button.TextScale = button.HoverTimer.Lerp(0.85f, 1f);
        };

        return button;
    }
}
