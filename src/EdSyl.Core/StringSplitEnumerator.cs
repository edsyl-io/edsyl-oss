using System.Runtime.InteropServices;

namespace EdSyl;

/// <summary>Splits a string into substrings based on a specified delimiting character and options.</summary>
[StructLayout(LayoutKind.Auto)]
public ref struct StringSplitEnumerator
{
    private readonly char separator;
    private readonly ReadOnlySpan<char> span;
    private readonly StringSplitOptions options;

    private int head;
    private int tail;
    private int next;

    /// <summary>Initializes a new instance of the <see cref="StringSplitEnumerator" /> struct.</summary>
    /// <param name="span">Sequence to enumerate.</param>
    /// <param name="separator">A character that delimits the substrings in this string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    public StringSplitEnumerator(ReadOnlySpan<char> span, char separator, StringSplitOptions options)
    {
        next = head = tail = -1;
        this.span = span;
        this.options = options;
        this.separator = separator;
    }

    /// <summary> Current range. </summary>
    public readonly Range Current => new(head, tail);

    /// <summary> Returns this instance as an enumerator. </summary>
    public readonly StringSplitEnumerator GetEnumerator() => this;

    /// <summary> Advances the enumerator to the next region in a sequence. </summary>
    public bool MoveNext()
    {
        while (true)
        {
            // initialize / exit / advance
            if (head < 0) head = 0;
            else if (next >= span.Length) return false;
            else head = next + 1;

            // seek next
            tail = next = head + span[head..].IndexOf(separator);

            // complete
            if (tail < head)
                next = tail = span.Length;

            // trim
            if ((options & StringSplitOptions.TrimEntries) != default)
                while (head < tail)
                    if (span[head] == ' ') head++;
                    else if (span[tail - 1] == ' ') tail--;
                    else break;

            // remove empty
            if ((options & StringSplitOptions.RemoveEmptyEntries) != default)
                if (head >= tail)
                    continue;

            return true;
        }
    }
}
