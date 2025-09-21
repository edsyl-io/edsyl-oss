using static System.StringSplitOptions;

namespace EdSyl.Razor;

public readonly struct ClassName : IEquatable<ClassName>
{
    public static bool operator ==(ClassName left, ClassName right) => left.Equals(right);
    public static bool operator !=(ClassName left, ClassName right) => !left.Equals(right);
    public static implicit operator ClassName(string? klass) => new(klass);
    public static ClassName FromString(string klass) => new(klass);
    private readonly ReadOnlyMemory<char>[] tokens;

    /// <summary> Determines if this is an empty class list. </summary>
    public bool Empty => tokens is not { Length: > 0 };

    /// <summary> List of class name tokens. </summary>
    public ReadOnlySpan<ReadOnlyMemory<char>> Span => tokens;

    /// <summary>Initializes a new instance of the <see cref="ClassName" /> struct. </summary>
    /// <param name="klass">CSS class name.</param>
    private ClassName(string? klass) => tokens = Tokenize(klass.AsMemory());

    /// <inheritdoc />
    public bool Equals(ClassName other)
        => other.Span.SequenceEqual(tokens, StringMemoryComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is ClassName other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => Span.ToHashCode(StringMemoryComparer.OrdinalIgnoreCase);

    private static ReadOnlyMemory<char>[] Tokenize(ReadOnlyMemory<char> klass)
    {
        const char separator = ' ';
        const StringSplitOptions options = TrimEntries | RemoveEmptyEntries;

        // calculate length
        var length = 0;
        var it = klass.EnumerateSplits(separator, options);
        while (it.MoveNext()) length++;
        if (length == 0) return [];

        // tokenize
        var array = new ReadOnlyMemory<char>[length];
        it = klass.EnumerateSplits(separator, options);
        for (var i = 0; it.MoveNext(); i++)
            array[i] = klass[it.Current];

        // order for comparison
        array.AsSpan().Sort(StringMemoryComparer.OrdinalIgnoreCase);
        return array;
    }
}
