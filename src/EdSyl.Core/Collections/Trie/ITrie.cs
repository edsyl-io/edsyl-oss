namespace EdSyl.Collections;

public interface ITrie : IReadOnlyList<IRadixEdge>
{
    /// <inheritdoc cref="ISet{T}.Add" />
    bool Add(ReadOnlySpan<char> key);

    /// <inheritdoc cref="ISet{T}.Contains" />
    bool Contains(ReadOnlySpan<char> key);

    /// <summary> Check if any prefix matches the beginning of the provided term. </summary>
    /// <param name="term">Term to be searched for prefix matches.</param>
    /// <returns>True when found any key matching a beginning of a term; false otherwise.</returns>
    bool TryMatch(ReadOnlySpan<char> term);

    /// <summary> Removes all items. </summary>
    void Clear();
}

/// <summary> Represents a Prefix Tree storing value by a prefix. </summary>
/// <typeparam name="TValue">Type of value stored by a prefix.</typeparam>
public interface ITrie<TValue> : IReadOnlyList<IRadixEdge<TValue>>
{
    /// <inheritdoc cref="IDictionary{TKey,TValue}.this" />
    [SuppressMessage("Design", "CA1043", Justification = "Optimization")]
    TValue this[ReadOnlySpan<char> key] { get; set; }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.ContainsKey" />
    bool ContainsKey(ReadOnlySpan<char> key);

    /// <inheritdoc cref="IDictionary{TKey,TValue}.Add(TKey, TValue)" />
    void Add(ReadOnlySpan<char> key, TValue value);

    /// <summary>
    /// Attempts to add key-value pair to the tree if such the key not already present.
    /// No modifications will take place if such key already present.
    /// </summary>
    /// <param name="key">Key to add.</param>
    /// <param name="value">Value to associate with the key.</param>
    /// <returns>True when key was successfully added; false if such key already present.</returns>
    bool TryAdd(ReadOnlySpan<char> key, TValue value);

    /// <inheritdoc cref="IDictionary{TKey,TValue}.TryGetValue" />
    bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value);

    /// <summary> Check if provided term begins with any key. </summary>
    /// <param name="term">Term to be searched for prefix matches.</param>
    /// <returns>True when found any key matching a beginning of a term; false otherwise.</returns>
    bool TryMatch(ReadOnlySpan<char> term);

    /// <summary> Find the value associated with the longest key that matching the beginning of the provided term. </summary>
    /// <param name="term">Term to be searched for prefix matches.</param>
    /// <param name="value">Value associated with the longest prefix if any.</param>
    /// <returns>True when found any key matching a beginning of a term; false otherwise.</returns>
    bool TryMatch(ReadOnlySpan<char> term, [MaybeNullWhen(false)] out TValue value);

    /// <summary> Removes all items. </summary>
    void Clear();
}
