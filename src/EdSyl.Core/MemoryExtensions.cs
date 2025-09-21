using System.Globalization;
using System.Runtime.InteropServices;

namespace EdSyl;

public static class MemoryExtensions
{
    /// <summary> Compute hashcode for a sequence. </summary>
    /// <param name="span">Sequence to compute hash for.</param>
    /// <typeparam name="T">Type of the elements in a sequence.</typeparam>
    public static int ToHashCode<T>(this Span<T> span)
    {
        HashCode hash = default;
        foreach (var item in span)
            hash.Add(item);

        return hash.ToHashCode();
    }

    /// <summary> Compute hashcode for a sequence. </summary>
    /// <param name="span">Sequence to compute hash for.</param>
    /// <typeparam name="T">Type of the elements in a sequence.</typeparam>
    public static int ToHashCode<T>(this ReadOnlySpan<T> span)
    {
        HashCode hash = default;
        foreach (var item in span)
            hash.Add(item);

        return hash.ToHashCode();
    }

    /// <summary> Compute hashcode for a sequence. </summary>
    /// <param name="span">Sequence to compute hash for.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}" /> to use to calculate the hash code.</param>
    /// <typeparam name="T">Type of the elements in a sequence.</typeparam>
    public static int ToHashCode<T>(this ReadOnlySpan<T> span, IEqualityComparer<T> comparer)
    {
        HashCode hash = default;
        foreach (var item in span)
            hash.Add(item, comparer);

        return hash.ToHashCode();
    }

    /// <summary> Check if the sequence elements are sorted. </summary>
    /// <param name="span">Sequence to check.</param>
    /// <param name="distinct">Whether all elements must be unique.</param>
    /// <typeparam name="T">Type of the elements in a sequence.</typeparam>
    public static bool IsSorted<T>(this ReadOnlySpan<T> span, bool distinct = false)
    {
        var c = distinct ? -1 : 0;
        for (var i = 1; i < span.Length; i++)
            if (Comparer<T>.Default.Compare(span[i - 1], span[i]) > c)
                return false;

        return true;
    }

    public static int SequenceCompareTo<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, IComparer<T> comparer)
    {
        if (Unsafe.AreSame(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(other)))
            return span.Length - other.Length;

        var length = Math.Min(span.Length, other.Length);

        var c = 0;
        for (var i = 0; c == 0 && i < length; i++)
            c = comparer.Compare(span[i], other[i]);

        if (c == 0)
            c = span.Length - other.Length;

        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CommonPrefixLength(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, out int order)
    {
        var size = Math.Min(span.Length, other.Length);
        var length = span.CommonPrefixLength(other);
        order = length < size ? span[length] - other[length] : 0;
        return length;
    }

    public static int CommonPrefixLength(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, IComparer<char>? comparer, out int order)
    {
        if (comparer == null)
            return CommonPrefixLength(span, other, out order);

        var length = order = 0;
        var size = Math.Min(span.Length, other.Length);
        while (length < size && (order = comparer.Compare(span[length], other[length])) == 0)
            length++;

        return length;
    }

    /// <summary> Get a new mutable span over a string by using its pointer to the first character. </summary>
    /// <param name="value">Target string.</param>
    public static Span<char> AsSpanUnsafe(this string value)
    {
        unsafe
        {
            fixed (char* pointer = value)
                return new(pointer, value.Length);
        }
    }

    /// <summary> Get a string presentation of a string slice. </summary>
    /// <param name="slice">Slice of a string.</param>
    /// <param name="reference">Reference string used to create a slice.</param>
    /// <returns>A reference string when a slice points to the same string</returns>
    public static string AsString(this Span<char> slice, string reference)
    {
        unsafe
        {
            fixed (char* a = slice, b = reference)
                if (a == b)
                    return slice.Length != reference.Length
                        ? reference[..slice.Length]
                        : reference;
        }

        return slice.ToString();
    }

    /// <summary> Trim a provided prefix from a span. </summary>
    /// <param name="span">Span to trim.</param>
    /// <param name="prefix">Prefix to search for.</param>
    /// <param name="comparer">Character comparer.</param>
    public static Span<char> TrimPrefix(this Span<char> span, ReadOnlySpan<char> prefix, IEqualityComparer<char> comparer)
    {
        return span.CommonPrefixLength(prefix, comparer) == prefix.Length
            ? span[prefix.Length..]
            : span;
    }

    /// <summary> Ensure a sentence case by toggling the first letter to uppercase. </summary>
    /// <param name="span">Sentence segment.</param>
    public static Span<char> ToSentenceCase(this Span<char> span)
    {
        span[0] = char.ToUpper(span[0], CultureInfo.InvariantCulture);
        return span;
    }

    /// <summary> Splits a sequence into ranges based on a specified delimiting value. </summary>
    /// <param name="span">Sequence to enumerate.</param>
    /// <param name="separator">A value that delimits the regions in the sequence.</param>
    /// <typeparam name="T">Type of elements in a sequence.</typeparam>
    public static SpanSplitEnumerator<T> EnumerateSplits<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>?
        => new(span, separator);

    /// <summary> Splits a sequence into ranges based on a specified delimiting value. </summary>
    /// <param name="memory">Sequence to enumerate.</param>
    /// <param name="separator">A value that delimits the regions in the sequence.</param>
    /// <typeparam name="T">Type of elements in a sequence.</typeparam>
    public static SpanSplitEnumerator<T> EnumerateSplits<T>(this ReadOnlyMemory<T> memory, T separator) where T : IEquatable<T>?
        => new(memory.Span, separator);

    /// <summary>Splits a string into substrings based on a specified delimiting character and options.</summary>
    /// <param name="span">Sequence to enumerate.</param>
    /// <param name="separator">A character that delimits the substrings in this string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    public static StringSplitEnumerator EnumerateSplits(this ReadOnlySpan<char> span, char separator, StringSplitOptions options)
        => new(span, separator, options);

    /// <summary>Splits a string into substrings based on a specified delimiting character and options.</summary>
    /// <param name="memory">Sequence to enumerate.</param>
    /// <param name="separator">A character that delimits the substrings in this string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    public static StringSplitEnumerator EnumerateSplits(this ReadOnlyMemory<char> memory, char separator, StringSplitOptions options)
        => new(memory.Span, separator, options);
}
