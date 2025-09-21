namespace EdSyl.Async;

[SuppressMessage("Design", "CA1724", Justification = "Extension")]
public static partial class Tasks
{
    /// <summary> Reference to <see cref="Task.FromResult{TResult}" /> method. </summary>
    public static readonly MethodInfo FromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))!;

    public static async Task<TResult> Map<TSource, TResult>(this Task<TSource> task, Func<TSource, TResult> map)
        => map(await task);

    [SuppressMessage("Design", "MA0016:Prefer returning collection abstraction instead of implementation", Justification = "Same as IEnumerable<T>.ToList<T>")]
    public static async Task<List<TSource>> ToList<TSource>(this Task<IEnumerable<TSource>> task)
        => (await task).ToList();

    [SuppressMessage("Design", "MA0016:Prefer returning collection abstraction instead of implementation", Justification = "Same as List<T>.ConvertAll")]
    public static async Task<List<TResult>> ConvertAll<T, TResult>(this Task<List<T>> task, Converter<T, TResult> selector)
        => (await task).ConvertAll(selector);

    public static void Forget(this Task task)
    {
        if (!task.IsCompleted) _ = WaitForget(task);
        else if (task.IsFaulted) HandleTaskFailure(task);
    }

    public static void Forget(this ValueTask task)
    {
        if (!task.IsCompleted) _ = WaitForget(task.AsTask());
        else if (task.IsFaulted) HandleTaskFailure(task.AsTask());
    }

    public static void Forget<T>(this ValueTask<T> task)
    {
        if (!task.IsCompleted) _ = WaitForget(task.AsTask());
        else if (task.IsFaulted) HandleTaskFailure(task.AsTask());
    }

    /// <summary> Run tasks in a sequence, one at a time. </summary>
    /// <param name="tasks">Sequence of tasks to run.</param>
    /// <param name="callback">Callback to invoke after each task completion.</param>
    public static async Task Sequentially(this IEnumerable<Task> tasks, Action callback)
    {
        foreach (var task in tasks)
        {
            await task;
            callback();
        }
    }

    /// <summary> Invoke the provided callback when the task completes. </summary>
    /// <param name="task">Task to wait for completion.</param>
    /// <param name="callback">Callback to invoke when the task completes.</param>
    public static async Task Tap(this Task task, Action callback)
    {
        await task;
        callback();
    }

    /// <summary> Invoke the provided callback when the task completes. </summary>
    /// <typeparam name="T">The type of the result produced by task.</typeparam>
    /// <param name="task">Task to wait for completion.</param>
    /// <param name="callback">Callback to invoke when the task completes.</param>
    public static async Task<T> Tap<T>(this Task<T> task, Action callback)
    {
        var result = await task;
        callback();
        return result;
    }

    private static async Task WaitForget(Task task)
    {
        try { await task; }
        catch { HandleTaskFailure(task); }
    }

    private static void HandleTaskFailure(Task task)
        => Console.Error.WriteLine(task.Exception);
}
