using ImproveGame.Interface.Attributes;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.GUI.FunctionList;

[AutoCreateGUI("Radial Hotbars", "Function List GUI")]
public class FunctionListGUI : BaseBody
{
    public override bool Enabled { get => _enabled && Main.playerInventory; set => _enabled = value; }
    private bool _enabled = true;

    public override bool CanDisableMouse(UIElement target)
    {
        return false;
    }

    public override bool CanPriority(UIElement target)
    {
        return false;
    }

    public SUIPanel Window;

    public SUIText BigBagButton { get; private set; }
    public SUIText SettingButton { get; private set; }

    public override void OnInitialize()
    {
        Window = new SUIPanel(Color.Transparent, Color.Black * .0f);
        Window.SetPadding(0f);
        Window.Left.Percent = Window.Top.Percent = 1f;

        Window.Join(this);

        InitializeBigBagButton();
        InitializeSettingButton();

        Window.SetSizePixels(new Vector2((BigBagButton.TextSize.X + SettingButton.TextSize.X) * 1.2f + 8f, 90f));
        Window.SetPosPixels(-Window.GetSize() - new Vector2(55f, 25f));
    }

    public void InitializeBigBagButton()
    {
        BigBagButton = new SUIText
        {
            Relative = RelativeMode.Horizontal,
            TextOrKey = GetText("SuperVault.Name"),
            TextPercentOffset = Vector2.One / 2f,
            TextOrigin = Vector2.One / 2f,
            IsLarge = true,
        };
        BigBagButton.SetSize(BigBagButton.TextSize * 1.2f);
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
            BigBagButton.TextScale = BigBagButton.HoverTimer.Lerp(1f, 1.2f);
        };
        BigBagButton.Join(Window);
    }

    public void InitializeSettingButton()
    {
        SettingButton = new SUIText
        {
            Relative = RelativeMode.Horizontal,
            Spacing = new Vector2(8f, 0f),
            TextOrKey = Lang.inter[62].Value,
            TextPercentOffset = Vector2.One / 2f,
            TextOrigin = Vector2.One / 2f,
            IsLarge = true,
        };
        SettingButton.SetSize(SettingButton.TextSize * 1.2f);
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
            SettingButton.TextScale = SettingButton.HoverTimer.Lerp(1f, 1.2f);
        };
        SettingButton.Join(Window);
    }
}
