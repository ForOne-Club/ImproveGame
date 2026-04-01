using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.CreateWand;

[RegisterUI]
public partial class CreateWandUI : BaseBody
{
    public override IEnumerable<UIView> BlurElements => [MainContainer];

    protected override void OnInitialize()
    {
        InitializeComponent();

        MainContainer.BorderColor = SUIColor.Border;
        MainContainer.BackgroundColor = SUIColor.Background * 0.75f;

        Header.ControlTarget = this;
        Title.UseDeathText();

        var searchCancel = Main.Assets.Request<Texture2D>("Images/UI/SearchCancel");
        Close.Texture2D = searchCancel;
        Close.LeftMouseDown += delegate { Enabled = false; };
        Close.OnUpdateStatus += delegate
        {
            Close.ImageColor = Color.White * Close.HoverTimer.Lerp(0.5f, 1f);
        };
    }
}