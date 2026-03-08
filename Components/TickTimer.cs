namespace ImproveGame.Components;

public sealed class TickTimer
{
    private TimeSpan _last;
    public double IntervalSeconds { get; set; }
    private readonly Action _callback;

    public TickTimer(double intervalSeconds, Action callback)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(intervalSeconds, 0f);

        IntervalSeconds = intervalSeconds;
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public void Tick(GameTime gameTime)
    {
        if ((gameTime.TotalGameTime - _last).TotalSeconds < IntervalSeconds) return;
        _last = gameTime.TotalGameTime;
        _callback.Invoke();
    }
}
