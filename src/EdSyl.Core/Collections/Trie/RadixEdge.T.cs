using System.Collections;
using static EdSyl.Collections.ThrowHelper;

namespace EdSyl.Collections;

/// <summary> Represents an edge of the Radix Tree, might be either a leaf with an associated value or a non-leaf node. </summary>
/// <typeparam name="T">Type of the value stored at leaf nodes.</typeparam>
public sealed partial class RadixEdge<T> : IRadixEdge<T>
{
    private static readonly List<RadixEdge<T>> Empty = [];

    private T? value;
    private bool leaf;
    private string label;
    private List<RadixEdge<T>>? edges;

    /// <inheritdoc />
    public bool Leaf => leaf;

    /// <inheritdoc />
    public ReadOnlySpan<char> Label => label;

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
    public int Count => edges?.Count ?? 0;

    /// <inheritdoc />
    public IRadixEdge<T> this[int index] => (edges ?? Empty)[index];

    /// <inheritdoc />
    IRadixEdge IReadOnlyList<IRadixEdge>.this[int index] => (edges ?? Empty)[index];

    public RadixEdge()
    {
        label = string.Empty;
    }

    public RadixEdge(ReadOnlySpan<char> label, T value)
    {
        leaf = true;
        this.value = value;
        this.label = label.Length > 0 ? new(label) : string.Empty;
    }

    private RadixEdge(RadixEdge<T> other)
    {
        leaf = other.leaf;
        label = other.label;
        edges = other.edges;
        value = other.value;
    }

    /// <summary> Attempt to get value of the edge. </summary>
    /// <param name="value">Value of the edge on success; default otherwise.</param>
    /// <returns>True if edge is a leaf and contains a value; false otherwise.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        value = this.value;
        return leaf;
    }

    /// <inheritdoc />
    public override string ToString() => label;

    /// <summary> Returns an enumerator that iterates through the collection. </summary>
    public List<RadixEdge<T>>.Enumerator GetEnumerator() => (edges ?? Empty).GetEnumerator();

    /// <inheritdoc />
    IEnumerator<IRadixEdge> IEnumerable<IRadixEdge>.GetEnumerator() => (edges ?? Empty).GetEnumerator();

    /// <inheritdoc />
    IEnumerator<IRadixEdge<T>> IEnumerable<IRadixEdge<T>>.GetEnumerator() => (edges ?? Empty).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => (edges ?? Empty).GetEnumerator();
}

public partial class RadixEdge<T>
{
    internal static bool Insert(List<RadixEdge<T>> edges, ReadOnlySpan<char> key, T value, IComparer<char>? comparer, InsertionBehavior behavior)
    {
        while (key.Length > 0)
        {
            // binary search for common prefix
            var index = edges.BinarySearch(key, comparer, out var length, exact: false);

            // insert if no common prefix
            RadixEdge<T> edge;
            if (index < 0)
            {
                edge = new(key, value);
                edges.Insert(~index, edge);
                return true;
            }

            edge = edges[index];
            key = key[length..];

            // split into two leafs with common prefix
            if (length < edge.label.Length)
            {
                Split(edge, length, key, value, comparer);
                return true;
            }

            // exact match
            if (key.IsEmpty)
            {
                // already exists?
                if (edge.leaf)
                {
                    switch (behavior)
                    {
                        case InsertionBehavior.OverwriteExisting:
                            edge.value = value;
                            break;
                        case InsertionBehavior.ThrowOnExisting:
                            throw AddingDuplicateWithKeyArgumentException(key.ToString(), nameof(key));
                    }

                    return false;
                }

                edge.leaf = true;
                edge.value = value;
                return true;
            }

            // advance
            edges = edge.edges ??= [];
        }

        return false;
    }

    private static RadixEdge<T> Split(RadixEdge<T> stem, int length, ReadOnlySpan<char> key, T value, IComparer<char>? comparer)
    {
        var copy = new RadixEdge<T>(stem);
        (stem.label, copy.label) = stem.label.Split(length);

        if (key.IsEmpty)
        {
            stem.edges = [copy];
            stem.value = value;
            stem.leaf = true;
            return stem;
        }

        var suffix = new RadixEdge<T>(key, value);
        stem.edges = ToList(copy, suffix, comparer);
        stem.value = default;
        stem.leaf = false;
        return suffix;
    }

    private static List<RadixEdge<T>> ToList(RadixEdge<T> x, RadixEdge<T>? y, IComparer<char>? comparer)
    {
        if (y == null) return [x];
        return x.label[0].Compare(y.label[0], comparer) < 0
            ? [x, y]
            : [y, x];
    }
}
