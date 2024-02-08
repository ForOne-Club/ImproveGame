using ImproveGame.Content.Functions;

namespace ImproveGame.Packets.Weather;

[AutoSync]
public class WeatherLockerPacket : NetModule
{
    private byte _states;

    private bool RainLocked => ((BitsByte) _states)[0];
    private bool MoonPhaseLocked => ((BitsByte) _states)[1];
    private bool WindLocked => ((BitsByte) _states)[2];

    public static void ToggleRain() => SetRain(!WeatherController.RainLocked);

    public static void SetRain(bool locked) =>
        SetStates(locked, WeatherController.MoonPhaseLocked, WeatherController.WindLocked);

    public static void ToggleMoonPhase() => SetMoonPhase(!WeatherController.MoonPhaseLocked);

    public static void SetMoonPhase(bool locked) =>
        SetStates(WeatherController.RainLocked, locked, WeatherController.WindLocked);

    public static void ToggleWind() => SetWind(!WeatherController.WindLocked);

    public static void SetWind(bool locked) =>
        SetStates(WeatherController.RainLocked, WeatherController.MoonPhaseLocked, locked);

    /// <summary>
    /// 这个包只要一发，就会解锁气候控制功能，所以“解锁”就是发个包罢了。
    /// 至于三个false，解锁的时候重置状态没问题罢（
    /// </summary>
    public static void Unlock() => SetStates(false, false, false);

    public static void SetStates(bool rain, bool moonPhase, bool wind)
    {
        var states = new BitsByte(rain, moonPhase, wind);
        var module = NetModuleLoader.Get<WeatherLockerPacket>();
        module._states = states;
        module.Send(runLocally: true);
    }

    public override void Receive()
    {
        WeatherController.RainLocked = RainLocked;
        WeatherController.MoonPhaseLocked = MoonPhaseLocked;
        WeatherController.WindLocked = WindLocked;
        WeatherController.Unlocked = true;

        if (Main.netMode is NetmodeID.Server)
            Send(-1, Sender);
    }
}

[AutoSync]
public class SetTimePacket : NetModule
{
    private int _time;
    private bool _setIsDayTime;

    public static void SetTime(int time, bool setIsDayTime)
    {
        var module = NetModuleLoader.Get<SetTimePacket>();
        module._time = time;
        module._setIsDayTime = setIsDayTime;
        module.Send(runLocally: Main.netMode is NetmodeID.SinglePlayer);
    }

    public override void Receive()
    {
        Main.SkipToTime(_time, _setIsDayTime);
    }
}

[AutoSync]
public class SetRainPacket : NetModule
{
    public static void ToggleRain()
    {
        var module = NetModuleLoader.Get<SetRainPacket>();
        module.Send(runLocally: true);
    }

    public override void Receive()
    {
        if (Main.raining)
        {
            Main.StopRain();
            Main.cloudAlpha = 0f;
            Main.maxRaining = 0f;
        }
        else
        {
            Main.StartRain();
            Main.cloudAlpha = .7f;
            Main.maxRaining = .7f;
        }
    }
}

[AutoSync]
public class SetMoonPhasePacket : NetModule
{
    private byte _moonPhase;

    public static void SetTo(int moonPhase)
    {
        var module = NetModuleLoader.Get<SetMoonPhasePacket>();
        if (moonPhase >= 8)
            moonPhase = 0;
        module._moonPhase = (byte) moonPhase;
        module.Send(runLocally: true);
    }

    public override void Receive()
    {
        Main.moonPhase = _moonPhase;

        if (Main.netMode is NetmodeID.Server)
            Send(-1, Sender);
    }
}

[AutoSync]
public class SetWindPacket : NetModule
{
    public enum WindStage : sbyte
    {
        West = 1,
        No = 0,
        East = -1
    }

    private sbyte _windStage;

    public static void SetTo(WindStage stage)
    {
        var module = NetModuleLoader.Get<SetWindPacket>();
        module._windStage = (sbyte) stage;
        module.Send(runLocally: true);
    }

    public override void Receive()
    {
        Main.windSpeedCurrent = Main.windSpeedTarget = _windStage * 0.5f;
        // 皮一下，不要完全没风
        if (_windStage is 0)
            Main.windSpeedCurrent = Main.windSpeedTarget = Main.rand.NextFloat(-0.04f, 0.04f);

        if (Main.netMode is NetmodeID.Server)
            Send(-1, Sender);
    }
}