using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.CreateWand;

[RegisterUI]
public partial class CreateWandUI : BaseBody
{
    public static CreateWandUI Instance { get; private set; }

    public void Toggle()
    {
        Enabled = !Enabled;
    }

    public override IEnumerable<UIView> BlurElements => [MainContainer];

    protected override void OnInitialize()
    {
        Instance = this;

        InitializeComponent();

        MainContainer.BorderColor = SUIColor.Border;
        MainContainer.BackgroundColor = SUIColor.Background * 0.75f;

        Header.ControlTarget = this;
        Title.UseDeathText();

        Add.Texture2D = ModAsset.Add;

        Download.Texture2D = ModAsset.Download;

        X.Texture2D = ModAsset.X;
        X.LeftMouseDown += delegate { Enabled = false; };

        SetHoverAnim(Add, Download, X);
    }

    static void SetHoverAnim(params SUIImage[] images)
    {
        foreach (var image in images)
        {
            image.OnUpdateStatus += (_) => image.ImageColor = Color.White * image.HoverTimer.Lerp(0.5f, 1f);
        }
    }
}