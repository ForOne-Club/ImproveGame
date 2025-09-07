using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;

namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

/// <summary>
/// 设计中不要动
/// </summary>
[RegisterUI("Vanilla: Radial Hotbars", "InfiniteBUFFController")]
public partial class InfiniteBUFFController : BasicBody
{
    public UIElementGroup ScrollContainer { get; private set; }

    protected override void OnInitialize()
    {
        Enabled = false;

        BorderColor = SUIColor.Border * 0.75f;
        BackgroundColor = SUIColor.Background * 0.75f;

        InitializeComponent();
        ScrollContainer = ScrollView.Container;

        Header.ControlTarget = this;
        Title.UseDeathText();

        Close.Texture2D = Main.Assets.Request<Texture2D>("Images/UI/SearchCancel");
        Close.OnUpdateStatus += delegate
        {
            Close.ImageColor = Color.White * Close.HoverTimer.Lerp(0.5f, 1f);
        };

        ClearEditText.Texture2D = Main.Assets.Request<Texture2D>("Images/UI/SearchCancel");
        ClearEditText.OnUpdateStatus += delegate
        {
            ClearEditText.ImageColor = Color.White * ClearEditText.HoverTimer.Lerp(0.5f, 1f);
        };

        ClearEditText.LeftMouseDown += delegate { FilterBox.Text = ""; };
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
    }
}
