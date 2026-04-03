using CommunityToolkit.Mvvm.ComponentModel;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.CreateWand;

public partial class CreateWandViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; }
}

[RegisterUI]
public partial class CreateWandController : BaseBody
{
    public static CreateWandController Instance { get; private set; }

    public void Toggle() => Enabled = !Enabled;

    public override IEnumerable<UIView> BlurElements => [MainContainer];

    private readonly CreateWandViewModel _vm = new();

    protected override void OnInitialize()
    {
        Instance = this;

        InitializeComponent();

        MainContainer.BorderColor = SUIColor.Border;
        MainContainer.BackgroundColor = SUIColor.Background * 0.75f;

        Header.ControlTarget = this;
        Title.UseDeathText();

        _vm.PropertyChanged += (s, e) =>
        {
            if (!e.PropertyName.Equals("Title")) return;
            if (s is not CreateWandViewModel vm) return;
            Title.Text = vm.Title;
        };

        Add.Texture2D = ModAsset.Add;
        Download.Texture2D = ModAsset.Download;
        X.Texture2D = ModAsset.X;
        X.LeftMouseDown += delegate { Enabled = false; };

        SetHoverAnim(Add, Download, X);

        SetHoverAnim2(MaterialButton, StructureButton);
    }

    static void SetHoverAnim(params SUIImage[] images)
    {
        foreach (var image in images)
        {
            image.OnUpdateStatus += (_) => image.ImageColor = Color.White * image.HoverTimer.Lerp(0.5f, 1f);
        }
    }

    static void SetHoverAnim2(params UIView[] buttons)
    {
        foreach (var button in buttons)
        {
            button.OnUpdateStatus += (_) => button.BackgroundColor = Color.Black * button.HoverTimer.Lerp(0f, 0.25f);
        }
    }
}