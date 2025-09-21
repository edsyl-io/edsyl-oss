using System.Runtime.InteropServices;

namespace EdSyl;

/// <summary> Splits a sequence into ranges based on a specified delimiting value. </summary>
/// <typeparam name="T">Type of elements in a sequence.</typeparam>
[StructLayout(LayoutKind.Auto)]
public ref struct SpanSplitEnumerator<T> where T : IEquatable<T>?
{
    private readonly T separator;
    private readonly ReadOnlySpan<T> span;

    private int head;
    private int tail;

    /// <summary>Initializes a new instance of the <see cref="SpanSplitEnumerator{T}" /> struct.</summary>
    /// <param name="span">Sequence to enumerate.</param>
    /// <param name="separator">A value that delimits the regions in the sequence.</param>
    public SpanSplitEnumerator(ReadOnlySpan<T> span, T separator)
    {
        head = tail = -1;
        this.span = span;
        this.separator = separator;
    }

    /// <summary> Current range. </summary>
    public readonly Range Current => new(head, tail);

    /// <summary> Returns this instance as an enumerator. </summary>
    public readonly SpanSplitEnumerator<T> GetEnumerator() => this;

    /// <summary> Advances the enumerator to the next region in a sequence. </summary>
    public bool MoveNext()
    {
        // initialize / exit / advance
        if (head < 0) head = 0;
        else if (tail >= span.Length) return false;
        else head = tail + 1;

        // seek next
        tail = head + span[head..].IndexOf(separator);

        // complete
        if (tail < head)
            tail = span.Length;

        return true;
    }
}
