namespace EdSyl.Collections;

public static class TrieExtensions
{
    /// <summary> Find the value associated with the longest prefix that matches the beginning of the provided term. </summary>
    /// <param name="trie">Prefix tree </param>
    /// <param name="term">Term to be searched for prefix matches.</param>
    /// <returns>Value associated with the longest prefix if any; default value otherwise.</returns>
    /// <typeparam name="TValue">Type of value stored by a prefix.</typeparam>
    public static TValue? Match<TValue>(this ITrie<TValue> trie, ReadOnlySpan<char> term)
    {
        return trie.TryMatch(term, out var value) ? value : default;
    }
}
