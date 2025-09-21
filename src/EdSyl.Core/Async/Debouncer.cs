namespace EdSyl.Async;

/// <summary>
/// Delays execution of a specified action or asynchronous function until a specified time interval has elapsed.
/// </summary>
public sealed class Debouncer : IDisposable
{
    private static readonly TimerCallback Callback = s => ((Debouncer)s!).Execute().Forget();

    private readonly Timer timer;
    private readonly object work;
    private readonly TimeSpan delay;
    private TaskCompletionSource? tcs;

    public Debouncer(Action action, TimeSpan delay) : this((object)action, delay) { }
    public Debouncer(Func<Task> func, TimeSpan delay) : this((object)func, delay) { }

    private Debouncer(object work, TimeSpan delay)
    {
        this.work = work;
        this.delay = delay;
        timer = new(Callback, this, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Triggers the debouncer so that associated action or asynchronous function will be executed after the delay,
    /// unless <see cref="Invoke" /> is called again within the interval, resetting the timer.
    /// </summary>
    public Task Invoke()
    {
        timer.Restart(delay);
        return (tcs ??= new()).Task;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        timer.Dispose();
    }

    private Task Execute() => work switch
    {
        Action action => Execute(tcs!, action),
        Func<Task> func => Execute(tcs!, func),
        _ => Task.CompletedTask,
    };

    private static Task Execute(TaskCompletionSource tcs, Action action)
    {
        try
        {
            action();
            tcs.TrySetResult();
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            tcs.TrySetException(e);
            return Task.CompletedTask;
        }
    }

    private static async Task Execute(TaskCompletionSource tcs, Func<Task> func)
    {
        try
        {
            await func();
            tcs.TrySetResult();
        }
        catch (Exception e)
        {
            tcs.TrySetException(e);
        }
    }
}
