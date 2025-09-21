namespace EdSyl.Async;

public static class Timers
{
    /// <summary> Creates a debounced version of the given function. </summary>
    /// <param name="func">The function to debounce.</param>
    /// <param name="delay">The debouncing delay.</param>
    /// <remarks> Use to execute a function after a certain amount of time since it was called the last time. </remarks>
    /// <remarks> https://gist.github.com/rent-a-developer/d1e832d9b0272dd9678f5d5d54f1a44e </remarks>
    [SuppressMessage("Reliability", "CA2000", Justification = "Consumer to use debouncer directly to control lifecycle.")]
    public static Func<Task> Debounce(this Func<Task> func, TimeSpan delay) => new Debouncer(func, delay).Invoke;

    public static Timer Create(Action callback)
    {
        return new(_ => callback(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public static Timer Create(TimerCallback callback, object? state)
    {
        return new(callback, state, Timeout.Infinite, Timeout.Infinite);
    }

    public static Timer Create<T>(Action<T> callback, T? state)
    {
        return new(s => callback((T)s), state, Timeout.Infinite, Timeout.Infinite);
    }

    public static void Restart(this Timer timer, int delay, int period = Timeout.Infinite)
    {
        timer.Change(delay, period);
    }

    public static void Restart(this Timer timer, TimeSpan delay, int period = Timeout.Infinite)
    {
        timer.Change((long)delay.TotalMilliseconds, period);
    }

    public static void Stop(this Timer timer)
    {
        timer.Change(Timeout.Infinite, Timeout.Infinite);
    }
}
