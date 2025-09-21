using System.Collections;

namespace EdSyl;

public sealed class StringMemoryComparer(StringComparison comparison) :
    IComparer<Memory<char>>,
    IComparer<ReadOnlyMemory<char>>,
    IComparer,
    IEqualityComparer<Memory<char>>,
    IEqualityComparer<ReadOnlyMemory<char>>,
    IEqualityComparer
{
    /// <summary> Compare memory using <see cref="StringComparison.CurrentCulture" /> </summary>
    public static readonly StringMemoryComparer CurrentCulture = new(StringComparison.CurrentCulture);

    /// <summary> Compare memory using <see cref="StringComparison.CurrentCultureIgnoreCase" /> </summary>
    public static readonly StringMemoryComparer CurrentCultureIgnoreCase = new(StringComparison.CurrentCultureIgnoreCase);

    /// <summary> Compare memory using <see cref="StringComparison.InvariantCulture" /> </summary>
    public static readonly StringMemoryComparer InvariantCulture = new(StringComparison.InvariantCulture);

    /// <summary> Compare memory using <see cref="StringComparison.InvariantCultureIgnoreCase" /> </summary>
    public static readonly StringMemoryComparer InvariantCultureIgnoreCase = new(StringComparison.InvariantCultureIgnoreCase);

    /// <summary> Compare memory using <see cref="StringComparison.Ordinal" /> </summary>
    public static readonly StringMemoryComparer Ordinal = new(StringComparison.Ordinal);

    /// <summary> Compare memory using <see cref="StringComparison.OrdinalIgnoreCase" /> </summary>
    public static readonly StringMemoryComparer OrdinalIgnoreCase = new(StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public int Compare(Memory<char> x, Memory<char> y)
        => ((ReadOnlyMemory<char>)x).Span.CompareTo(((ReadOnlyMemory<char>)y).Span, comparison);

    /// <inheritdoc />
    public int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
        => x.Span.CompareTo(y.Span, comparison);

    /// <inheritdoc />
    public bool Equals(Memory<char> x, Memory<char> y)
        => ((ReadOnlyMemory<char>)x).Span.Equals(((ReadOnlyMemory<char>)y).Span, comparison);

    /// <inheritdoc />
    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
        => x.Span.Equals(y.Span, comparison);

    /// <inheritdoc />
    public int GetHashCode(Memory<char> obj)
        => string.GetHashCode(obj.Span, comparison);

    /// <inheritdoc />
    public int GetHashCode(ReadOnlyMemory<char> obj)
        => string.GetHashCode(obj.Span, comparison);

    /// <inheritdoc />
    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x == y) return true;
        if (x == null || y == null) return false;
        return (x, y) switch
        {
            (Memory<char> a, Memory<char> b) => Equals(a, b),
            (ReadOnlyMemory<char> a, ReadOnlyMemory<char> b) => Equals(a, b),
            _ => false,
        };
    }

    /// <inheritdoc />
    int IComparer.Compare(object? x, object? y)
    {
        if (x == y) return 0;
        if (y == null) return 1;
        if (x == null) return -1;
        return (x, y) switch
        {
            (Memory<char> a, Memory<char> b) => Compare(a, b),
            (ReadOnlyMemory<char> a, ReadOnlyMemory<char> b) => Compare(a, b),
            _ => 0,
        };
    }

    /// <inheritdoc />
    int IEqualityComparer.GetHashCode(object? obj) => obj switch
    {
        null => 0,
        Memory<char> x => GetHashCode(x),
        ReadOnlyMemory<char> x => GetHashCode(x),
        _ => throw new ArgumentException(string.Empty, nameof(obj)),
    };
}
