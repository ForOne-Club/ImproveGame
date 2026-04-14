using CommunityToolkit.Mvvm.ComponentModel;
using ImproveGame.Packets;
using Terraria.ModLoader.IO;

namespace ImproveGame.Modules.InfiniteBuff;

[INotifyPropertyChanged]
public partial class SpawnRateSliderModPlayer : ModPlayer
{
    [ObservableProperty]
    public partial bool ShowSlider { get; set; } = true;

    public float SpawnRateSliderValue
    {
        get; set
        {
            SetProperty(ref field, MathF.Round(value / 0.01f) * 0.01f);

            if (Player.whoAmI != Main.myPlayer || Main.netMode is NetmodeID.Server) return;
            SpawnRateSlider.Get(Player.whoAmI, field).Send();
        }
    } = 0.5f;

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) => SpawnRateSlider.Get(Player.whoAmI, SpawnRateSliderValue).Send(toWho, fromWho);

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
