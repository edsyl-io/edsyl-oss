using System.Collections;

namespace EdSyl.Collections;

public class RadixTree : ITrie
{
    private readonly List<RadixEdge> edges = [];
    private readonly IComparer<char>? comparer;

    /// <inheritdoc />
    public int Count => edges.Count;

    /// <inheritdoc />
    public IRadixEdge this[int index] => edges[index];

    public RadixTree(IComparer<char>? comparer = default)
    {
        this.comparer = comparer;
    }

    public RadixTree(IEnumerable<string> keys, IComparer<char>? comparer = default)
    {
        this.comparer = comparer;
        AddRange(keys);
    }

    /// <inheritdoc />
    public bool Add(ReadOnlySpan<char> key)
    {
        return RadixEdge.Insert(edges, key, comparer, InsertionBehavior.None);
    }

    public void AddRange(IEnumerable<string> keys)
    {
        foreach (var key in keys)
            RadixEdge.Insert(edges, key, comparer, InsertionBehavior.None);
    }

    /// <inheritdoc />
    public bool Contains(ReadOnlySpan<char> key)
    {
        return edges.Find(key, comparer) != null;
    }

    /// <inheritdoc />
    public bool TryMatch(ReadOnlySpan<char> term)
    {
        return edges.Match(term, comparer, out _) != null;
    }

    /// <inheritdoc />
    public void Clear()
    {
        edges.Clear();
    }

    /// <summary> Returns an enumerator that iterates through the collection. </summary>
    public List<RadixEdge>.Enumerator GetEnumerator() => edges.GetEnumerator();

    /// <inheritdoc />
    IEnumerator<IRadixEdge> IEnumerable<IRadixEdge>.GetEnumerator() => edges.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => edges.GetEnumerator();
}
