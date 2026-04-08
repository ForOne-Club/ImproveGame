using CommunityToolkit.Mvvm.ComponentModel;
using ImproveGame.Packets;
using Terraria.ModLoader.IO;

namespace ImproveGame.Modules.InfiniteBuff;

[INotifyPropertyChanged]
public partial class SpawnRateSliderValueModPlayer : ModPlayer
{
    [ObservableProperty]
    public partial bool ShowSlider { get; set; } = true;

    [ObservableProperty]
    public partial float SpawnRateSliderValue { get; private set; } = 0.5f;

    public void SetSpawnRateSliderValue(float value)
    {
        SpawnRateSliderValue = MathF.Round(value / 0.01f) * 0.01f;
        SpawnRateSlider.Get(Player.whoAmI, SpawnRateSliderValue).Send();
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) => SpawnRateSlider.Get(Player.whoAmI, SpawnRateSliderValue).Send();

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet<float>(nameof(SpawnRateSliderValue), out var spawnRateSliderValue))
        {
            SpawnRateSliderValue = spawnRateSliderValue;
        }
        else SpawnRateSliderValue = 0.5f;

        if (tag.TryGet<bool>(nameof(ShowSlider), out var showSlider))
        {
            ShowSlider = showSlider;
        }
        else ShowSlider = true;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(SpawnRateSliderValue)] = SpawnRateSliderValue;
        tag[nameof(ShowSlider)] = ShowSlider;
    }
}
