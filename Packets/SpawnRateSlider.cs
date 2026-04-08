using ImproveGame.Modules.InfiniteBuff;

namespace ImproveGame.Packets;

[AutoSync]
public class SpawnRateSlider : NetModule
{
    private int _whoAmI;
    private float _sliderValue;

    public static SpawnRateSlider Get(int whoAmI, float sliderValue)
    {
        var module = NetModuleLoader.Get<SpawnRateSlider>();
        module._sliderValue = sliderValue;
        module._whoAmI = whoAmI;
        return module;
    }

    public override void Receive()
    {
        if (!Main.player[_whoAmI].TryGetModPlayer<SpawnRateSliderValueModPlayer>(out var battler))
            return;

        battler.SetSpawnRateSliderValue(_sliderValue);

        if (Main.netMode is NetmodeID.Server) Send(-1, _whoAmI, false);
    }
}
