namespace EdSyl.Collections;

public static class CharComparer
{
    /// <summary> Compare characters with the provided comparer. </summary>
    /// <param name="x">First character to compare.</param>
    /// <param name="y">Second character to compare.</param>
    /// <param name="comparer">Optional comparer to use; fallback to ordinal compare.</param>
    public static int Compare(this char x, char y, IComparer<char>? comparer) => comparer?.Compare(x, y) ?? x - y;

    /// <summary>Compare chars using culture-sensitive sort rules, the current culture, and ignoring the case of the chars being compared.</summary>
    public static readonly IUnifiedComparer<char> IgnoreCase = new CurrentCultureIgnoreCase();

    /// <summary>Compare chars using culture-sensitive sort rules, the invariant culture, and ignoring the case of the chars being compared.</summary>
    public static readonly IUnifiedComparer<char> InvariantIgnoreCase = new InvariantCultureIgnoreCase();

    [SuppressMessage("Usage", "MA0011", Justification = "CurrentCulture by Design")]
    [SuppressMessage("Globalization", "CA1304", Justification = "CurrentCulture by Design")]
    private sealed class CurrentCultureIgnoreCase : IUnifiedComparer<char>
    {
        public int Compare(char x, char y) => x != y ? char.ToUpper(x) - char.ToUpper(y) : 0;
        public bool Equals(char x, char y) => x == y || char.ToUpper(x) == char.ToUpper(y);
        public int GetHashCode(char x) => char.ToUpper(x).GetHashCode();
    }

    private sealed class InvariantCultureIgnoreCase : IUnifiedComparer<char>
    {
        public int Compare(char x, char y) => x != y ? char.ToUpperInvariant(x) - char.ToUpperInvariant(y) : 0;
        public bool Equals(char x, char y) => x == y || char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
        public int GetHashCode(char x) => char.ToUpperInvariant(x).GetHashCode();
    }
}
