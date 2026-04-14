using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImproveGame.Styles;
using SilkyUIFramework.Interfaces;
using System.ComponentModel;

namespace ImproveGame.Modules.InfiniteBuff.UserInterface.ViewModel;

public partial class InfiniteBuffLocalization : ObservableObject
{
    private static string GetTextValue(string key) => Language.GetTextValue($"Mods.ImproveGame.UI.InfiniteBUFFController.{key}");

    [ObservableProperty]
    public partial string Title { get; set; } = GetTextValue("DisplayName");

    [ObservableProperty]
    public partial string Placeholder { get; set; } = GetTextValue("Placeholder");

    [ObservableProperty]
    public partial string EnemySpawnRate { get; set; } = GetTextValue("EnemySpawnRate");
}

public sealed partial class InfiniteBuffViewModel : ObservableObject, IUpdatable, IDisposable
{
    [ObservableProperty]
    public partial object Style { get; set; } = new DefaultStyle();

    [ObservableProperty]
    public partial InfiniteBuffLocalization Localization { get; set; } = new();

    [ObservableProperty]
    public partial Asset<Texture2D> SearchCancel { get; set; } = Main.Assets.Request<Texture2D>("Images/UI/SearchCancel");

    [ObservableProperty]
    public partial Asset<Texture2D> EyeSwitch { get; set; } = ModAsset.EyeSwitch;

    private readonly SpawnRateSliderValueModPlayer _model;

    public InfiniteBuffViewModel()
    {
        _model = Main.LocalPlayer.GetModPlayer<SpawnRateSliderValueModPlayer>();
        _model.PropertyChanged += OnModelPropertyChanged;

        OpenSlider = _model.ShowSlider;
        SliderValue = _model.SpawnRateSliderValue;
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not SpawnRateSliderValueModPlayer player) return;
        switch (e.PropertyName)
        {
            case nameof(SpawnRateSliderValueModPlayer.ShowSlider):
            {
                OpenSlider = player.ShowSlider;
                break;
            }
            case nameof(SpawnRateSliderValueModPlayer.SpawnRateSliderValue):
            {
                SliderValue = player.SpawnRateSliderValue;
                break;
            }
        }
    }

    [ObservableProperty]
    public partial float SliderValue { get; private set; }

    [RelayCommand]
    private void SetSliderValue(float value) => _model.SetSpawnRateSliderValue(value);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HiddenSlider))]
    public partial bool HiddenSliderEyeButton { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HiddenSlider))]
    [NotifyPropertyChangedFor(nameof(EyeColor))]
    public partial bool OpenSlider { get; private set; }

    public bool HiddenSlider => HiddenSliderEyeButton || !OpenSlider;

    public Color EyeColor => OpenSlider ? Color.White : Color.White * 0.5f;

    [RelayCommand] public void ToggleSliderVisibility() => _model.ShowSlider = !_model.ShowSlider;

    void IUpdatable.Update(GameTime gameTime)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffModPlayer>(out var infinitePlayer)) return;
        HiddenSliderEyeButton = !infinitePlayer.MeetsBattlerCombination();
    }

    public void Dispose() => _model.PropertyChanged -= OnModelPropertyChanged;
}