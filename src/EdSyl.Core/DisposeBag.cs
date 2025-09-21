using System.Runtime.InteropServices;
using EdSyl.Async;

namespace EdSyl;

/// <summary> Disposable container that can hold one or more disposable items for efficient batch disposal. </summary>
[StructLayout(LayoutKind.Auto)]
public partial struct DisposeBag
{
    private object? frugal;

    /// <summary> Adds an <see cref="IDisposable" /> item to the bag. </summary>
    /// <param name="bag">The bag to add the item to.</param>
    /// <param name="disposable">The disposable item to add.</param>
    public static DisposeBag operator +(DisposeBag bag, IDisposable disposable)
    {
        Add(ref bag.frugal, disposable);
        return bag;
    }

    /// <summary> Adds an <see cref="IDisposable" /> item to the bag. </summary>
    /// <param name="bag">The bag to add the item to.</param>
    /// <param name="disposable">The disposable item to add.</param>
    public static DisposeBag operator +(DisposeBag bag, IAsyncDisposable disposable)
    {
        Add(ref bag.frugal, disposable);
        return bag;
    }

    /// <summary> Removes an <see cref="IDisposable" /> item from the bag. </summary>
    /// <param name="bag">The bag to remove the item from.</param>
    /// <param name="disposable">The disposable item to remove.</param>
    [SuppressMessage("Usage", "CA2225", Justification = "use .Remove")]
    public static DisposeBag operator -(DisposeBag bag, IDisposable disposable)
    {
        Remove(ref bag.frugal, disposable);
        return bag;
    }

    /// <summary> Removes an <see cref="IDisposable" /> item from the bag. </summary>
    /// <param name="bag">The bag to remove the item from.</param>
    /// <param name="disposable">The disposable item to remove.</param
    [SuppressMessage("Usage", "CA2225", Justification = "use .Remove")]
    public static DisposeBag operator -(DisposeBag bag, IAsyncDisposable disposable)
    {
        Remove(ref bag.frugal, disposable);
        return bag;
    }

    /// <summary> Adds an <see cref="IDisposable" /> item to the bag. </summary>
    /// <param name="item">The disposable item to add.</param>
    public void Add(IDisposable item) => Add(ref frugal, item);

    /// <summary> Adds an <see cref="IDisposable" /> item to the bag. </summary>
    /// <param name="item">The disposable item to add.</param>
    public void Add(IAsyncDisposable item) => Add(ref frugal, item);

    /// <summary> Removes an <see cref="IDisposable" /> item from the bag. </summary>
    /// <param name="item">The disposable item to remove.</param>
    public void Remove(IDisposable item) => Remove(ref frugal, item);

    /// <summary> Removes an <see cref="IDisposable" /> item from the bag. </summary>
    /// <param name="item">The disposable item to remove.</param>
    public void Remove(IAsyncDisposable item) => Remove(ref frugal, item);

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose() => Dispose(ref frugal);

    /// <inheritdoc cref="IDisposable.Dispose" />
    public ValueTask DisposeAsync() => DisposeAsync(ref frugal);
}

public partial struct DisposeBag
{
    private static void Add(ref object? frugal, object item)
    {
        if (frugal == null) frugal = item;
        else if (frugal is ISet<object> set) set.Add(item);
        else frugal = new HashSet<object> { frugal, item };
    }

    private static void Remove(ref object? frugal, object item)
    {
        if (frugal == null) return;
        if (frugal == item) frugal = null;
        else if (frugal is ISet<object> bag) bag.Remove(item);
    }

    private static void Dispose(ref object? frugal)
    {
        switch (Nullify(ref frugal))
        {
            case HashSet<object> items:
                Dispose(items);
                break;

            case { } item:
                Dispose(item);
                break;
        }
    }

    private static void Dispose(HashSet<object> items)
    {
        foreach (var item in items)
            Dispose(item);
    }

    private static void Dispose(object? item)
    {
        switch (item)
        {
            case IDisposable sync:
                sync.Dispose();
                break;
            case IAsyncDisposable async:
                async.DisposeAsync().Forget();
                break;
        }
    }

    private static ValueTask DisposeAsync(ref object? frugal) => Nullify(ref frugal) switch
    {
        HashSet<object> items => DisposeAsync(items),
        { } item => DisposeAsync(item),
        _ => default,
    };

    [SuppressMessage("Usage", "MA0100", Justification = "Performance")]
    [SuppressMessage("Roslynator", "RCS1229", Justification = "Performance")]
    [SuppressMessage("Reliability", "CA2012", Justification = "Performance")]
    private static ValueTask DisposeAsync(HashSet<object> items)
    {
        // TODO: stackalloc https://dev.to/maximiliysiss/bending-net-how-to-stack-allocate-reference-types-in-c-73g
        using (Arrays.Rent<Task>(items.Count, out var tasks))
        {
            var size = 0;
            foreach (var item in items)
                if (DisposeAsync(item) is { IsCompletedSuccessfully: false } async)
                    tasks[size++] = async.AsTask();

            return DisposeAsync(tasks.AsSpan(0, size));
        }
    }

    private static ValueTask DisposeAsync(ReadOnlySpan<Task> tasks) => tasks.Length switch
    {
        0 => default,
        1 => new(tasks[0]),
        _ => new(Task.WhenAll(tasks)),
    };

    private static ValueTask DisposeAsync(object? item)
    {
        switch (item)
        {
            case IAsyncDisposable async:
                return async.DisposeAsync();

            case IDisposable sync:
                sync.Dispose();
                return default;

            default:
                return default;
        }
    }

    private static object? Nullify(ref object? curr)
    {
        (var last, curr) = (curr, null);
        return last;
    }
}
