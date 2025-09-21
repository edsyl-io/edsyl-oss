namespace EdSyl;

[SuppressMessage("Roslynator", "RCS1241", Justification = "By Design")]
public sealed class MemoryEqualityComparer<T> :
    IEqualityComparer<Memory<T>>,
    IEqualityComparer<ReadOnlyMemory<T>>
{
    /// <summary> Default instance of <see cref="MemoryEqualityComparer{T}" /> </summary>
    public static readonly MemoryEqualityComparer<T> Default = new();

    /// <inheritdoc />
    public bool Equals(Memory<T> x, Memory<T> y)
        => x.Span.SequenceEqual(y.Span);

    /// <inheritdoc />
    public bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
        => x.Span.SequenceEqual(y.Span);

    /// <inheritdoc />
    public int GetHashCode(ReadOnlyMemory<T> obj)
        => obj.Span.ToHashCode();

    /// <inheritdoc />
    public int GetHashCode(Memory<T> obj)
        => obj.Span.ToHashCode();
}
