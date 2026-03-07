namespace ImproveGame.Modules.InfiniteBuff;

public sealed class SpawnRateSliderViewModel : IDisposable
{
    private readonly SpawnRateSliderValueModPlayer _model;

    public event EventHandler<float> SliderValueChanged;

    public SpawnRateSliderViewModel()
    {
        _model = Main.LocalPlayer.GetModPlayer<SpawnRateSliderValueModPlayer>();
        _model.SpawnRateSliderValueChanged += OnModelSliderValueChanged;
    }

    /// <summary>
    /// 给 View 绑定的滑块值，范围 [0,1]
    /// </summary>
    public float SliderValue => _model.SpawnRateSliderValue;

    /// <summary>
    /// 给 View 显示的实际刷怪倍率文本
    /// </summary>
    public string SpawnRateText => $"{InfiniteBuffHelper.RemapSliderToSpawnRate(SliderValue):0.##}";

    /// <summary>
    /// 用户拖动滑块
    /// </summary>
    public void SetSliderValue(float value)
    {
        _model.SetSpawnRateSliderValue(value);
    }

    private void OnModelSliderValueChanged(object sender, float value)
    {
        SliderValueChanged?.Invoke(this, value);
    }

    public void Dispose()
    {
        _model.SpawnRateSliderValueChanged -= OnModelSliderValueChanged;
    }
}