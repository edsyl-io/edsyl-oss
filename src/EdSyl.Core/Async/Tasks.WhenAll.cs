namespace EdSyl.Async;

public static partial class Tasks
{
    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<T1> WhenAll<T1>(Task<T1> a, Task b, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        await Task.WhenAll(t1, t2);
        return await t1;
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> a, Task<T2> b, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        return (await t1, await t2);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> a, Task<T2> b, Task<T3> c, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        return (await t1, await t2, await t3);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> a, Task<T2> b, Task<T3> c, Task<T4> d, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        var t4 = d.WaitAsync(cancellation);
        return (await t1, await t2, await t3, await t4);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Task<T1> a, Task<T2> b, Task<T3> c, Task<T4> d, Task<T5> e, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        var t4 = d.WaitAsync(cancellation);
        var t5 = e.WaitAsync(cancellation);
        return (await t1, await t2, await t3, await t4, await t5);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3, T4, T5, T6)> WhenAll<T1, T2, T3, T4, T5, T6>(Task<T1> a, Task<T2> b, Task<T3> c, Task<T4> d, Task<T5> e, Task<T6> f, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        var t4 = d.WaitAsync(cancellation);
        var t5 = e.WaitAsync(cancellation);
        var t6 = f.WaitAsync(cancellation);
        return (await t1, await t2, await t3, await t4, await t5, await t6);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3, T4, T5, T6, T7)> WhenAll<T1, T2, T3, T4, T5, T6, T7>(Task<T1> a, Task<T2> b, Task<T3> c, Task<T4> d, Task<T5> e, Task<T6> f, Task<T7> g, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        var t4 = d.WaitAsync(cancellation);
        var t5 = e.WaitAsync(cancellation);
        var t6 = f.WaitAsync(cancellation);
        var t7 = g.WaitAsync(cancellation);
        return (await t1, await t2, await t3, await t4, await t5, await t6, await t7);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(Task<T1> a, Task<T2> b, Task<T3> c, Task<T4> d, Task<T5> e, Task<T6> f, Task<T7> g, Task<T8> h, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        var t4 = d.WaitAsync(cancellation);
        var t5 = e.WaitAsync(cancellation);
        var t6 = f.WaitAsync(cancellation);
        var t7 = g.WaitAsync(cancellation);
        var t8 = h.WaitAsync(cancellation);
        return (await t1, await t2, await t3, await t4, await t5, await t6, await t7, await t8);
    }

    /// <inheritdoc cref="Task.WhenAll(Task[])" />
    public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Task<T1> a, Task<T2> b, Task<T3> c, Task<T4> d, Task<T5> e, Task<T6> f, Task<T7> g, Task<T8> h, Task<T9> i, CancellationToken cancellation = default)
    {
        var t1 = a.WaitAsync(cancellation);
        var t2 = b.WaitAsync(cancellation);
        var t3 = c.WaitAsync(cancellation);
        var t4 = d.WaitAsync(cancellation);
        var t5 = e.WaitAsync(cancellation);
        var t6 = f.WaitAsync(cancellation);
        var t7 = g.WaitAsync(cancellation);
        var t8 = h.WaitAsync(cancellation);
        var t9 = i.WaitAsync(cancellation);
        return (await t1, await t2, await t3, await t4, await t5, await t6, await t7, await t8, await t9);
    }
}
