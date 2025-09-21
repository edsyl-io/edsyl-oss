using System.Runtime.CompilerServices;

namespace EdSyl.Razor;

public interface ILifecycle : IDisposable
{
    /// <summary> Dead lifecycle that always returns a cancelled <see cref="Token" />. </summary>
    static readonly ILifecycle Dead = new Dead();

    /// <summary> Create new instance of the <see cref="Lifecycle" />. </summary>
    static ILifecycle New() => new Lifecycle();

    /// <summary> Token to be canceled upon destruction. </summary>
    CancellationToken Token { get; }
}

public static class Lifecycles
{
    public static CancellationToken Cancellation(this ILifecycle lifecycle, ILifecycle? other)
        => CreateLinkedToken(lifecycle.Token, other?.Token ?? default);

    private static CancellationToken CreateLinkedToken(CancellationToken a, CancellationToken b)
    {
        if (!a.CanBeCanceled) return b;
        if (!b.CanBeCanceled) return a;
        return CancellationTokenSource.CreateLinkedTokenSource(a, b).Token;
    }
}

file sealed class Lifecycle : ILifecycle
{
    private static readonly CancellationTokenSource Cancelled = GetCancelledSource(default!);

    private CancellationTokenSource? cts;

    /// <inheritdoc />
    public CancellationToken Token => (cts ??= new()).Token;

    /// <inheritdoc />
    public void Dispose()
    {
        var source = Interlocked.Exchange(ref cts, Cancelled);
        if (source == null || source == Cancelled) return;

        source.Cancel();
        source.Dispose();
    }

    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "s_canceledSource")]
    private static extern ref CancellationTokenSource GetCancelledSource(CancellationTokenSource @this);
}

file sealed class Dead : ILifecycle
{
    private static readonly CancellationToken Cancelled = new(true);

    /// <inheritdoc />
    public CancellationToken Token => Cancelled;

    /// <inheritdoc />
    public void Dispose() { }
}
