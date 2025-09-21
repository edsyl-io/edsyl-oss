using System.Collections;

namespace EdSyl.Collections;

public class RadixTree<T> : ITrie<T>
{
    private readonly List<RadixEdge<T>> edges = [];
    private readonly IComparer<char>? comparer;

    /// <inheritdoc />
    public int Count => edges.Count;

    /// <inheritdoc />
    public IRadixEdge<T> this[int index] => edges[index];

    public RadixTree(IComparer<char>? comparer = default)
    {
        this.comparer = comparer;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1043", Justification = "Optimization")]
    public T this[ReadOnlySpan<char> key]
    {
        get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        set => RadixEdge<T>.Insert(edges, key, value, comparer, InsertionBehavior.OverwriteExisting);
    }

    /// <inheritdoc />
    public void Add(ReadOnlySpan<char> key, T value)
    {
        RadixEdge<T>.Insert(edges, key, value, comparer, InsertionBehavior.ThrowOnExisting);
    }

    /// <inheritdoc />
    public bool TryAdd(ReadOnlySpan<char> key, T value)
    {
        return RadixEdge<T>.Insert(edges, key, value, comparer, InsertionBehavior.None);
    }

    /// <inheritdoc />
    public bool ContainsKey(ReadOnlySpan<char> key)
    {
        return edges.Find(key, comparer) != null;
    }

    /// <inheritdoc />
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value)
    {
        if (edges.Find(key, comparer) is RadixEdge<T> edge)
            return edge.TryGetValue(out value);

        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryMatch(ReadOnlySpan<char> term)
    {
        return edges.Match(term, comparer, out var length) != null
            && ValidateMatch(term, term[..length]);
    }

    /// <inheritdoc />
    public bool TryMatch(ReadOnlySpan<char> term, [MaybeNullWhen(false)] out T value)
    {
        if (edges.Match(term, comparer, out var length) is RadixEdge<T> edge && ValidateMatch(term, term[..length]))
            return edge.TryGetValue(out value);

        value = default;
        return false;
    }

    /// <inheritdoc />
    public void Clear()
    {
        edges.Clear();
    }

    /// <summary> Returns an enumerator that iterates through the collection. </summary>
    public List<RadixEdge<T>>.Enumerator GetEnumerator() => edges.GetEnumerator();

    /// <inheritdoc />
    IEnumerator<IRadixEdge<T>> IEnumerable<IRadixEdge<T>>.GetEnumerator() => edges.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => edges.GetEnumerator();

    /// <summary> Check if provided key is valid match for the given term. </summary>
    /// <param name="term">Term to be searched for prefix matches.</param>
    /// <param name="key">A key found as a prefix match.</param>
    protected virtual bool ValidateMatch(ReadOnlySpan<char> term, ReadOnlySpan<char> key) => true;
}
