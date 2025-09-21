namespace EdSyl.Collections;

internal static class RadixEdgeExtensions
{
    public static TValue Reduce<TValue>(this IEnumerable<IRadixEdge> edges, TValue seed, Func<TValue, IRadixEdge, TValue> func)
    {
        foreach (var edge in edges)
            seed = Reduce(edge, func(seed, edge), func);

        return seed;
    }

    public static TValue Reduce<T, TValue>(this IEnumerable<IRadixEdge<T>> edges, TValue seed, Func<TValue, IRadixEdge<T>, TValue> func)
    {
        foreach (var edge in edges)
            seed = Reduce(edge, func(seed, edge), func);

        return seed;
    }

    public static IRadixEdge? Find(this IReadOnlyList<IRadixEdge> edges, ReadOnlySpan<char> key, IComparer<char>? comparer)
    {
        var edge = edges.Get(key, comparer, out var length);
        while (edge != null && key.Length > length)
        {
            key = key[length..];
            edge = edge.Get(key, comparer, out length);
        }

        return edge is { Leaf: true } ? edge : null;
    }

    public static IRadixEdge? Match(this IReadOnlyList<IRadixEdge>? edges, ReadOnlySpan<char> key, IComparer<char>? comparer, out int length)
    {
        length = default;
        IRadixEdge? match = default;

        while (edges != null && length < key.Length)
        {
            var edge = edges.Get(key[length..], comparer, out var common);
            if (edge is { Leaf: true }) match = edge;
            length += common;
            edges = edge;
        }

        return match;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IRadixEdge? Get(this IReadOnlyList<IRadixEdge> edges, ReadOnlySpan<char> key, IComparer<char>? comparer, out int length, bool exact = true)
    {
        var index = BinarySearch(edges, key, comparer, out length, exact);
        return index >= 0 ? edges[index] : default;
    }

    public static int BinarySearch(this IReadOnlyList<IRadixEdge> edges, ReadOnlySpan<char> key, IComparer<char>? comparer, out int length, bool exact)
    {
        var lo = 0;
        var hi = edges.Count - 1;
        while (lo <= hi)
        {
            var i = lo + ((hi - lo) >> 1);
            var label = edges[i].Label;
            length = key.CommonPrefixLength(label, comparer, out var order);

            // check match
            if (exact ? length == label.Length : length > 0)
                return i;

            // compare first letters to choose direction
            if (order > 0) lo = i + 1;
            else hi = i - 1;
        }

        length = default;
        return ~lo;
    }
}
