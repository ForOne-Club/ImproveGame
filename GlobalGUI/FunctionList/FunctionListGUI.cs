using ImproveGame.Interface.SUIElements;

namespace ImproveGame.GlobalGUI.FunctionList;

[AutoCreateGUI("Radial Hotbars", "Function List GUI")]
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
        Window.IsAdaptiveWidth = true;
        Window.Height.Pixels = 90f;
        Window.SetAlign(1f, 1f);
        Window.SetPosPixels(-new Vector2(55f, 25f));

        Window.JoinParent(this);

        InitializeBigBagButton();
        InitializeSettingButton();

    }

    public void InitializeBigBagButton()
    {
        BigBagButton = new SUIText
        {
            RelativeMode = RelativeMode.Horizontal,
            TextOrKey = GetText("SuperVault.Name"),
            TextPercentOffset = Vector2.One / 2f,
            TextOrigin = Vector2.One / 2f,
            IsLarge = true,
        };
        BigBagButton.SetSize(BigBagButton.TextSize * 1.1f);
        BigBagButton.SetAlign(0f, 0.5f);

        BigBagButton.OnMouseOver += (_, _) =>
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        };

        BigBagButton.OnLeftMouseDown += (_, _) =>
        {
            if (!Config.SuperVault)
            {
                if (BigBagGUI.Visible)
                {
                    BigBagGUI.Instance.Close();
                }

                return;
            }

            if (BigBagGUI.Visible)
            {
                BigBagGUI.Instance.Close();
            }
            else
            {
                BigBagGUI.Instance.Open();
            }
        };

        BigBagButton.OnUpdate += (_) =>
        {
            BigBagButton.TextColor = BigBagButton.HoverTimer.Lerp(Color.White, Config.SuperVault ? Color.Yellow : Color.Red);
            BigBagButton.TextScale = BigBagButton.HoverTimer.Lerp(1f, 1.1f);
        };
        BigBagButton.JoinParent(Window);
    }

    public void InitializeSettingButton()
    {
        SettingButton = new SUIText
        {
            RelativeMode = RelativeMode.Horizontal,
            Spacing = new Vector2(8f, 0f),
            TextOrKey = Lang.inter[62].Value,
            TextPercentOffset = Vector2.One / 2f,
            TextOrigin = Vector2.One / 2f,
            IsLarge = true,
        };
        SettingButton.SetSize(SettingButton.TextSize * 1.1f);
        SettingButton.SetAlign(0f, 0.5f);

        SettingButton.OnMouseOver += (_, _) =>
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        };

        SettingButton.OnLeftMouseDown += (_, _) =>
        {
            IngameOptions.Open();
        };

        SettingButton.OnUpdate += (_) =>
        {
            SettingButton.TextColor = SettingButton.HoverTimer.Lerp(Color.White, Color.Yellow);
            SettingButton.TextScale = SettingButton.HoverTimer.Lerp(1f, 1.1f);
        };
        SettingButton.JoinParent(Window);
    }
}
