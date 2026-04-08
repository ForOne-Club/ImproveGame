using CommunityToolkit.Mvvm.ComponentModel;

namespace ImproveGame.UserInterfaces.CreateWand;

public partial class CreateWandViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = GetText("UI.CreateWandController.Title");

    [ObservableProperty]
    public partial Asset<Texture2D> Download { get; set; } = ModAsset.Download;

}
