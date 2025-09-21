namespace EdSyl;

public static class Dictionaries
{
    public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey? key)
        => key is not null && dictionary.TryGetValue(key, out var value) ? value : default;

    public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey? key) where TKey : struct
        => key.HasValue && dictionary.TryGetValue(key.Value, out var value) ? value : default;

    /// <inheritdoc cref="CollectionExtensions.GetValueOrDefault{TKey, TValue}(IReadOnlyDictionary{TKey, TValue}, TKey)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue? Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey? key)
        => key is not null && dictionary.TryGetValue(key, out var value) ? value : default;

    /// <summary>Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries.</summary>
    /// <param name="source">A dictionary to trim.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static Dictionary<TKey, TValue> Trim<TKey, TValue>(this Dictionary<TKey, TValue> source) where TKey : notnull
    {
        source.TrimExcess();
        return source;
    }

    /// <summary>Projects each element of a dictionary into a new form.</summary>
    /// <param name="source">A dictionary to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static TResult[] ToArray<TKey, TValue, TResult>(this Dictionary<TKey, TValue> source, Func<KeyValuePair<TKey, TValue>, TResult> selector) where TKey : notnull
    {
        if (source is not { Count: > 0 })
            return [];

        var size = 0;
        var array = new TResult[source.Count];
        foreach (var pair in source)
            array[size++] = selector(pair);

        return array;
    }

    /// <summary>Projects each element of a dictionary values into a new form.</summary>
    /// <param name="source">A dictionary value collection to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static TResult[] ToArray<TKey, TValue, TResult>(this Dictionary<TKey, TValue>.ValueCollection source, Func<TValue, TResult> selector) where TKey : notnull
    {
        if (source is not { Count: > 0 })
            return [];

        var size = 0;
        var array = new TResult[source.Count];
        foreach (var value in source)
            array[size++] = selector(value);

        return array;
    }
}
